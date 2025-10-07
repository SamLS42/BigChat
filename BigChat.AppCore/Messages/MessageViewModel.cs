using Microsoft.Extensions.AI;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace BigChat.AppCore.ViewModel;

public partial class MessageViewModel : ReactiveObject
{

    [Reactive]
    public int Id { get; set; }

    [Reactive]
    public int ConversationId { get; set; }

    [Reactive]
    public partial string Content { get; set; } = string.Empty;

    [Reactive]
    public ChatRole Role { get; set; }

    [Reactive]
    public DateTime CreatedAt { get; set; }

    [Reactive]
    public partial string ThinkContent { get; set; } = string.Empty;

    [Reactive]
    public partial bool IsPending { get; set; }

    [Reactive]
    public partial bool IsEditable { get; private set; }

    [Reactive]
    public partial string EditText { get; set; } = string.Empty;

    Subject<Unit> MessageUpdatesSource { get; } = new();
    public IObservable<Unit> MessageUpdated => MessageUpdatesSource.AsObservable();

    private readonly ObservableAsPropertyHelper<bool> hasThink;
    public bool HasThink => hasThink.Value;

    private readonly ObservableAsPropertyHelper<bool> isThinking;
    public bool IsThinking => isThinking.Value;

    private readonly ObservableAsPropertyHelper<string> displayContent;
    public string DisplayContent => displayContent.Value;

    public MessageViewModel()
    {
        hasThink = this.WhenAnyValue(x => x.ThinkContent)
            .Select(t => !string.IsNullOrWhiteSpace(t))
            .ToProperty(this, nameof(HasThink));

        isThinking = this.WhenAnyValue(x => x.HasThink, x => x.Content, x => x.IsPending,
            (HasThink, Content, IsPending) => (HasThink && string.IsNullOrEmpty(Content)) || IsPending)
            .ToProperty(this, nameof(IsThinking));

        displayContent = this.WhenAnyValue(x => x.IsThinking, x => x.Content,
            (HasThink, Content) => IsThinking ? "<Thinking...>" : Content)
            .ToProperty(this, nameof(DisplayContent));
    }

    [ReactiveCommand]
    private void EnableEdit()
    {
        EditText = Content;
        IsEditable = true;
    }

    [ReactiveCommand]
    private void ConfirmEdit()
    {
        if (string.IsNullOrWhiteSpace(EditText)) return;

        IsEditable = false;
        Content = EditText;

        MessageUpdatesSource.OnNext(Unit.Default);
    }

    [ReactiveCommand]
    private void CancelEdit()
    {
        IsEditable = false;
    }
}
