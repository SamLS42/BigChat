using BigChat.AppCore.ChatClient;
using BigChat.AppCore.Settings;
using System.Text.Json.Serialization;

namespace BigChat.Utils;


[JsonSerializable(typeof(ChatCompletionsClientSettings))]
[JsonSerializable(typeof(OllamaChatClientSettings))]
[JsonSerializable(typeof(SupportedClients))]
internal sealed partial class SourceGenerationContext : JsonSerializerContext;
