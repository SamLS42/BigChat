using BigChat.AppCore.Notifications;
using Microsoft.UI.Xaml.Controls;

namespace BigChat.Utils;

internal static class Extensions
{
    public static InfoBarSeverity ToInfoBarSeverity(this Severity severity)
    {
        return severity switch
        {
            Severity.Informational => InfoBarSeverity.Informational,
            Severity.Success => InfoBarSeverity.Success,
            Severity.Warning => InfoBarSeverity.Warning,
            Severity.Error => InfoBarSeverity.Error,
            _ => throw new NotSupportedException(nameof(ToInfoBarSeverity))
        };
    }
}