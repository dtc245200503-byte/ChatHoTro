namespace ChatApi.Data.Entities;

public sealed class MessageEntity
{
    public Guid Id { get; set; }
    public Guid ConversationId { get; set; }
    public Guid ParticipantId { get; set; }
    public required string Content { get; set; }
    public DateTimeOffset SentAt { get; set; }
    public ConversationEntity Conversation { get; set; } = null!;
    public ParticipantEntity Participant { get; set; } = null!;
}
