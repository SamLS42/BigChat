using BigChat.Infrastructure.Data.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Text.RegularExpressions;

namespace BigChat.AppCore.ViewModel;

public partial class MessageViewModel(Message message) : ObservableObject
{
    public int Id { get => message.Id; set => message.Id = value; }
    public int ConversationId { get => message.ConversationId; set => message.ConversationId = value; }

    public bool TextIsEmpty => string.IsNullOrWhiteSpace(Text);

    public event EventHandler? EditConfirmed;

    [ObservableProperty]
    public partial string ReasoningText { get; private set; } = string.Empty;

    [ObservableProperty]
    public partial string ResponseText { get; private set; } = string.Empty;

    [ObservableProperty]
    public partial bool IsEditable { get; private set; }

    [ObservableProperty]
    public partial string EditText { get; set; } = string.Empty;

    public string Text
    {
        get => message.Text;
        set
        {
            if (!string.Equals(message.Text, value, StringComparison.Ordinal) && value is not null)
            {
                message.Text = value;

                OnPropertyChanged(nameof(Text));
                OnPropertyChanged(nameof(TextIsEmpty));
            }
        }
    }

    public string Role
    {
        get => message.Role;
        set => SetProperty(message.Role, value, message, (u, n) => u.Role = n);
    }

    [RelayCommand]
    private void EnableEdit()
    {
        EditText = Text ?? string.Empty;
        IsEditable = true;
    }

    [RelayCommand]
    private void ConfirmEdit()
    {
        IsEditable = false;
        Text = EditText;
        EditConfirmed?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private void CancelEdit()
    {
        IsEditable = false;
    }

    private void OrganizeText()
    {
        try
        {
            Match match = ThinkDetector().Match(Text ?? string.Empty);
            if (match.Success)
            {
                ReasoningText = match.Groups["reasoning"].Value.Trim();
                ResponseText = match.Groups["response"].Value.Trim();
            }
            else
            {
                ResponseText = Text ?? string.Empty;
            }
        }
        catch (RegexMatchTimeoutException)
        {
            ResponseText = Text ?? string.Empty;
        }
    }

    [GeneratedRegex(@"<think>(?<reasoning>.*?)</think>(?<response>.*)", RegexOptions.ExplicitCapture | RegexOptions.Singleline, 500)]
    private static partial Regex ThinkDetector();
}
