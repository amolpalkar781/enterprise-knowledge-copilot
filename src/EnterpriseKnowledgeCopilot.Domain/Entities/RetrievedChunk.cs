namespace EnterpriseKnowledgeCopilot.Domain.Entities;

public sealed record RetrievedChunk(
    string ChunkId,
    string DocumentId,
    string DocumentName,
    string Content,
    int? PageNumber,
    double RelevanceScore);

