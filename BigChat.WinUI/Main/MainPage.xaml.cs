using BigChat.AppCore;
using BigChat.AppCore.Conversations;
using BigChat.AppCore.Conversations.EventMessages;
using BigChat.AppCore.Localization;
using BigChat.AppCore.MainPage;
using BigChat.Localization;
using CommunityToolkit.Mvvm.Messaging;
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

internal sealed partial class MainPage : Page,
    IRecipient<RenameConversationConfirmation>,
    IRecipient<DeleteConversationConfirmation>,
    IDisposable
{
    private MainPageViewModel ViewModel { get; } = ServiceLocator.GetRequiredService<MainPageViewModel>();
    private CompositeDisposable CleanUp { get; init; } = [];
    private LocalizedTexts Loc { get; } = ServiceLocator.GetRequiredService<ILocalizedTexts>().As<LocalizedTexts>();
    private DialogService DialogService { get; } = ServiceLocator.GetRequiredService<DialogService>();

    private readonly ReadOnlyObservableCollection<ConversationViewModel> Conversations = null!;

    public MainPage()
    {
        InitializeComponent();

        //NavigationService navigationService = ServiceLocator.GetRequiredService<INavigationService>().As<NavigationService>();

        //navigationService.Setup(NavViewFrame, NavView, EmptyConversation, () => ChatNavigationViewItems);

        ViewModel.GoBackCommand.CanExecute.Subscribe(v => TitleBar.IsBackButtonEnabled = v)
            .DisposeWith(CleanUp);

        ViewModel.LoadConversationsCommand.Execute().Subscribe();

        ViewModel.Conversations
            .Connect()
            .Bind(out Conversations)
            .Subscribe()
            .DisposeWith(CleanUp);

        WeakReferenceMessenger.Default.Register<RenameConversationConfirmation>(this);
        WeakReferenceMessenger.Default.Register<DeleteConversationConfirmation>(this);
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        ViewModel.OpenEmptyConversationCommand.Execute().Subscribe();
    }

    private void TitleBar_PaneToggleRequested(TitleBar sender, object args)
    {
        NavView.IsPaneOpen = !NavView.IsPaneOpen;
    }

    private void TitleBar_BackRequested(TitleBar sender, object args)
    {
        ViewModel.GoBackCommand.Execute().Subscribe();
    }

    private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
        {
            ViewModel.FilterConversationsCommand.Execute().Subscribe();
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
