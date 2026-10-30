using EnterpriseKnowledgeCopilot.Application.Abstractions;
using EnterpriseKnowledgeCopilot.Application.Exceptions;
using EnterpriseKnowledgeCopilot.Contracts.Requests;
using EnterpriseKnowledgeCopilot.Contracts.Responses;
using EnterpriseKnowledgeCopilot.Infrastructure.OpenAI;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseKnowledgeCopilot.Api.Controllers;

[ApiController]
[Route("api/chat")]
public sealed class ChatController(
    IKnowledgeCopilot knowledgeCopilot,
    ILogger<ChatController> logger) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(ChatResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status502BadGateway)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult<ChatResponse>> Ask(
        [FromBody] ChatRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await knowledgeCopilot.AskAsync(request, cancellationToken));
        }
        catch (AiServiceNotConfiguredException exception)
        {
            return Problem(
                statusCode: StatusCodes.Status503ServiceUnavailable,
                title: "OpenAI File Search is not configured",
                detail: exception.Message);
        }
        catch (OpenAiApiException exception)
        {
            logger.LogError(exception, "The OpenAI API request failed.");
            return Problem(
                statusCode: StatusCodes.Status502BadGateway,
                title: "OpenAI request failed",
                detail: exception.Message);
        }
    }
}

