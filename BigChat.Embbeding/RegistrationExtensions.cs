using Microsoft.Extensions.DependencyInjection;

namespace BigChat.Embbeding;

public static class RegistrationExtensions
{
    private static readonly string modelFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Models", "distiluse-base-multilingual-cased-v2");
    public static IServiceCollection AddEmbeddingServices(this IServiceCollection serviceCollection)
    {
        return serviceCollection.AddBertOnnxEmbeddingGenerator(onnxModelPath: Path.Combine(modelFolder, "model.onnx"),
                vocabPath: Path.Combine(modelFolder, "vocab.txt"));
    }
}
