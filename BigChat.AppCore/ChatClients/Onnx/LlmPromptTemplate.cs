using System.Collections.ObjectModel;

namespace BigChat.AppCore.ChatClients.Onnx;

public sealed class LlmPromptTemplate
{
    public string? System { get; init; }
    public string? User { get; init; }
    public string? Assistant { get; init; }
    public ReadOnlyCollection<string>? Stop { get; init; }
}