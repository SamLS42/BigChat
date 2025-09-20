using BigChat.AppCore.ViewModel;
using Microsoft.UI.Composition;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using ReactiveUI;
using System.Reactive.Linq;
using Windows.ApplicationModel.DataTransfer;
using Windows.System;
using Windows.UI.Core;

namespace BigChat.Messages;

internal class ReactiveUserMessage : ReactiveUserControl<MessageViewModel>;
internal sealed partial class UserMessage : ReactiveUserMessage
{
    private readonly Compositor _compositor = Microsoft.UI.Xaml.Media.CompositionTarget.GetCompositorForCurrentThread();
    private SpringScalarNaturalMotionAnimation? _springAnimation;
    public UserMessage()
    {
        InitializeComponent();

        this.BindCommand(ViewModel, vm => vm.EnableEditCommand, v => v.EnableEditBtn);
        this.BindCommand(ViewModel, vm => vm.CancelEditCommand, v => v.CancelEditBtn);
        this.BindCommand(ViewModel, vm => vm.ConfirmEditCommand, v => v.ConfirmEditBtn);

        Observable.FromEventPattern<object, KeyRoutedEventArgs>(EditTextBox, nameof(EditTextBox.PreviewKeyDown))
            .Subscribe(ep =>
            {
                if (ep.EventArgs.Key == VirtualKey.Enter && !InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Shift).HasFlag(CoreVirtualKeyStates.Down))
                {
                    ViewModel!.ConfirmEditCommand.Execute().Subscribe();
                    ep.EventArgs.Handled = true;
                }
            });

        Observable.FromEventPattern<object, KeyRoutedEventArgs>(EditTextBox, nameof(EditTextBox.PreviewKeyDown))
            .Subscribe(ep =>
            {
                if (ep.EventArgs.Key == VirtualKey.Enter && !InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Shift).HasFlag(CoreVirtualKeyStates.Down))
                {
                    ViewModel!.ConfirmEditCommand.Execute().Subscribe();
                    ep.EventArgs.Handled = true;
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
