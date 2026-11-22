using ChatApi.Data;
using ChatApi.Data.Entities;
using Contracts;
using Microsoft.EntityFrameworkCore;

namespace ChatApi.Services;

public sealed class ConversationService(
    ChatDbContext dbContext,
    ILogger<ConversationService> logger) : IConversationService
{
    public async Task<ConversationDto> CreateAsync(
        CreateConversationRequest request,
        CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;
        var customer = new ParticipantEntity
        {
            Id = Guid.NewGuid(),
            ExternalId = request.CustomerExternalId,
            Name = request.CustomerName,
            Role = ParticipantRole.Customer,
            JoinedAt = now
        };
        var conversation = new ConversationEntity
        {
            Id = Guid.NewGuid(),
            CustomerExternalId = request.CustomerExternalId,
            CustomerName = request.CustomerName,
            OrderCode = request.OrderCode,
            Subject = request.Subject,
            Status = ConversationStatus.Waiting,
            CreatedAt = now,
            Participants = [customer]
        };
        customer.ConversationId = conversation.Id;

        if (!string.IsNullOrWhiteSpace(request.InitialMessage))
        {
            conversation.Messages.Add(new MessageEntity
            {
                Id = Guid.NewGuid(),
                ConversationId = conversation.Id,
                ParticipantId = customer.Id,
                Content = request.InitialMessage,
                SentAt = now,
                Participant = customer
            });
        }

        dbContext.Conversations.Add(conversation);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Created conversation {ConversationId} for customer {CustomerExternalId}, order {OrderCode}, subject {Subject}",
            conversation.Id,
            conversation.CustomerExternalId,
            conversation.OrderCode,
            conversation.Subject);

        return ToDto(conversation);
    }

    public async Task<IReadOnlyList<ConversationDto>> GetAllAsync(
        ConversationStatus? status,
        CancellationToken cancellationToken)
    {
        var query = dbContext.Conversations.AsNoTracking().Include(conversation => conversation.Participants);
        if (status.HasValue)
        {
            query = query.Where(conversation => conversation.Status == status.Value)
                .Include(conversation => conversation.Participants);
        }

        var conversations = await query
            .OrderByDescending(conversation => conversation.CreatedAt)
            .ToListAsync(cancellationToken);
        logger.LogInformation(
            "Listed {ConversationCount} conversations with status filter {Status}",
            conversations.Count,
            status);
        return conversations.Select(ToDto).ToArray();
    }

    public async Task<ConversationDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var conversation = await dbContext.Conversations
            .AsNoTracking()
            .Include(item => item.Participants)
            .SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
        logger.LogInformation(
            "Retrieved conversation {ConversationId}; found: {Found}",
            id,
            conversation is not null);
        return conversation is null ? null : ToDto(conversation);
    }

    public async Task<IReadOnlyList<MessageDto>?> GetMessagesAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var conversationExists = await dbContext.Conversations
            .AsNoTracking()
            .AnyAsync(conversation => conversation.Id == id, cancellationToken);
        if (!conversationExists)
        {
            logger.LogInformation("Cannot list messages for missing conversation {ConversationId}", id);
            return null;
        }

        var messages = await dbContext.Messages
            .AsNoTracking()
            .Where(message => message.ConversationId == id)
            .Include(message => message.Participant)
            .OrderBy(message => message.SentAt)
            .ToListAsync(cancellationToken);
        logger.LogInformation(
            "Retrieved {MessageCount} messages for conversation {ConversationId}",
            messages.Count,
            id);
        return messages.Select(ToDto).ToArray();
    }

    public async Task<ConversationDto?> CloseAsync(Guid id, CancellationToken cancellationToken)
    {
        var conversation = await dbContext.Conversations
            .Include(item => item.Participants)
            .SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (conversation is null)
        {
            return null;
        }

        if (conversation.Status == ConversationStatus.Closed)
        {
            logger.LogInformation("Conversation {ConversationId} was already closed", id);
            return ToDto(conversation);
        }

        conversation.Status = ConversationStatus.Closed;
        conversation.ClosedAt = DateTimeOffset.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Closed conversation {ConversationId} at {ClosedAt}",
            conversation.Id,
            conversation.ClosedAt);
        return ToDto(conversation);
    }

    private static ConversationDto ToDto(ConversationEntity conversation) =>
        new(
            conversation.Id,
            conversation.Status,
            conversation.CreatedAt,
            conversation.ClosedAt,
            conversation.Participants
                .OrderBy(participant => participant.JoinedAt)
                .Select(ToDto)
                .ToArray(),
            conversation.OrderCode,
            conversation.Subject);

    private static ParticipantDto ToDto(ParticipantEntity participant) =>
        new(
            participant.Id,
            participant.ConversationId,
            participant.ExternalId,
            participant.Name,
            participant.Role,
            participant.JoinedAt);

    private static MessageDto ToDto(MessageEntity message) =>
        new(
            message.Id,
            message.ConversationId,
            message.ParticipantId,
            message.Participant.Name,
            message.Participant.Role,
            message.Content,
            message.SentAt);
}
