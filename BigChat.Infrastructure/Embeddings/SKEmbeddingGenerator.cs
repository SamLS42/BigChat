using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel.Connectors.Onnx;

namespace BigChat.Infrastructure.Embeddings;

#pragma warning disable SKEXP0070
public class SKEmbeddingGenerator : IEmbeddingGenerator<string, Embedding<float>>
{
    private string ModelId { get; } = "all-MiniLM-L6-v2_quint8_avx2.onnx";
    private BertOnnxTextEmbeddingGenerationService EmbeddingGenerationService { get; }
    public SKEmbeddingGenerator(BertOnnxOptions? options = null)
    {
        string modelPath = Path.Combine(AppContext.BaseDirectory, "Embeddings", ModelId);
        string vocabPath = Path.Combine(AppContext.BaseDirectory, "Embeddings", "vocab.txt");

        EmbeddingGenerationService = BertOnnxTextEmbeddingGenerationService.Create(modelPath, vocabPath, options);
    }

    public async Task<GeneratedEmbeddings<Embedding<float>>> GenerateAsync(IEnumerable<string> values, EmbeddingGenerationOptions? options = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(values);

        IList<ReadOnlyMemory<float>> embeddings = await EmbeddingGenerationService.GenerateEmbeddingsAsync([.. values], cancellationToken: cancellationToken);

        return [.. embeddings.Select(e => new Embedding<float>(e)
        {
            CreatedAt = DateTimeOffset.UtcNow,
            ModelId = options?.ModelId ?? ModelId,
        })];
    }

    public object? GetService(Type serviceType, object? serviceKey = null)
    {
        return null;
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            EmbeddingGenerationService.Dispose();
        }
    }
}