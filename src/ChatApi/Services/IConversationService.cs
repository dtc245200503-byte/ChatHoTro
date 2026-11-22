using Contracts;

namespace ChatApi.Services;

public interface IConversationService
{
    Task<ConversationDto> CreateAsync(CreateConversationRequest request, CancellationToken cancellationToken);

    Task<IReadOnlyList<ConversationDto>> GetAllAsync(
        ConversationStatus? status,
        CancellationToken cancellationToken);

    Task<ConversationDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyList<MessageDto>?> GetMessagesAsync(Guid id, CancellationToken cancellationToken);

    Task<ConversationDto?> CloseAsync(Guid id, CancellationToken cancellationToken);
}
