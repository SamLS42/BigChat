namespace BigChat.Messages;


internal sealed class CacheGetOrCreateException : Exception
{
    public CacheGetOrCreateException()
    {
    }

    public CacheGetOrCreateException(string? message) : base(message)
    {
    }

    public CacheGetOrCreateException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
