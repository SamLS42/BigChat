using BigChat.AppCore.Conversations.EventMessages;
using BigChat.AppCore.Navigation;
using BigChat.Infrastructure.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DynamicData;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace BigChat.AppCore.ViewModel;

public sealed partial class MainPageViewModel : ObservableObject,
    IRecipient<ConversationAdded>,
    IDisposable
{
    private MyDbContext Db { get; init; }
    public SourceList<ConversationViewModel> Conversations { get; init; } = new();
    private INavigationService NavigationService { get; set; }

    [ObservableProperty]
    public partial bool IsShowingConversation { get; set; }

    [ObservableProperty]
    public partial string AutoSuggestBoxText { get; set; } = string.Empty;

    [ObservableProperty]
    public partial ReadOnlyCollection<ConversationViewModel> FilteredConversations { get; set; } = ReadOnlyCollection<ConversationViewModel>.Empty;

    public MainPageViewModel(MyDbContext db, INavigationService navigationService)
    {
        Db = db;
        NavigationService = navigationService;
        WeakReferenceMessenger.Default.Register(this);
    }

    [RelayCommand]
    private void OpenNewConversation()
    {
        NavigationService.OpenEmptyConversation();
    }

    [RelayCommand]
    private async Task LoadConversations(CancellationToken cancellationToken = default)
    {
        await foreach (var conversation in Db.Conversations.Select(c => new { c.Id, c.Subject })
            .AsAsyncEnumerable()
            .WithCancellation(cancellationToken))
        {
            Conversations.Add(new ConversationViewModel
            {
                Id = conversation.Id,
                Subject = conversation.Subject,
            });
        }
    }

    public void Receive(ConversationAdded message)
    {
        ArgumentNullException.ThrowIfNull(message);
        Conversations.Add(message.NewConversation);
    }

    public void Dispose()
    {
        WeakReferenceMessenger.Default.UnregisterAll(this);
        Conversations.Clear();
        Conversations.Dispose();
    }

    [RelayCommand]
    private async Task UpdateConversationSubjectAsync(ConversationViewModel conversation, CancellationToken cancellationToken = default)
    {
        await Db.Conversations.Where(c => c.Id == conversation.Id)
            .ExecuteUpdateAsync(s => s.SetProperty(c => c.Subject, conversation.Subject), cancellationToken: cancellationToken);
    }

    [RelayCommand]
    private async Task DeleteConversationAsync(ConversationViewModel conversation, CancellationToken cancellationToken = default)
    {
        NavigationService.RemoveFromBackStack(conversation);

        NavigationService.LeaveConversation(conversation);

        Conversations.Remove(conversation);
        await Db.Conversations.Where(c => c.Id == conversation.Id)
            .ExecuteDeleteAsync(cancellationToken: cancellationToken);
    }

    [RelayCommand]
    private void UpdateBackStackOnNavigation()
    {
        NavigationService.UpdateBackStackOnNavigation();
    }

    [RelayCommand]
    private void NavigateOnSelection(object selectedItem)
    {
        NavigationService.NavigateOnSelection(selectedItem);
        IsShowingConversation = NavigationService.SelectedConversation is not null;
    }

    [RelayCommand]
    private void GoBack()
    {
        NavigationService.GoBack();
    }

    [RelayCommand]
    private void OpenEmptyConversation()
    {
        NavigationService.OpenEmptyConversation();
    }

    [RelayCommand]
    private void SelectConversation((ConversationViewModel conversation, bool navigate) parameter)
    {
        NavigationService.SelectConversation(parameter.conversation, parameter.navigate);
    }

    [RelayCommand]
    private void SelectSuggestedConversation(object chosenSuggestion)
    {
        if (chosenSuggestion is ConversationViewModel conversation)
        {
            SelectConversation((conversation, navigate: true));
            AutoSuggestBoxText = string.Empty;
            FilteredConversations = ReadOnlyCollection<ConversationViewModel>.Empty;
        }
    }

    [RelayCommand]
    private void UpdateAutoSuggestBoxText(object SelectedItem)
    {
        AutoSuggestBoxText = SelectedItem is ConversationViewModel conversation
            ? conversation.Subject
            : string.Empty;
    }

    [RelayCommand]
    private void FilterConversations()
    {
        FilteredConversations = new([.. Conversations.Items.Where(c => c.Subject.Contains(AutoSuggestBoxText, StringComparison.OrdinalIgnoreCase))]);
    }
}
