using Microsoft.Windows.AI.MachineLearning;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Windows.Foundation;

namespace BigChat.Onnx;

internal sealed partial class OnnxSetupService : IDisposable
{
    private BehaviorSubject<double> ProgressSource { get; } = new(0);
    public IObservable<double> Progress => ProgressSource.AsObservable();
    public bool Completed => ProgressSource.Value == 100;

    public async Task InitializeAsync()
    {
        ExecutionProviderCatalog catalog = ExecutionProviderCatalog.GetDefault();

        IAsyncOperationWithProgress<IList<ExecutionProvider>, double> operation = catalog.EnsureAndRegisterCertifiedAsync();

        operation.Progress = (_, progressValue) => ProgressSource.OnNext(progressValue);

        operation.Completed = (asyncInfo, status) =>
        {
            if (status == AsyncStatus.Error)
            {
                ProgressSource.OnError(asyncInfo.ErrorCode);
                return;
            }

            ProgressSource.OnCompleted();
        };

        await operation;
    }

    public void Dispose()
    {
        ProgressSource?.Dispose();
    }
}