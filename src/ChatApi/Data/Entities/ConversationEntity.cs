using Contracts;

namespace ChatApi.Data.Entities;

public sealed class ConversationEntity
{
    public Guid Id { get; set; }
    public required string CustomerExternalId { get; set; }
    public required string CustomerName { get; set; }
    public string? OrderCode { get; set; }
    public string? Subject { get; set; }
    public ConversationStatus Status { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? ClosedAt { get; set; }
    public ICollection<ParticipantEntity> Participants { get; set; } = new List<ParticipantEntity>();
    public ICollection<MessageEntity> Messages { get; set; } = new List<MessageEntity>();
}
