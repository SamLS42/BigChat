using BigChat.AppCore.Messages;
using BigChat.AppCore.ViewModel;
using Microsoft.Extensions.Caching.Memory;

namespace BigChat.Messages;
internal sealed class MessageControlProvider(IMemoryCache cache, IMessageControlSelector selector) : IMessageControlProvider
{

    private readonly MemoryCacheEntryOptions cacheOptions = new() { Size = 1 };

    public IMessageControl GetOrCreate(MessageViewModel message)
    {
        return cache.GetOrCreate(message.Id, (entry) =>
        {
            entry.SetOptions(cacheOptions);

            return selector.GetControl(message);

        }) ?? throw new CacheGetOrCreateException($"Couldn't get or create the IMessage control for {message}");
    }

    public void CreateEntry(IMessageControl messageControl)
    {
        using ICacheEntry entry = cache.CreateEntry(messageControl.Message.Id);
        entry.SetOptions(cacheOptions);
        entry.Value = messageControl;
    }
}
