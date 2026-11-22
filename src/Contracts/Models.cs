namespace Contracts;

public enum ConversationStatus
{
    Waiting,
    Active,
    Closed
}

public enum ParticipantRole
{
    Customer,
    Agent
}

public sealed record CreateConversationRequest(
    string CustomerExternalId,
    string CustomerName);

public sealed record ConversationDto(
    Guid Id,
    ConversationStatus Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset? ClosedAt,
    IReadOnlyList<ParticipantDto> Participants);

public sealed record JoinConversationRequest(
    string ExternalId,
    string ParticipantName,
    ParticipantRole Role);

public sealed record ParticipantDto(
    Guid Id,
    Guid ConversationId,
    string ExternalId,
    string Name,
    ParticipantRole Role,
    DateTimeOffset JoinedAt);

public sealed record SendMessageRequest(
    Guid ParticipantId,
    string Content);

public sealed record MessageDto(
    Guid Id,
    Guid ConversationId,
    Guid ParticipantId,
    string ParticipantName,
    ParticipantRole ParticipantRole,
    string Content,
    DateTimeOffset SentAt);

public sealed record MessageSentEvent(
    Guid EventId,
    Guid MessageId,
    Guid ConversationId,
    Guid ParticipantId,
    string ParticipantName,
    ParticipantRole ParticipantRole,
    string Content,
    DateTimeOffset SentAt);

public sealed record ConversationClosedEvent(
    Guid EventId,
    Guid ConversationId,
    ConversationStatus Status,
    Guid? ClosedByParticipantId,
    string? ClosedByParticipantName,
    DateTimeOffset ClosedAt);
