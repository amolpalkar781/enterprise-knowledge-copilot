using System.Text;
using EnterpriseKnowledgeCopilot.Application.Services;
using EnterpriseKnowledgeCopilot.Application.Abstractions;
using EnterpriseKnowledgeCopilot.Application.Exceptions;
using EnterpriseKnowledgeCopilot.Contracts.Responses;
using EnterpriseKnowledgeCopilot.Infrastructure.OpenAI;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseKnowledgeCopilot.Api.Controllers;

[ApiController]
[Route("api/documents")]
public sealed class DocumentsController(
    KnowledgeDocumentService documents,
    IOpenAiFileIndexer fileIndexer) : ControllerBase
{
    private const long MaximumFileSize = 5 * 1024 * 1024;

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<DocumentSummary>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<DocumentSummary>>> List(CancellationToken cancellationToken) =>
        Ok((await documents.ListAsync(cancellationToken)).Select(ToSummary).ToArray());

    [HttpPost]
    [RequestSizeLimit(MaximumFileSize + 64 * 1024)]
    [ProducesResponseType(typeof(DocumentSummary), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<DocumentSummary>> Upload(
        IFormFile? file,
        CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
            return BadRequest(new ProblemDetails { Title = "Choose a non-empty file to upload." });
        if (file.Length > MaximumFileSize)
            return BadRequest(new ProblemDetails { Title = "Files must be 5 MB or smaller." });

        var fileName = Path.GetFileName(file.FileName);
        var extension = Path.GetExtension(fileName);
        if (!extension.Equals(".txt", StringComparison.OrdinalIgnoreCase)
            && !extension.Equals(".md", StringComparison.OrdinalIgnoreCase))
            return BadRequest(new ProblemDetails { Title = "Only .txt and .md files are supported." });

        string text;
        try
        {
            using var reader = new StreamReader(file.OpenReadStream(), Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
            text = await reader.ReadToEndAsync(cancellationToken);
        }
        catch (DecoderFallbackException)
        {
            return BadRequest(new ProblemDetails { Title = "The file must contain valid UTF-8 text." });
        }

        if (string.IsNullOrWhiteSpace(text))
            return BadRequest(new ProblemDetails { Title = "The file does not contain readable text." });

        string openAiFileId;
        try
        {
            var contentType = extension.Equals(".md", StringComparison.OrdinalIgnoreCase)
                ? "text/markdown"
                : "text/plain";
            await using var stream = file.OpenReadStream();
            openAiFileId = await fileIndexer.IndexAsync(fileName, stream, contentType, cancellationToken);
        }
        catch (AiServiceNotConfiguredException exception)
        {
            return Problem(
                statusCode: StatusCodes.Status503ServiceUnavailable,
                title: "OpenAI File Search is not configured",
                detail: exception.Message);
        }
        catch (OpenAiApiException)
        {
            return Problem(
                statusCode: StatusCodes.Status502BadGateway,
                title: "OpenAI could not index this document",
                detail: "Check the API configuration and try again.");
        }

        var document = await documents.AddAsync(fileName, text, openAiFileId, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, ToSummary(document));
    }

    private static DocumentSummary ToSummary(Domain.Entities.KnowledgeDocument document) =>
        new(document.Id, document.FileName, document.UploadedAt, document.ChunkCount, document.OpenAiFileId is not null);
}
