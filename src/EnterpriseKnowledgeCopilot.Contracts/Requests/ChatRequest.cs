using System.ComponentModel.DataAnnotations;

namespace EnterpriseKnowledgeCopilot.Contracts.Requests;

public sealed class ChatRequest
{
    [Required]
    [StringLength(2_000, MinimumLength = 3)]
    public string Question { get; init; } = string.Empty;
}

