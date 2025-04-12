using BigChat.AppCore.Conversations.EventMessages;
using BigChat.AppCore.Localization;
using BigChat.AppCore.Navigation;
using BigChat.AppCore.ViewModel;
using BigChat.Localization;
using CommunityToolkit.Mvvm.Messaging;
using DynamicData;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using WinRT;

namespace BigChat.Main;

internal sealed partial class MainPage : Page,
    IRecipient<RenameConversationConfirmation>,
    IRecipient<DeleteConversationConfirmation>,
    IDisposable
{
    private MainPageViewModel ViewModel { get; init; }
    private IDisposable CleanUp { get; init; }
    private bool IsInitialized { get; set; }
    private LocalizedTexts Loc { get; }
    private DialogService DialogService { get; }

    private readonly ReadOnlyObservableCollection<ChatNavigationViewItem> ChatNavigationViewItems = null!;

    public MainPage()
    {
        InitializeComponent();
        Loc = App.ServiceProvider.GetRequiredService<ILocalizedTexts>().As<LocalizedTexts>();
        DialogService = App.ServiceProvider.GetRequiredService<DialogService>();

        NavigationService navigationService = App.ServiceProvider.GetRequiredService<INavigationService>().As<NavigationService>();

        navigationService.Setup(NavViewFrame, NavView, EmptyConversation, () => ChatNavigationViewItems);

        ViewModel = App.ServiceProvider.GetRequiredService<MainPageViewModel>();

        ViewModel.LoadConversationsCommand.Execute(parameter: null);

        CleanUp = ViewModel.Conversations
            .Connect()
            .Transform(conversation => new ChatNavigationViewItem { Conversation = conversation })
            .DisposeMany()
            .Bind(out ChatNavigationViewItems)
            .OnItemAdded(item =>
            {
                if (IsInitialized)
                {
                    ViewModel.SelectConversationCommand.Execute((item.Conversation, navigate: true));
                }
            })
            .Subscribe();

        IsInitialized = true;

        WeakReferenceMessenger.Default.Register<RenameConversationConfirmation>(this);
        WeakReferenceMessenger.Default.Register<DeleteConversationConfirmation>(this);
    }

    private void NavViewFrame_Navigated(object sender, NavigationEventArgs e)
    {
        ViewModel.UpdateBackStackOnNavigationCommand.Execute(parameter: null);
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        ViewModel.OpenEmptyConversationCommand.Execute(parameter: null);
    }

    private void NavigationView_SelectionChanged(NavigationView _, NavigationViewSelectionChangedEventArgs args)
    {
        ViewModel.NavigateOnSelectionCommand.Execute(parameter: args.SelectedItem);
    }

    private void TitleBar_PaneToggleRequested(TitleBar sender, object args)
    {
        NavView.IsPaneOpen = !NavView.IsPaneOpen;
    }

    private void TitleBar_BackRequested(TitleBar sender, object args)
    {
        ViewModel.GoBackCommand.Execute(parameter: null);
    }

    private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
        {
            ViewModel.FilterConversationsCommand.Execute(parameter: null);
        }
    }

    private void AutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
    {
        ViewModel.UpdateAutoSuggestBoxTextCommand.Execute(parameter: args.SelectedItem);
    }

    private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        ViewModel.SelectSuggestedConversationCommand.Execute(parameter: args.ChosenSuggestion);
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
                ViewModel.UpdateConversationSubjectCommand.Execute(message.Conversation);
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
                ViewModel.DeleteConversationCommand.Execute(message.Conversation);
            }
        };
    }

    public void Dispose()
    {
        WeakReferenceMessenger.Default.UnregisterAll(this);
        CleanUp.Dispose();
    }
}
