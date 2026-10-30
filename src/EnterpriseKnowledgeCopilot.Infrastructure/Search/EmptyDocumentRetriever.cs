using EnterpriseKnowledgeCopilot.Application.Abstractions;
using EnterpriseKnowledgeCopilot.Domain.Entities;

namespace EnterpriseKnowledgeCopilot.Infrastructure.Search;

/// <summary>
/// Keeps the local application honest until the Azure AI Search adapter is added.
/// </summary>
public sealed class EmptyDocumentRetriever : IDocumentRetriever
{
    public Task<IReadOnlyList<RetrievedChunk>> RetrieveAsync(
        string question,
        int maximumResults,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<RetrievedChunk> noResults = Array.Empty<RetrievedChunk>();
        return Task.FromResult(noResults);
    }
}

