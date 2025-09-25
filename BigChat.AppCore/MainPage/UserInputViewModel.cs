using ReactiveUI;
using ReactiveUI.SourceGenerators;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace BigChat.AppCore.MainPage;

public partial class UserInputViewModel : ReactiveObject
{
    [Reactive]
    public partial bool AiIsResponding { get; set; }
    [Reactive]
    public string InputBoxText { get; set; } = string.Empty;
    private Subject<string> UserInputSource { get; } = new();
    public IObservable<string> UserInputs => UserInputSource.Where(s => !string.IsNullOrWhiteSpace(s)).AsObservable();

    [ReactiveCommand]
    private void AddMessage()
    {
        UserInputSource.OnNext(InputBoxText);

        InputBoxText = string.Empty;
    }

    [ReactiveCommand]
    private void StopResponse() { }
}
