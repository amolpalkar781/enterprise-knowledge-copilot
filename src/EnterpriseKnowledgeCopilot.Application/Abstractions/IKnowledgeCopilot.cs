using EnterpriseKnowledgeCopilot.Contracts.Requests;
using EnterpriseKnowledgeCopilot.Contracts.Responses;

namespace EnterpriseKnowledgeCopilot.Application.Abstractions;

public interface IKnowledgeCopilot
{
    Task<ChatResponse> AskAsync(ChatRequest request, CancellationToken cancellationToken);
}

