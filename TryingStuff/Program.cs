using BigChat.Infrastructure;
using BigChat.Infrastructure.Data;
using BigChat.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;

IServiceProvider ServiceProvider = new ServiceCollection()
    .AddServices()
    .BuildServiceProvider();

IEmbeddingGenerator<string, Embedding<float>> generator = ServiceProvider.GetRequiredService<IEmbeddingGenerator<string, Embedding<float>>>();

string filename = "RandomFileName";

(string Value, Embedding<float> Embedding)[] embeddings = await generator.GenerateAndZipAsync(
[
    "What is AI?",
    "What is .NET?",
    "How to use AI in .NET?"
]);

//foreach ((string Value, Embedding<float> Embedding) item in embeddings)
//{
//    Console.WriteLine($"'{item.Value}' embedding length: {item.Embedding.Vector.Length}");
//    Console.WriteLine($"'Vector': [{string.Join(", ", item.Embedding.Vector.ToArray().Take(20))}]");
//    Console.WriteLine($"--------------------------------");
//}

MyDbContext myDbContext = ServiceProvider.GetRequiredService<MyDbContext>();

#pragma warning disable IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
await myDbContext.Database.EnsureDeletedAsync();
await myDbContext.Database.EnsureCreatedAsync();
#pragma warning restore IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.

if (await myDbContext.FileEmbeddings.AnyAsync() is false)
{
    EntityEntry<UserFile> result = await myDbContext.UserFiles.AddAsync(new UserFile() { Name = filename });

    await myDbContext.FileEmbeddings.AddRangeAsync(embeddings.Select(e => new FileEmbedding() { Contents = e.Value, ContentsEmbedding = new(e.Embedding.Vector.ToArray()), UserFile = result.Entity }));

    await myDbContext.SaveChangesAsync();
}

string searchTerm = "inteligencia artificial";

float[] searchTermEmbedding = (await generator.GenerateEmbeddingVectorAsync(searchTerm)).ToArray();

var res = await myDbContext.FileEmbeddings.Select(fe => new { fe.Contents, distance = myDbContext.VecDistanceCosine(fe.ContentsEmbedding, searchTermEmbedding) })
    .OrderBy(r => r.distance)
    .Take(3)
    .ToArrayAsync();

foreach (var item in res)
{
    Console.WriteLine(string.Create(CultureInfo.InvariantCulture, $"Value: '{item.Contents}', Distance: '{item.distance}'"));
}