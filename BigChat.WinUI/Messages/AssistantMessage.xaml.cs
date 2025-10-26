using BigChat.AppCore;
using BigChat.AppCore.Localization;
using BigChat.AppCore.ViewModel;
using CommunityToolkit.WinUI;
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
    private Compositor Compositor { get; } = CompositionTarget.GetCompositorForCurrentThread();
    private LocalizedTexts Loc { get; } = ServiceLocator.GetRequiredService<LocalizedTexts>();
    private SpringScalarNaturalMotionAnimation? SpringAnimation { get; set; }
    private MarkdownConfig MarkdownConfig => MarkdownConfig.Default;
    public AssistantMessage()
    {
        InitializeComponent();

        this.FindChildren().OfType<MarkdownTextBlock>().ForEach(m => m.UpdateDebounceDelayMs = TimeSpan.FromMilliseconds(200));

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
                    ActionButtonsPanel.StartAnimation(SpringAnimation);
                })
                .DisposeWith(d);

            Observable.FromEventPattern<object, PointerRoutedEventArgs>(ActionsGrid, nameof(ActionsGrid.PointerExited))
                .Subscribe(_ =>
                {
                    CreateOrUpdateAppearingAnimation(0f);
                    ActionButtonsPanel.StartAnimation(SpringAnimation);
                })
                .DisposeWith(d);
        });
    }

    private void CreateOrUpdateAppearingAnimation(float finalValue)
    {
        if (SpringAnimation == null)
        {
            SpringAnimation = Compositor.CreateSpringScalarAnimation();
            SpringAnimation.Target = "Opacity";
        }

        SpringAnimation.FinalValue = finalValue;
    }
}
