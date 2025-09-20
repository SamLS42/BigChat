using BigChat.AppCore.ViewModel;
using CommunityToolkit.WinUI.Controls;
using Microsoft.UI.Composition;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using ReactiveUI;
using System.Reactive.Linq;
using Windows.ApplicationModel.DataTransfer;

namespace BigChat.Messages;

internal class ReactiveAssistantMessage : ReactiveUserControl<MessageViewModel>;
internal sealed partial class AssistantMessage : ReactiveAssistantMessage
{
    private readonly Compositor _compositor = CompositionTarget.GetCompositorForCurrentThread();
    private SpringScalarNaturalMotionAnimation? _springAnimation;
    private MarkdownConfig MarkdownConfig { get; set; } = new();
    public AssistantMessage()
    {
        InitializeComponent();

        this.WhenAnyValue(x => x.ViewModel!.Text)
            .WhereNotNull()
            .Subscribe(text => AssistantResponse.Text = text);

        this.WhenAnyValue(x => x.ViewModel!.Text)
            .Select(string.IsNullOrWhiteSpace)
            .Subscribe(messageIsEmpty =>
            {
                if (messageIsEmpty)
                {
                    ProgressRing.Visibility = Visibility.Visible;
                    AssistantResponse.Visibility = Visibility.Collapsed;
                }
                else
                {
                    ProgressRing.Visibility = Visibility.Collapsed;
                    AssistantResponse.Visibility = Visibility.Visible;
                }
            });

        Observable.FromEventPattern<object, RoutedEventArgs>(CopyBtn, nameof(CopyBtn.Click))
            .Subscribe(_ =>
            {
                DataPackage dataPackage = new()
                {
                    RequestedOperation = DataPackageOperation.Move,
                };
                dataPackage.SetText(ViewModel?.Text);
                Clipboard.SetContent(dataPackage);
            });

        Observable.FromEventPattern<object, PointerRoutedEventArgs>(ActionsGrid, nameof(ActionsGrid.PointerEntered))
            .Subscribe(_ =>
            {
                CreateOrUpdateAppearingAnimation(1f);
                ActionButtonsPanel.StartAnimation(_springAnimation);
            });

        Observable.FromEventPattern<object, PointerRoutedEventArgs>(ActionsGrid, nameof(ActionsGrid.PointerExited))
            .Subscribe(_ =>
            {
                CreateOrUpdateAppearingAnimation(0f);
                ActionButtonsPanel.StartAnimation(_springAnimation);
            });
    }

    private void UpdateMarkdown(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (string.Equals(e.PropertyName, nameof(ViewModel.Text), StringComparison.Ordinal))
        {
            //needed to not block the UI, works better with low priority, I think
            AssistantResponse.DispatcherQueue.TryEnqueue(DispatcherQueuePriority.High, () => AssistantResponse.Text = ViewModel?.Text ?? string.Empty);
        }
    }

    private void CreateOrUpdateAppearingAnimation(float finalValue)
    {
        if (_springAnimation == null)
        {
            _springAnimation = _compositor.CreateSpringScalarAnimation();
            _springAnimation.Target = "Opacity";
        }

        _springAnimation.FinalValue = finalValue;
    }
}
