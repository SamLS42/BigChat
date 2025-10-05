using Microsoft.Extensions.AI;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text.RegularExpressions;

namespace BigChat.AppCore.ViewModel;

public partial class MessageViewModel : ReactiveObject
{
    [Reactive]
    public int Id { get; set; }
    [Reactive]
    public int ConversationId { get; set; }

    Subject<Unit> MessageUpdatesSource { get; } = new();
    public IObservable<Unit> MessageUpdated => MessageUpdatesSource.AsObservable();

    [Reactive]
    public partial string Text { get; set; } = string.Empty;
    [Reactive]
    public ChatRole Role { get; set; }
    [Reactive]
    public DateTime CreatedAt { get; set; }

    [Reactive]
    public partial string ReasoningText { get; private set; } = string.Empty;

    [Reactive]
    public partial string ResponseText { get; private set; } = string.Empty;

    [Reactive]
    public partial bool IsEditable { get; private set; }

    [Reactive]
    public partial string EditText { get; set; } = string.Empty;

    [ReactiveCommand]
    private void EnableEdit()
    {
        EditText = Text ?? string.Empty;
        IsEditable = true;
    }

    [ReactiveCommand]
    private void ConfirmEdit()
    {
        if (string.IsNullOrWhiteSpace(EditText)) return;

        IsEditable = false;
        Text = EditText;

        MessageUpdatesSource.OnNext(Unit.Default);
    }

    [ReactiveCommand]
    private void CancelEdit()
    {
        IsEditable = false;
    }

    private void OrganizeText()
    {
        try
        {
            Match match = ThinkDetector().Match(Text ?? string.Empty);
            if (match.Success)
            {
                ReasoningText = match.Groups["reasoning"].Value.Trim();
                ResponseText = match.Groups["response"].Value.Trim();
            }
            else
            {
                ResponseText = Text ?? string.Empty;
            }
        }
        catch (RegexMatchTimeoutException)
        {
            ResponseText = Text ?? string.Empty;
        }
    }

    [GeneratedRegex(@"<think>(?<reasoning>.*?)</think>(?<response>.*)", RegexOptions.ExplicitCapture | RegexOptions.Singleline, 500)]
    private static partial Regex ThinkDetector();
}
