using EnterpriseKnowledgeCopilot.Domain.Entities;

namespace EnterpriseKnowledgeCopilot.Application.Services;

public static class DocumentChunker
{
    private const int ChunkSize = 1_000;
    private const int Overlap = 150;

    public static IReadOnlyList<RetrievedChunk> Create(
        string documentId,
        string documentName,
        string text)
    {
        var normalized = string.Join(' ', text.Split(
            (char[]?)null,
            StringSplitOptions.RemoveEmptyEntries));

        var chunks = new List<RetrievedChunk>();
        var start = 0;
        while (start < normalized.Length)
        {
            var end = Math.Min(start + ChunkSize, normalized.Length);
            if (end < normalized.Length)
            {
                var boundary = normalized.LastIndexOf(' ', end, end - start);
                if (boundary > start + ChunkSize / 2) end = boundary;
            }

            var content = normalized[start..end].Trim();
            if (content.Length > 0)
            {
                chunks.Add(new RetrievedChunk(
                    $"{documentId}-{chunks.Count + 1}",
                    documentId,
                    documentName,
                    content,
                    PageNumber: null,
                    RelevanceScore: 0));
            }

            if (end >= normalized.Length) break;
            start = Math.Max(end - Overlap, start + 1);
        }

        return chunks;
    }
}
