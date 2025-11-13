using BigChat.AppCore.ChatClients;
using BigChat.AppCore.Settings.AzureAIInference;
using BigChat.AppCore.Settings.Ollama;
using BigChat.AppCore.Settings.Onnx;
using BigChat.AppCore.Settings.OpenAI;
using System.Text.Json.Serialization;

namespace BigChat.AppCore.Settings;


[JsonSerializable(typeof(AzureAIInferenceClientSettings))]
[JsonSerializable(typeof(OllamaChatClientSettings))]
[JsonSerializable(typeof(OnnxChatClientSettings))]
[JsonSerializable(typeof(OpenAIClientSettings))]
[JsonSerializable(typeof(SupportedClients))]
[JsonSerializable(typeof(WindowState))]
public sealed partial class SourceGenerationContext : JsonSerializerContext;
