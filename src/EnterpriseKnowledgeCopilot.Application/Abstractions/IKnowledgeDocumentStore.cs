using EnterpriseKnowledgeCopilot.Domain.Entities;

namespace EnterpriseKnowledgeCopilot.Application.Abstractions;

public interface IKnowledgeDocumentStore
{
    Task<IReadOnlyList<KnowledgeDocument>> ListAsync(CancellationToken cancellationToken);

    Task<KnowledgeDocument> AddAsync(
        string fileName,
        string text,
        string openAiFileId,
        CancellationToken cancellationToken);
}
