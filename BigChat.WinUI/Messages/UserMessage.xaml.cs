using BigChat.AppCore.Messages;
using BigChat.AppCore.ViewModel;
using Microsoft.UI.Composition;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Windows.ApplicationModel.DataTransfer;
using Windows.System;
using Windows.UI.Core;

namespace BigChat.Messages;

internal sealed partial class UserMessage : UserControl, IMessageControl
{
    private readonly Compositor _compositor = Microsoft.UI.Xaml.Media.CompositionTarget.GetCompositorForCurrentThread();
    private SpringScalarNaturalMotionAnimation? _springAnimation;
    public UserMessage()
    {
        InitializeComponent();
    }

    public MessageViewModel Message
    {
        get => (MessageViewModel)GetValue(MessageProperty);
        set { SetValue(MessageProperty, value); Bindings.Update(); }
    }

    private static readonly DependencyProperty MessageProperty = DependencyProperty.Register(
        name: nameof(Message),
        propertyType: typeof(MessageViewModel),
        ownerType: typeof(UserMessage),
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

    private void InputBox_PreviewKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == VirtualKey.Enter && !InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Shift).HasFlag(CoreVirtualKeyStates.Down))
        {
            if (Message.ConfirmEditCommand.CanExecute(parameter: null))
            {
                Message.ConfirmEditCommand.Execute(parameter: null);
                e.Handled = true;
            }
        }
    }
}
