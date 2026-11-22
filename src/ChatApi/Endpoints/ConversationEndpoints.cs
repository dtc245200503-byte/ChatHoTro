using Contracts;
using ChatApi.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ChatApi.Endpoints;

public static class ConversationEndpoints
{
    public static IEndpointRouteBuilder MapConversationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var conversations = endpoints.MapGroup("/conversations");

        conversations.MapPost("", CreateAsync)
            .WithName("CreateConversation")
            .WithSummary("Create an e-commerce support conversation")
            .WithDescription("Creates a Waiting conversation with customer and optional order context and opening message.")
            .Produces<ConversationDto>(StatusCodes.Status201Created)
            .ProducesValidationProblem();

        conversations.MapGet("", GetAllAsync)
            .WithName("GetConversations")
            .WithSummary("List support conversations")
            .WithDescription("Returns conversations, optionally filtered by Waiting, Active, or Closed status.")
            .Produces<IReadOnlyList<ConversationDto>>()
            .ProducesValidationProblem();

        conversations.MapGet("/{id:guid}", GetByIdAsync)
            .WithName("GetConversationById")
            .WithSummary("Get a support conversation")
            .WithDescription("Returns a conversation with its participants and optional order context.")
            .Produces<ConversationDto>()
            .Produces(StatusCodes.Status404NotFound);

        conversations.MapGet("/{id:guid}/messages", GetMessagesAsync)
            .WithName("GetConversationMessages")
            .WithSummary("List messages in a conversation")
            .WithDescription("Returns the conversation's messages in chronological order.")
            .Produces<IReadOnlyList<MessageDto>>()
            .Produces(StatusCodes.Status404NotFound);

        conversations.MapPost("/{id:guid}/close", CloseAsync)
            .WithName("CloseConversation")
            .WithSummary("Close a support conversation")
            .WithDescription("Sets the conversation status to Closed and records its closure time.")
            .Produces<ConversationDto>()
            .Produces(StatusCodes.Status404NotFound);

        return endpoints;
    }

    private static async Task<Results<Created<ConversationDto>, ValidationProblem>> CreateAsync(
        CreateConversationRequest request,
        IConversationService conversationService,
        CancellationToken cancellationToken)
    {
        var errors = new Dictionary<string, string[]>();
        if (string.IsNullOrWhiteSpace(request.CustomerExternalId) || request.CustomerExternalId.Length > 128)
        {
            errors[nameof(request.CustomerExternalId)] = ["Customer ID is required and must be at most 128 characters."];
        }

        if (string.IsNullOrWhiteSpace(request.CustomerName) || request.CustomerName.Length > 200)
        {
            errors[nameof(request.CustomerName)] = ["Customer name is required and must be at most 200 characters."];
        }

        if (request.OrderCode?.Length > 64)
        {
            errors[nameof(request.OrderCode)] = ["Order code must be at most 64 characters."];
        }

        if (request.Subject?.Length > 200)
        {
            errors[nameof(request.Subject)] = ["Subject must be at most 200 characters."];
        }

        if (request.InitialMessage?.Length > 4000)
        {
            errors[nameof(request.InitialMessage)] = ["Opening message must be at most 4000 characters."];
        }

        if (errors.Count > 0)
        {
            return TypedResults.ValidationProblem(errors);
        }

        var conversation = await conversationService.CreateAsync(request, cancellationToken);
        return TypedResults.Created($"/conversations/{conversation.Id}", conversation);
    }

    private static async Task<Results<Ok<IReadOnlyList<ConversationDto>>, ValidationProblem>> GetAllAsync(
        string? status,
        IConversationService conversationService,
        CancellationToken cancellationToken)
    {
        ConversationStatus? parsedStatus = null;
        if (!string.IsNullOrWhiteSpace(status))
        {
            var requestedStatus = status.Trim();
            if (!Enum.TryParse<ConversationStatus>(requestedStatus, ignoreCase: true, out var value)
                || !Enum.IsDefined(value)
                || !string.Equals(requestedStatus, value.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                return TypedResults.ValidationProblem(new Dictionary<string, string[]>
                {
                    [nameof(status)] = ["Status must be Waiting, Active, or Closed."]
                });
            }

            parsedStatus = value;
        }

        var conversations = await conversationService.GetAllAsync(parsedStatus, cancellationToken);
        return TypedResults.Ok(conversations);
    }

    private static async Task<Results<Ok<ConversationDto>, NotFound>> GetByIdAsync(
        Guid id,
        IConversationService conversationService,
        CancellationToken cancellationToken)
    {
        var conversation = await conversationService.GetByIdAsync(id, cancellationToken);
        return conversation is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(conversation);
    }

    private static async Task<Results<Ok<IReadOnlyList<MessageDto>>, NotFound>> GetMessagesAsync(
        Guid id,
        IConversationService conversationService,
        CancellationToken cancellationToken)
    {
        var messages = await conversationService.GetMessagesAsync(id, cancellationToken);
        return messages is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(messages);
    }

    private static async Task<Results<Ok<ConversationDto>, NotFound>> CloseAsync(
        Guid id,
        IConversationService conversationService,
        CancellationToken cancellationToken)
    {
        var conversation = await conversationService.CloseAsync(id, cancellationToken);
        return conversation is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(conversation);
    }
}
