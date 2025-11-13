using Microsoft.Extensions.AI;
using System.Text;

namespace BigChat.AppCore.ChatClients.Onnx;

public static class ConfiguredOnnxChatClientHelpers
{
    public const string TEMPLATEPLACEHOLDER = "{{CONTENT}}";
    public static readonly LlmPromptTemplate template = new()
    {
        System = "<|system|>\n{{CONTENT}}<|end|>\n",
        User = "<|user|>\n{{CONTENT}}<|end|>\n",
        Assistant = "<|assistant|>\n{{CONTENT}}<|end|>\n",
        Stop = ["<|system|>", "<|user|>", "<|assistant|>", "<|end|>"]
    };

    public static string GetPrompt(LlmPromptTemplate? template, ChatMessage[] history, ChatOptions? chatOptions)
    {
        if (history.Length == 0)
        {
            return string.Empty;
        }

        if (template == null)
        {
            return string.Join(". ", history);
        }

        StringBuilder prompt = new();

        string systemMsgWithoutSystemTemplate = string.Empty;

        for (int i = 0; i < history.Length; i++)
        {
            ChatMessage message = history[i];
            if (message.Role == ChatRole.System)
            {
                // ignore system prompts that aren't at the beginning
                if (i == 0)
                {
                    if (string.IsNullOrWhiteSpace(template.System))
                    {
                        systemMsgWithoutSystemTemplate = message.Text ?? string.Empty;
                    }
                    else
                    {
                        prompt.Append(template.System.Replace(TEMPLATEPLACEHOLDER, message.Text, StringComparison.InvariantCulture));
                    }
                }
            }
            else if (message.Role == ChatRole.User)
            {
                string msgText = message.Text ?? string.Empty;
                if (i == 1 && !string.IsNullOrWhiteSpace(systemMsgWithoutSystemTemplate))
                {
                    msgText = $"{systemMsgWithoutSystemTemplate} {msgText}";
                }

                prompt.Append(string.IsNullOrWhiteSpace(template.User) ?
                    msgText :
                    template.User.Replace(TEMPLATEPLACEHOLDER, msgText, StringComparison.InvariantCulture));
            }
            else if (message.Role == ChatRole.Assistant)
            {
                prompt.Append(string.IsNullOrWhiteSpace(template.Assistant) ?
                    message.Text :
                    template.Assistant.Replace(TEMPLATEPLACEHOLDER, message.Text, StringComparison.InvariantCulture));
            }
        }

        if (!string.IsNullOrWhiteSpace(template.Assistant))
        {
            int substringIndex = template.Assistant.IndexOf(TEMPLATEPLACEHOLDER, StringComparison.InvariantCulture);
            prompt.Append(template.Assistant[..substringIndex]);
        }

        return prompt.ToString();
    }
}