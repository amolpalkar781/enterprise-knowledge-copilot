namespace EnterpriseKnowledgeCopilot.Infrastructure.OpenAI;

public sealed class OpenAiApiException : Exception
{
    public OpenAiApiException(string message, Exception? innerException = null)
        : base(message, innerException) { }

    public OpenAiApiException(int statusCode, string? errorCode)
        : base(errorCode is null
            ? $"OpenAI API returned HTTP {statusCode}."
            : $"OpenAI API returned HTTP {statusCode} ({errorCode}).")
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
    }

    public int? StatusCode { get; }
    public string? ErrorCode { get; }
}
