using BigChat.AppCore.Messages;
using BigChat.AppCore.ViewModel;
using CommunityToolkit.Labs.WinUI.MarkdownTextBlock;
using Microsoft.UI.Composition;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Windows.ApplicationModel.DataTransfer;

namespace BigChat.Messages;

internal sealed partial class AssistantMessage : UserControl, IMessageControl
{
    private readonly Compositor _compositor = CompositionTarget.GetCompositorForCurrentThread();
    private SpringScalarNaturalMotionAnimation? _springAnimation;
    private MarkdownConfig MarkdownConfig { get; set; } = new();
    public AssistantMessage()
    {
        InitializeComponent();
    }

    private void UpdateMarkdown(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (string.Equals(e.PropertyName, nameof(Message.Text), StringComparison.Ordinal))
        {
            //needed to not block the UI, works better with low priority, I think
            AssistantResponse.DispatcherQueue.TryEnqueue(DispatcherQueuePriority.Low, () => AssistantResponse.Text = Message.Text ?? string.Empty);
        }
    }

    public MessageViewModel Message
    {
        get => (MessageViewModel)GetValue(MessageProperty);
        set
        {
            SetValue(MessageProperty, value);
            if (!string.Equals(AssistantResponse.Text, Message.Text, StringComparison.Ordinal))
            {
                AssistantResponse.Text = Message.Text;
            }
            Message.PropertyChanged += UpdateMarkdown;
        }
    }

    private static readonly DependencyProperty MessageProperty = DependencyProperty.Register(
        name: nameof(Message),
        propertyType: typeof(MessageViewModel),
        ownerType: typeof(AssistantMessage),
        typeMetadata: new PropertyMetadata(defaultValue: null));

    private void CreateOrUpdateAppearingAnimation(float finalValue)
    {
        if (_springAnimation == null)
        {
            _springAnimation = _compositor.CreateSpringScalarAnimation();
            _springAnimation.Target = "Opacity";
        }

        _springAnimation.FinalValue = finalValue;
    }

    private void Element_PointerEntered(object sender, PointerRoutedEventArgs e)
    {
        CreateOrUpdateAppearingAnimation(1f);

        ActionButtonsPanel.StartAnimation(_springAnimation);
    }

    private void Element_PointerExited(object sender, PointerRoutedEventArgs e)
    {
        CreateOrUpdateAppearingAnimation(0f);

        ActionButtonsPanel.StartAnimation(_springAnimation);
    }

    private void CopyButton_Click(object sender, RoutedEventArgs e)
    {
        DataPackage dataPackage = new()
        {
            RequestedOperation = DataPackageOperation.Move,
        };
        dataPackage.SetText(Message.Text);
        Clipboard.SetContent(dataPackage);
    }
}
