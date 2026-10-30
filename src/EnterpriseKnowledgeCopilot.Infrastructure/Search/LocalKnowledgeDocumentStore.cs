using System.Text.Json;
using EnterpriseKnowledgeCopilot.Application.Abstractions;
using EnterpriseKnowledgeCopilot.Application.Services;
using EnterpriseKnowledgeCopilot.Domain.Entities;

namespace EnterpriseKnowledgeCopilot.Infrastructure.Search;

/// <summary>Local development store. Documents remain on disk and are never sent to a cloud service.</summary>
public sealed class LocalKnowledgeDocumentStore : IKnowledgeDocumentStore
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly string _directory;
    private readonly SemaphoreSlim _gate = new(1, 1);

    public LocalKnowledgeDocumentStore(string contentRootPath)
    {
        _directory = Path.GetFullPath(Path.Combine(
            contentRootPath, "..", "..", ".local", "knowledge-base"));
        Directory.CreateDirectory(_directory);
    }

    public async Task<IReadOnlyList<KnowledgeDocument>> ListAsync(CancellationToken cancellationToken)
    {
        await _gate.WaitAsync(cancellationToken);
        try
        {
            var documents = new List<KnowledgeDocument>();
            foreach (var path in Directory.EnumerateFiles(_directory, "*.json"))
            {
                await using var stream = File.OpenRead(path);
                var document = await JsonSerializer.DeserializeAsync<KnowledgeDocument>(stream, JsonOptions, cancellationToken);
                if (document is not null) documents.Add(document);
            }

            return documents.OrderByDescending(document => document.UploadedAt).ToArray();
        }
        finally { _gate.Release(); }
    }

    public async Task<KnowledgeDocument> AddAsync(
        string fileName,
        string text,
        string openAiFileId,
        CancellationToken cancellationToken)
    {
        var id = Guid.NewGuid().ToString("N");
        var chunks = DocumentChunker.Create(id, fileName, text);
        var document = new KnowledgeDocument(id, fileName, DateTimeOffset.UtcNow, chunks.Count, chunks, openAiFileId);

        await _gate.WaitAsync(cancellationToken);
        try
        {
            var path = Path.Combine(_directory, $"{id}.json");
            await using var stream = File.Create(path);
            await JsonSerializer.SerializeAsync(stream, document, JsonOptions, cancellationToken);
        }
        finally { _gate.Release(); }

        return document;
    }

}
