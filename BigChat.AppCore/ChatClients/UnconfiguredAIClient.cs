using BigChat.AppCore.Localization;
using Microsoft.Extensions.AI;
using System.ClientModel;

namespace BigChat.AppCore.ChatClients;

internal class UnconfiguredAIClient : IChatClient
{
    private LocalizedTexts Loc { get; } = ServiceLocator.GetRequiredService<LocalizedTexts>();

    public void Dispose()
    {
    }

    public Task<ChatResponse> GetResponseAsync(IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        throw new ClientResultException(Loc.UnconfiguredAIClientMessage);
    }

    public object? GetService(Type serviceType, object? serviceKey = null)
    {
        return null;
    }

    public IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        throw new ClientResultException(Loc.UnconfiguredAIClientMessage);
    }
}
