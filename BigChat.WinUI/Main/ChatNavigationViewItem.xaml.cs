using BigChat.AppCore;
using BigChat.AppCore.Conversations;
using BigChat.AppCore.Localization;
using BigChat.Localization;
using Microsoft.UI.Xaml;
using WinRT;

namespace BigChat.Main;

internal sealed partial class ChatNavigationViewItem
{
    private LocalizedTexts Loc { get; } = ServiceLocator.GetRequiredService<ILocalizedTexts>().As<LocalizedTexts>();

    public ChatNavigationViewItem()
    {
        InitializeComponent();
    }

    public ConversationViewModel Conversation
    {
        get => (ConversationViewModel)GetValue(ConversationProperty);
        set => SetValue(ConversationProperty, value);
    }

    public static readonly DependencyProperty ConversationProperty = DependencyProperty.Register(
        name: nameof(Conversation),
        propertyType: typeof(ConversationViewModel),
        ownerType: typeof(ChatNavigationViewItem),
        typeMetadata: new PropertyMetadata(defaultValue: null));

}
