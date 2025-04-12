namespace BigChat.Infrastructure.Conversations;

public class ResponseMessage
{
    public required string Text
    {
        get; set
        {
            field = value;
            TextUpdated?.Invoke(this, EventArgs.Empty);
        }
    }

    public bool Completed
    {
        get; set
        {
            field = value;
            if (field)
            {
                StreamingCompleted?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public int Id { get; set; }

    public event EventHandler? TextUpdated;
    public event EventHandler? StreamingCompleted;
}
