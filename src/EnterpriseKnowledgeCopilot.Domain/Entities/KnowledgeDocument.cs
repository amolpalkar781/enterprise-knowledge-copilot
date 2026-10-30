namespace EnterpriseKnowledgeCopilot.Domain.Entities;

public sealed record KnowledgeDocument(
    string Id,
    string FileName,
    DateTimeOffset UploadedAt,
    int ChunkCount,
    IReadOnlyList<RetrievedChunk> Chunks,
    string? OpenAiFileId = null);
