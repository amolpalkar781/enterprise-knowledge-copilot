# Enterprise Knowledge Copilot

A portfolio project for grounded question answering over enterprise documents. The application uses .NET 8 and Angular, with OpenAI Responses API and hosted File Search for RAG. Uploaded files are also kept in the local ignored `.local/knowledge-base/` folder for this development build.

[Technical documentation and architecture diagram](docs/technical-documentation.md)

## Current milestone

- Cleanly separated .NET API, Application, Domain, Infrastructure, and Contracts projects.
- Angular chat interface with an API connection indicator.
- `GET /api/health` and `POST /api/chat` endpoints.
- Upload `.txt` and `.md` files to an OpenAI vector store and ask grounded questions with citations.
- The Responses API uses OpenAI File Search for semantic and keyword retrieval and answer generation.
- OpenAI credentials and vector store settings are read from environment variables; no API key is committed.

The document upload path sends files to the configured OpenAI vector store. Existing files in `.local/knowledge-base/` are local-only and must be uploaded again to be searchable by OpenAI. This version supports `.txt` and `.md` files up to 5 MB.

## Run locally

Create an OpenAI API key and a vector store first. API usage is billed separately from a ChatGPT subscription. In one PowerShell terminal, set `OPENAI_API_KEY`, then create the vector store with:

```powershell
.\scripts\create-openai-vector-store.ps1
```

Copy the returned ID. Set these in the API terminal (keep the key private and out of source control):

```powershell
$env:OPENAI_API_KEY = 'your-api-key'
$env:OPENAI_VECTOR_STORE_ID = 'vs_your-vector-store-id'
$env:OPENAI_MODEL = 'gpt-5.4-mini'
```

The default model is `gpt-5.4-mini` if `OPENAI_MODEL` is omitted. The API will not make OpenAI calls until the API key and vector store ID are configured. Uploaded files are sent to OpenAI for File Search indexing.

Open two terminals from this directory.

Terminal 1 — API:

```powershell
dotnet run --project src/EnterpriseKnowledgeCopilot.Api --launch-profile http
```

The API listens on `http://localhost:5050`; OpenAPI/Swagger is available at `/swagger` in Development.

Terminal 2 — Angular:

```powershell
Set-Location frontend
npm start
```

Open `http://localhost:4200`. The Angular development server proxies `/api` requests to the local API.

Choose **Document library** in the sidebar to upload a `.txt` or `.md` file up to 5 MB. Uploading sends the document to the configured OpenAI vector store and also keeps a local development copy. `GET /api/documents` lists the library and `POST /api/documents` accepts multipart form data with a `file` field.

## Costs and data handling

Responses API model usage is billed by tokens. File Search tool calls and vector storage may also be billed; current pricing includes 1 GB of vector-store storage and charges for File Search calls. Check the [OpenAI API pricing page](https://developers.openai.com/api/docs/pricing) before uploading files. Do not upload confidential documents to this portfolio instance. No Azure resources are required for this OpenAI-hosted RAG path.

## Repository layout

```text
src/
  EnterpriseKnowledgeCopilot.Api/            HTTP API and controllers
  EnterpriseKnowledgeCopilot.Application/    Use cases and service boundaries
  EnterpriseKnowledgeCopilot.Domain/         Core knowledge-domain models
  EnterpriseKnowledgeCopilot.Infrastructure/ OpenAI File Search integration
  EnterpriseKnowledgeCopilot.Contracts/      HTTP request and response models
frontend/                                     Angular user interface
docs/                                         Technical documentation, architecture, and RAG notes
infra/terraform/                              Reserved for optional Azure hosting
sample-data/                                  Synthetic, non-sensitive sample documents
```
