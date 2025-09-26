using BigChat.AppCore;
using BigChat.AppCore.Localization;
using BigChat.AppCore.MainPage;
using BigChat.Localization;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using ReactiveUI;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Windows.System;
using Windows.UI.Core;
using WinRT;

namespace BigChat.Main;

internal class ReactiveUserInput : ReactiveUserControl<UserInputViewModel>;
internal sealed partial class UserInput : ReactiveUserInput
{
    private LocalizedTexts Loc { get; } = ServiceLocator.GetRequiredService<ILocalizedTexts>().As<LocalizedTexts>();

    public UserInput()
    {
        InitializeComponent();

        ViewModel ??= new();

        this.WhenActivated(d =>
        {
            this.BindCommand(ViewModel, vm => vm.AddMessageCommand, v => v.SendBtn).DisposeWith(d);
            this.BindCommand(ViewModel, vm => vm.StopResponseCommand, v => v.StopBtn).DisposeWith(d);

            this.Bind(ViewModel, vm => vm.InputBoxText, v => v.InputBox.Text).DisposeWith(d);

            this.OneWayBind(ViewModel, vm => vm.AiIsResponding, v => v.SendBtn.Visibility, v => v ? Visibility.Collapsed : Visibility.Visible).DisposeWith(d);
            this.OneWayBind(ViewModel, vm => vm.AiIsResponding, v => v.StopBtn.Visibility, v => v ? Visibility.Visible : Visibility.Collapsed).DisposeWith(d);
        });
    }

    private void InputBox_PreviewKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == VirtualKey.Enter && !InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Shift).HasFlag(CoreVirtualKeyStates.Down))
        {
            ViewModel!.AddMessageCommand.Execute().Subscribe();
            e.Handled = true;
        }
    }

    public void Focus()
    {
        if (InputBox.IsLoaded)
        {
            DispatcherQueue.TryEnqueue(() => InputBox.Focus(FocusState.Programmatic));
            return;
        }

        Observable.FromEventPattern(InputBox, nameof(InputBox.Loaded))
            .Subscribe(_ => DispatcherQueue.TryEnqueue(() => InputBox.Focus(FocusState.Programmatic)));
    }
}
