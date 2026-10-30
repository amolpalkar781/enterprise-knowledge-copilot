using EnterpriseKnowledgeCopilot.Domain.Entities;

namespace EnterpriseKnowledgeCopilot.Application.Abstractions;

public interface IDocumentRetriever
{
    Task<IReadOnlyList<RetrievedChunk>> RetrieveAsync(
        string question,
        int maximumResults,
        CancellationToken cancellationToken);
}

