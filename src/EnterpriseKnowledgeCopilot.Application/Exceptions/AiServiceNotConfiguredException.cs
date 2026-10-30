namespace EnterpriseKnowledgeCopilot.Application.Exceptions;

public sealed class AiServiceNotConfiguredException(string serviceName)
    : Exception($"The {serviceName} service is not configured.")
{
    public string ServiceName { get; } = serviceName;
}

