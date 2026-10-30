using EnterpriseKnowledgeCopilot.Application.Abstractions;
using EnterpriseKnowledgeCopilot.Domain.Entities;

namespace EnterpriseKnowledgeCopilot.Application.Services;

public sealed class KnowledgeDocumentService(IKnowledgeDocumentStore store)
{
    public Task<IReadOnlyList<KnowledgeDocument>> ListAsync(CancellationToken cancellationToken) =>
        store.ListAsync(cancellationToken);

    public Task<KnowledgeDocument> AddAsync(
        string fileName,
        string text,
        string openAiFileId,
        CancellationToken cancellationToken) =>
        store.AddAsync(fileName, text, openAiFileId, cancellationToken);
}
