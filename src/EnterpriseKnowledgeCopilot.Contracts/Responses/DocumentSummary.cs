namespace EnterpriseKnowledgeCopilot.Contracts.Responses;

public sealed record DocumentSummary(
    string Id,
    string FileName,
    DateTimeOffset UploadedAt,
    int ChunkCount,
    bool IndexedInOpenAi);
