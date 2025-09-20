using BigChat.AppCore.ViewModel;
using Microsoft.Extensions.AI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace BigChat.Messages;

public partial class ChatTemplateSelector : DataTemplateSelector
{
    public DataTemplate UserTemplate { get; set; } = null!;

    public DataTemplate AssistantTemplate { get; set; } = null!;

    protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
    {
        if (item is MessageViewModel selectedObject && selectedObject.Role == ChatRole.User)
        {
            return UserTemplate;
        }

        return AssistantTemplate;
    }

}
