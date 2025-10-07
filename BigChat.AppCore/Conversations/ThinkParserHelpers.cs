using BigChat.AppCore.Conversations;
using BigChat.AppCore.ViewModel;
using System.Reactive.Linq;

namespace BigChat.AppCore.Conversations;

public static class ThinkParserHelpers
{

    // Markers for the assistant's think area (displayed in a dedicated UI region).
    private static readonly string[] ThinkTagOpens = ["<think>", "<thought>", "<reasoning>"];
    private static readonly string[] ThinkTagCloses = ["</think>", "</thought>", "</reasoning>"];
    private static readonly int MaxOpenThinkMarkerLength = ThinkTagOpens.Max(s => s.Length);

    // Applies a single streamed fragment to the response message. Returns updated rolling buffer and current think tag index.
    public static (string rolling, int currentThinkTagIndex) ApplyPartToResponse(MessageViewModel responseMessage, string rolling, int currentThinkTagIndex, string part)
    {
        // Parse character by character/fragment to identify think tags (e.g., <think>...</think>, <thought>...</thought>)
        rolling += part;

        while (!string.IsNullOrEmpty(rolling))
        {
            if (currentThinkTagIndex == -1)
            {
                // Find the earliest occurring open marker among supported think tags
                int earliestIdx = -1;
                int foundTagIndex = -1;
                for (int i = 0; i < ThinkTagOpens.Length; i++)
                {
                    int idx = rolling.IndexOf(ThinkTagOpens[i], StringComparison.Ordinal);
                    if (idx >= 0 && (earliestIdx == -1 || idx < earliestIdx))
                    {
                        earliestIdx = idx;
                        foundTagIndex = i;
                    }
                }

                if (earliestIdx >= 0)
                {
                    // Output safe content before the start marker
                    if (earliestIdx > 0)
                    {
                        responseMessage.Content = string.Concat(responseMessage.Content, rolling.AsSpan(0, earliestIdx));
                    }

                    // Enter think mode, discard the marker text itself
                    rolling = rolling[(earliestIdx + ThinkTagOpens[foundTagIndex].Length)..];
                    currentThinkTagIndex = foundTagIndex;
                    continue;
                }
                else
                {
                    // Start marker not found: only flush safe parts, keep the tail that might form a marker
                    int keep = MaxOpenThinkMarkerLength - 1;
                    if (rolling.Length > keep)
                    {
                        int flushLen = rolling.Length - keep;
                        responseMessage.Content = string.Concat(responseMessage.Content.TrimStart(), rolling.AsSpan(0, flushLen));
                        rolling = rolling[flushLen..];
                    }

                    break;
                }
            }
            else
            {
                string closeMarker = ThinkTagCloses[currentThinkTagIndex];
                int closeIdx = rolling.IndexOf(closeMarker, StringComparison.Ordinal);
                if (closeIdx >= 0)
                {
                    // Append content before the closing marker to the think box
                    if (closeIdx > 0)
                    {
                        responseMessage.ThinkContent = string.Concat(responseMessage.ThinkContent, rolling.AsSpan(0, closeIdx));
                    }

                    // Exit think mode, discard the closing marker
                    rolling = rolling[(closeIdx + closeMarker.Length)..];
                    currentThinkTagIndex = -1;
                    continue;
                }
                else
                {
                    // Closing marker not found: only flush safe parts, keep the tail that might form a marker
                    int keep = closeMarker.Length - 1;
                    if (rolling.Length > keep)
                    {
                        int flushLen = rolling.Length - keep;
                        responseMessage.ThinkContent = string.Concat(responseMessage.ThinkContent, rolling.AsSpan(0, flushLen));
                        rolling = rolling[flushLen..];
                    }

                    break;
                }
            }
        }

        return (rolling, currentThinkTagIndex);
    }
}