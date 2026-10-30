# Architecture

The Angular application calls the ASP.NET Core API. The API sends uploaded files to OpenAI File Search and sends chat questions to the Responses API with that vector store enabled. Infrastructure contains the provider adapter; the API and UI continue to use the same chat and document endpoints.

```mermaid
flowchart LR
    UI[Angular UI] -->|HTTP /api| API[ASP.NET Core API]
    API --> App[Application: IKnowledgeCopilot]
    App --> Adapter[Infrastructure: OpenAiFileSearchClient]
    Adapter --> Upload[OpenAI Files API]
    Upload --> Store[(OpenAI Vector Store)]
    Adapter --> Responses[OpenAI Responses API]
    Responses -->|File Search tool| Store
    API --> Local[Local document catalog]
    API -. HTTP request/response DTOs .-> Contracts[Contracts]
```

The Application layer defines `IKnowledgeCopilot` and a file-indexing boundary; Infrastructure calls OpenAI using HTTP. The API keeps a local document catalog and sends each selected upload to the configured vector store. OpenAI performs chunking, embedding, retrieval, answer generation, and citation annotation. MCP tools are a separate future extension; no MCP server or tool calls are implemented yet.

See [technical-documentation.md](./technical-documentation.md) for endpoint contracts, errors, configuration, and local run instructions.
