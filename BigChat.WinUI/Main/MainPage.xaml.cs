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
using Microsoft.UI.Dispatching;
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
                else if (ep.EventArgs.InvokedItem is ChatNavigationViewItem item)
                {
                    OpenConversation(item.Conversation);
                }
                else
                {
                    OpenEmptyConversation();
                }
            });

        Observable.FromEventPattern<object, NavigationEventArgs>(NavViewFrame, nameof(NavViewFrame.Navigated))
            .Subscribe(ep =>
            {
                SelectItem(null);
                CurrentConversationPage = null;

                if (ep.EventArgs.SourcePageType == typeof(SettingsPage))
                {
                    SelectItem(NavView.SettingsItem);
                }
                else if (ep.EventArgs.SourcePageType == typeof(ConversationPage))
                {
                    CurrentConversationPage = ep.EventArgs.Content.As<ConversationPage>();
                    SelectItem(ep.EventArgs.Parameter);
                }
            });


        ViewModel.LoadConversationsCommand.Execute().Subscribe();
        OpenEmptyConversation();

        ViewModel.Conversations
            .Connect()
            .SortBy(x => x.Id)
            .Bind(out Conversations)
            .OnItemRemoved(vm =>
            {
                if (ReferenceEquals(CurrentConversationPage?.ViewModel, vm))
                {
                    OpenEmptyConversation();
                }

                PageStackEntry[] ToDelete = [.. NavViewFrame.BackStack.Where(p => p.Parameter is ConversationViewModel c && c.Id == vm.Id)];

                NavViewFrame.BackStack.RemoveMany(ToDelete);

                PageStackEntry[] distintValues = [.. NavViewFrame.BackStack.DistinctUntilChanged(keySelector: p => p.Parameter)];

                NavViewFrame.BackStack.Clear();

                if (distintValues.Length > 1)
                {
                    NavViewFrame.BackStack.AddRange(distintValues);
                }
            })
            .Subscribe()
            .DisposeWith(CleanUp);

        ViewModel.Conversations
            .Connect()
            .Subscribe()
            .DisposeWith(CleanUp);

        UserInput.UserInputs.Subscribe(async input =>
        {
            ConversationViewModel vm = CurrentConversationPage?.ViewModel ?? await ViewModel.GetNewConversationAsync();

            OpenConversation(vm);

            vm.AddMessageCommand.Execute(input).Subscribe();
        }).DisposeWith(CleanUp);
    }

    private void SelectItem(object? obj)
    {
        DispatcherQueue.GetForCurrentThread().TryEnqueue(() =>
        {
            NavView.SelectedItem = null;
            NavView.UpdateLayout();
            NavView.SelectedItem = obj;
        });
    }

    private void OpenConversation(ConversationViewModel conversation)
    {
        if (NavViewFrame.Content is ConversationPage page && page.ViewModel?.Id == conversation.Id)
        {
            return;
        }

        NavViewFrame.Navigate(typeof(ConversationPage), conversation);
    }

    private void OpenEmptyConversation()
    {
        if (NavViewFrame.Content is Empty)
        {
            return;
        }
        NavViewFrame.Navigate(typeof(Empty));
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
