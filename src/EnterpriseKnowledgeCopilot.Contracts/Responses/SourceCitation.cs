namespace EnterpriseKnowledgeCopilot.Contracts.Responses;

public sealed record SourceCitation(
    string CitationId,
    string DocumentId,
    string DocumentName,
    int? PageNumber,
    string Excerpt);

