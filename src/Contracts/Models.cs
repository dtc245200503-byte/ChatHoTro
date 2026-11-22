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

/// <summary>Payload for starting an e-commerce customer-support conversation.</summary>
public sealed record CreateConversationRequest(
    string CustomerExternalId,
    string CustomerName,
    string? OrderCode = null,
    string? Subject = null,
    string? InitialMessage = null);

/// <summary>Conversation details, participants, and optional order context.</summary>
public sealed record ConversationDto(
    Guid Id,
    ConversationStatus Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset? ClosedAt,
    IReadOnlyList<ParticipantDto> Participants,
    string? OrderCode = null,
    string? Subject = null);

/// <summary>Payload for joining a conversation as a customer or agent.</summary>
public sealed record JoinConversationRequest(
    string ExternalId,
    string ParticipantName,
    ParticipantRole Role);

/// <summary>A customer or agent participating in a conversation.</summary>
public sealed record ParticipantDto(
    Guid Id,
    Guid ConversationId,
    string ExternalId,
    string Name,
    ParticipantRole Role,
    DateTimeOffset JoinedAt);

/// <summary>Payload for sending a message as a conversation participant.</summary>
public sealed record SendMessageRequest(
    Guid ParticipantId,
    string Content);

/// <summary>A message exchanged in a conversation.</summary>
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
