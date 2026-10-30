using EnterpriseKnowledgeCopilot.Application.Abstractions;
using EnterpriseKnowledgeCopilot.Contracts.Requests;
using EnterpriseKnowledgeCopilot.Contracts.Responses;

namespace EnterpriseKnowledgeCopilot.Application.Services;

public sealed class KnowledgeCopilot(
    IDocumentRetriever documentRetriever,
    IAnswerGenerator answerGenerator) : IKnowledgeCopilot
{
    private const int MaximumRetrievedChunks = 5;

    public async Task<ChatResponse> AskAsync(
        ChatRequest request,
        CancellationToken cancellationToken)
    {
        var chunks = await documentRetriever.RetrieveAsync(
            request.Question,
            MaximumRetrievedChunks,
            cancellationToken);

        if (chunks.Count == 0)
        {
            return new ChatResponse(
                "I couldn't find supporting documents for that question yet. Add documents to the knowledge base, then try again.",
                IsGrounded: false,
                Citations: []);
        }

        var answer = await answerGenerator.GenerateAsync(
            request.Question,
            chunks,
            cancellationToken);

        var citations = chunks
            .Select((chunk, index) => new SourceCitation(
                CitationId: $"[{index + 1}]",
                DocumentId: chunk.DocumentId,
                DocumentName: chunk.DocumentName,
                PageNumber: chunk.PageNumber,
                Excerpt: chunk.Content.Length <= 280
                    ? chunk.Content
                    : $"{chunk.Content[..277]}..."))
            .ToArray();

        return new ChatResponse(answer, IsGrounded: true, citations);
    }
}

