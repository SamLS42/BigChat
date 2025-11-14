namespace BigChat.AppCore.ChatClients;

public enum SupportedClients
{
    Unconfigured = 0,
    Ollama = 1,
    AzureAIInference = 2,
    Onnx = 3,
    OpenAI = 4,
    Test = 5,
}