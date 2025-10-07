using BigChat.AppCore.ViewModel;
using CommunityToolkit.WinUI.Controls;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using ReactiveUI;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using Windows.ApplicationModel.DataTransfer;

namespace BigChat.Messages;

internal partial class ReactiveAssistantMessage : ReactiveUserControl<MessageViewModel>;
internal sealed partial class AssistantMessage : ReactiveAssistantMessage
{
    private readonly Compositor _compositor = CompositionTarget.GetCompositorForCurrentThread();
    private SpringScalarNaturalMotionAnimation? _springAnimation;
    private MarkdownConfig MarkdownConfig => MarkdownConfig.Default;
    public AssistantMessage()
    {
        InitializeComponent();

        this.WhenActivated(d =>
        {
            Observable.FromEventPattern<object, RoutedEventArgs>(CopyBtn, nameof(CopyBtn.Click))
                .Subscribe(_ =>
                {
                    DataPackage dataPackage = new()
                    {
                        RequestedOperation = DataPackageOperation.Move,
                    };
                    dataPackage.SetText(ViewModel?.DisplayContent);
                    Clipboard.SetContent(dataPackage);
                })
                .DisposeWith(d);

            Observable.FromEventPattern<object, PointerRoutedEventArgs>(ActionsGrid, nameof(ActionsGrid.PointerEntered))
                .Subscribe(_ =>
                {
                    CreateOrUpdateAppearingAnimation(1f);
                    ActionButtonsPanel.StartAnimation(_springAnimation);
                })
                .DisposeWith(d);

            Observable.FromEventPattern<object, PointerRoutedEventArgs>(ActionsGrid, nameof(ActionsGrid.PointerExited))
                .Subscribe(_ =>
                {
                    CreateOrUpdateAppearingAnimation(0f);
                    ActionButtonsPanel.StartAnimation(_springAnimation);
                })
                .DisposeWith(d);
        });
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
