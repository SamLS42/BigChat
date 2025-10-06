using BigChat.AppCore.ChatClient;
using BigChat.AppCore.Settings.AzureAIInference;
using BigChat.AppCore.Settings.Ollama;
using BigChat.AppCore.Settings.Onnx;
using System.Text.Json.Serialization;

namespace BigChat.Utils;


[JsonSerializable(typeof(AzureAIInferenceClientSettings))]
[JsonSerializable(typeof(OllamaChatClientSettings))]
[JsonSerializable(typeof(OnnxChatClientSettings))]
[JsonSerializable(typeof(SupportedClients))]
internal sealed partial class SourceGenerationContext : JsonSerializerContext;
