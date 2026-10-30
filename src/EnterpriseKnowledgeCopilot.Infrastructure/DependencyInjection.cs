using EnterpriseKnowledgeCopilot.Application.Abstractions;
using EnterpriseKnowledgeCopilot.Infrastructure.OpenAI;
using EnterpriseKnowledgeCopilot.Infrastructure.Search;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EnterpriseKnowledgeCopilot.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        string contentRootPath)
    {
        services.AddSingleton<LocalKnowledgeDocumentStore>(_ =>
            new LocalKnowledgeDocumentStore(contentRootPath));
        services.AddSingleton<IKnowledgeDocumentStore>(provider => provider.GetRequiredService<LocalKnowledgeDocumentStore>());

        var httpClient = new HttpClient { BaseAddress = new Uri("https://api.openai.com/v1/") };
        var openAiClient = new OpenAiFileSearchClient(
            httpClient,
            configuration["OPENAI_API_KEY"],
            configuration["OPENAI_VECTOR_STORE_ID"],
            configuration["OPENAI_MODEL"] ?? "gpt-5.4-mini");
        services.AddSingleton(openAiClient);
        services.AddSingleton<IKnowledgeCopilot>(provider => provider.GetRequiredService<OpenAiFileSearchClient>());
        services.AddSingleton<IOpenAiFileIndexer>(provider => provider.GetRequiredService<OpenAiFileSearchClient>());

        return services;
    }
}

