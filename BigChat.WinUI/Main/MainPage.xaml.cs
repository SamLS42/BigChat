using BigChat.AppCore;
using BigChat.AppCore.Conversations;
using BigChat.AppCore.Conversations.EventMessages;
using BigChat.AppCore.Localization;
using BigChat.AppCore.MainPage;
using BigChat.Conversations;
using BigChat.Localization;
using BigChat.Settings;
using DynamicData;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using WinRT;

namespace BigChat.Main;

internal class ReactiveMainPageView : ReactivePage<MainPageViewModel>;
internal sealed partial class MainPage : ReactiveMainPageView, IDisposable
{
    private ConversationPage? CurrentConversationPage { get; set; }
    private CompositeDisposable CleanUp { get; init; } = [];
    private LocalizedTexts Loc { get; } = ServiceLocator.GetRequiredService<ILocalizedTexts>().As<LocalizedTexts>();
    private DialogService DialogService { get; } = ServiceLocator.GetRequiredService<DialogService>();

    private IDisposable? ConversationAddedSubscription { get; set; }

    private readonly ReadOnlyObservableCollection<ConversationViewModel> Conversations = null!;
    public MainPage()
    {
        InitializeComponent();

        ViewModel = ServiceLocator.GetRequiredService<MainPageViewModel>();

        Observable.FromEventPattern<NavigationView, NavigationViewItemInvokedEventArgs>(NavView, nameof(NavView.ItemInvoked))
            .Subscribe(ep =>
            {
                if (ep.EventArgs.IsSettingsInvoked)
                {
                    if (NavViewFrame.Content is SettingsPage)
                    {
                        return;
                    }

                    NavViewFrame.Navigate(typeof(SettingsPage));
                }
                else if (ep.EventArgs.InvokedItem is ConversationViewModel conversation)
                {
                    if (NavViewFrame.Content is ConversationPage page && page.ViewModel?.Id == conversation.Id)
                    {
                        return;
                    }

                    NavViewFrame.Navigate(typeof(ConversationPage), conversation);
                }
            });

        Observable.FromEventPattern<object, NavigationEventArgs>(NavViewFrame, nameof(NavViewFrame.Navigated))
            .Subscribe(ep =>
            {
                ConversationAddedSubscription?.Dispose();

                if (ep.EventArgs.SourcePageType == typeof(SettingsPage))
                {
                    CurrentConversationPage = null;
                    NavView.SelectedItem = NavView.SettingsItem;
                }
                else if (ep.EventArgs.SourcePageType == typeof(ConversationPage))
                {
                    CurrentConversationPage = ep.EventArgs.Content.As<ConversationPage>();
                    NavView.SelectedItem = ep.EventArgs.Parameter;

                    if (ep.EventArgs.Parameter is ConversationViewModel vm && vm.Id == 0)
                    {
                        ConversationAddedSubscription = vm.WhenAnyValue(x => x.Id)
                            .Where(v => v != 0)
                            .Select(_ => vm)
                            .Subscribe(vm =>
                            {
                                ViewModel.AddConversation(vm);
                                NavViewFrame.Navigate(typeof(ConversationPage), vm);
                            });
                    }
                }
            });


        ViewModel.LoadConversationsCommand.Execute().Subscribe();
        OpenEmptyConversation();

        ViewModel.Conversations
            .Connect()
            .Bind(out Conversations)
            .Subscribe()
            .DisposeWith(CleanUp);

        ViewModel.Conversations
            .Connect()
            .OnItemRemoved(vm => NavViewFrame.BackStack.RemoveMany(NavViewFrame.BackStack.Where(p => ReferenceEquals(p.Parameter, vm))))
            .Subscribe()
            .DisposeWith(CleanUp);

        UserInput.UserInputs.Subscribe(input => CurrentConversationPage?.ViewModel?.AddMessageCommand.Execute(input).Subscribe())
            .DisposeWith(CleanUp);
    }

    private void OpenEmptyConversation()
    {
        NavViewFrame.Navigate(typeof(ConversationPage), ViewModel!.GetEmptyConversation());
    }

    private void TitleBar_PaneToggleRequested(TitleBar sender, object args)
    {
        NavView.IsPaneOpen = !NavView.IsPaneOpen;
    }

    private void TitleBar_BackRequested(TitleBar sender, object args)
    {
        NavViewFrame.GoBack();
    }

    private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
        {
            ViewModel!.FilterConversationsCommand.Execute().Subscribe();
        }
    }

    private void AutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
    {
        ViewModel!.UpdateAutoSuggestBoxTextCommand.Execute(parameter: args.SelectedItem);
    }

    private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        ViewModel!.SelectSuggestedConversationCommand.Execute(parameter: args.ChosenSuggestion);
    }

    public void Receive(RenameConversationConfirmation message)
    {
        ContentDialog dialog = DialogService.GetConfirmationDialog(xamlRoot: XamlRoot,
            title: "Rename Subject",
            primaryButtonText: "Save",
            content: new TextBox()
            {
                Text = message.Conversation.Subject,
                SelectionStart = 0,
                SelectionLength = message.Conversation.Subject.Length,
            });

        dialog.ShowAsync().Completed += (info, _) =>
        {
            ContentDialogResult result = info.GetResults();

            if (result.HasFlag(ContentDialogResult.Primary))
            {
                message.Conversation.Subject = dialog.Content.As<TextBox>().Text;
                ViewModel!.UpdateConversationSubjectCommand.Execute(message.Conversation);
            }
        };
    }

    public void Receive(DeleteConversationConfirmation message)
    {
        ContentDialog dialog = DialogService.GetConfirmationDialog(xamlRoot: XamlRoot,
            title: "Delete Conversation",
            primaryButtonText: "Delete",
            content: new TextBlock()
            {
                Text = "This action can't be undone",
                Style = Application.Current.Resources["CaptionTextBlockStyle"] as Style,
            });

        dialog.ShowAsync().Completed += (info, _) =>
        {
            ContentDialogResult result = info.GetResults();

            if (result.HasFlag(ContentDialogResult.Primary))
            {
                ViewModel!.DeleteConversationCommand.Execute(message.Conversation);
            }
        };
    }

    public void Dispose()
    {
        CleanUp.Dispose();
    }

    private void PageButton_Click(object sender, RoutedEventArgs e)
    {
        OpenEmptyConversation();
    }
}
