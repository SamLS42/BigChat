using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BigChat.AppCore.Conversations;
using BigChat.AppCore.ViewModel;

namespace BigChat.AppCore.Benchmarks
{
    [MemoryDiagnoser]
    public class ThinkParserBenchmarks
    {
        private List<string> _fragments = new();

        [GlobalSetup]
        public void Setup()
        {
            // Build a representative message containing normal text and think tags interleaved
            string sample = "Hello, this is a sample response that includes some <think>internal reasoning that might be long</think> and then continues with more visible text. " +
                            "Sometimes multiple tags like <thought>short thought</thought> or <reasoning>more reasoning content</reasoning> appear. " +
                            "We also include some edge cases where tags are split across fragments so the parser must keep a rolling buffer.\n";

            // Repeat to make message larger
            var full = string.Concat(Enumerable.Repeat(sample, 50));

            // Create many small fragments to simulate streaming (fixed small chunk sizes to avoid CA5394 warnings)
            for (int i = 0; i < full.Length; i += 12)
            {
                int take = Math.Min(12, full.Length - i);
                _fragments.Add(full.Substring(i, take));
            }
        }

        [Benchmark]
        public void ApplyPartsStreamedFragments()
        {
            var message = new MessageViewModel();
            string rolling = string.Empty;
            int currentThinkTagIndex = -1;

            foreach (var frag in _fragments)
            {
                var res = ThinkParserHelpers.ApplyPartToResponse(message, rolling, currentThinkTagIndex, frag);
                rolling = res.rolling;
                currentThinkTagIndex = res.currentThinkTagIndex;
            }

            // Simple verification to ensure work isn't optimized away
            if (string.IsNullOrEmpty(message.Content) && string.IsNullOrEmpty(message.ThinkContent))
                throw new InvalidOperationException("unexpected empty result");
        }
    }
}