using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace BigChat.AppCore.Notifications;

public class NotificationService : IDisposable
{
    private Subject<NotificationMessage> NotificationSource { get; set; } = new();
    public IObservable<NotificationMessage> Notifications => NotificationSource.AsObservable();

    public void Send(Severity severity, string message)
    {
        ArgumentNullException.ThrowIfNull(message);
        NotificationSource.OnNext(new(message, severity));
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool dispose)
    {
        if (dispose)
        {
            NotificationSource.Dispose();
        }
    }
}
