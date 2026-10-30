namespace EnterpriseKnowledgeCopilot.Application.Abstractions;

public interface IOpenAiFileIndexer
{
    Task<string> IndexAsync(
        string fileName,
        Stream content,
        string contentType,
        CancellationToken cancellationToken);
}
