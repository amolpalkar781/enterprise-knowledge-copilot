import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

export interface SourceCitation {
  citationId: string;
  documentId: string;
  documentName: string;
  pageNumber: number | null;
  excerpt: string;
}

export interface ChatResponse {
  answer: string;
  isGrounded: boolean;
  citations: SourceCitation[];
}

export interface DocumentSummary {
  id: string;
  fileName: string;
  uploadedAt: string;
  chunkCount: number;
  indexedInOpenAi: boolean;
}

@Injectable({ providedIn: 'root' })
export class CopilotApiService {
  private readonly http = inject(HttpClient);

  getHealth(): Observable<{ status: string }> {
    return this.http.get<{ status: string }>('/api/health');
  }

  ask(request: { question: string }): Observable<ChatResponse> {
    return this.http.post<ChatResponse>('/api/chat', request);
  }

  getDocuments(): Observable<DocumentSummary[]> {
    return this.http.get<DocumentSummary[]>('/api/documents');
  }

  uploadDocument(file: File): Observable<DocumentSummary> {
    const form = new FormData();
    form.append('file', file);
    return this.http.post<DocumentSummary>('/api/documents', form);
  }
}

