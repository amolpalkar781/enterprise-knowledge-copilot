namespace EnterpriseKnowledgeCopilot.Contracts.Responses;

public sealed record ChatResponse(
    string Answer,
    bool IsGrounded,
    IReadOnlyList<SourceCitation> Citations);

