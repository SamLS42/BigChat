using BigChat.AppCore.ViewModel;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace BigChat.Conversations;

internal sealed partial class ConversationPage : Page
{
    private ConversationPageViewModel ViewModel { get; init; } = App.ServiceProvider.GetRequiredService<ConversationPageViewModel>();

    public ConversationPage()
    {
        InitializeComponent();
        MessageListView.Loaded += MessageListView_Loaded;
        WeakReferenceMessenger.Default.Send<ConversationPageChanged>(new(ViewModel));
    }

    private void MessageListView_Loaded(object sender, RoutedEventArgs e)
    {
        ViewModel.LoadHistoryCommand.Execute(parameter: null);
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        ArgumentNullException.ThrowIfNull(e.Parameter, "conversation parameter");

        if (e.Parameter is ConversationViewModel conversation)
        {
            ViewModel.Conversation = conversation;
        }
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        base.OnNavigatedFrom(e);
        MessageListView.Loaded -= MessageListView_Loaded;
        ViewModel.Dispose();
    }
}
