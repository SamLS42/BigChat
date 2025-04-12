using BigChat.Infrastructure.ChatClient;
using BigChat.Infrastructure.Settings;
using System.Text.Json.Serialization;

namespace BigChat.Utils;


[JsonSerializable(typeof(ChatCompletionsClientSettings))]
[JsonSerializable(typeof(OllamaChatClientSettings))]
[JsonSerializable(typeof(SupportedClients))]
internal sealed partial class SourceGenerationContext : JsonSerializerContext;
