using Contracts;

namespace ChatApi.Data.Entities;

public sealed class ParticipantEntity
{
    public Guid Id { get; set; }
    public Guid ConversationId { get; set; }
    public required string ExternalId { get; set; }
    public required string Name { get; set; }
    public ParticipantRole Role { get; set; }
    public DateTimeOffset JoinedAt { get; set; }
    public ConversationEntity Conversation { get; set; } = null!;
    public ICollection<MessageEntity> Messages { get; set; } = new List<MessageEntity>();
}
