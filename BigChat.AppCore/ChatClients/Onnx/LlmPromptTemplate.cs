using System.Collections.ObjectModel;

namespace BigChat.Embedding.ChatClient;

public sealed class LlmPromptTemplate
{
    public string? System { get; init; }
    public string? User { get; init; }
    public string? Assistant { get; init; }
    public ReadOnlyCollection<string>? Stop { get; init; }
}