# RAG pipeline

This implementation uses the hosted File Search tool in the OpenAI Responses API. It automatically chunks and embeds uploaded files in an OpenAI vector store, then retrieves relevant passages at question time.

## Ingestion

```text
.txt / .md → OpenAI Files API → OpenAI vector store → chunking + embeddings
```

## Question answering

```text
Question → Responses API → File Search → grounded answer + source citations
```

The assistant must treat retrieved document text as untrusted data, ignore instructions embedded in documents, and refuse when retrieved passages do not support an answer. The current upload API accepts `.txt` and `.md` files up to 5 MB. MCP tools are a separate extension for app actions such as listing or managing documents; File Search handles RAG.
