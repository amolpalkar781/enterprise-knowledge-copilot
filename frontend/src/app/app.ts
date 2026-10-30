import { Component, inject, signal } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { firstValueFrom } from 'rxjs';
import { CopilotApiService, DocumentSummary, SourceCitation } from './services/copilot-api.service';

interface ChatMessage {
  role: 'assistant' | 'user';
  content: string;
  citations?: SourceCitation[];
  grounded?: boolean;
}

@Component({
  imports: [FormsModule, DatePipe],
  selector: 'app-root',
  styleUrl: './app.scss',
  templateUrl: './app.html',
})
export class App {
  private readonly copilotApi = inject(CopilotApiService);

  readonly apiStatus = signal<'checking' | 'online' | 'offline'>('checking');
  readonly isSending = signal(false);
  readonly draft = signal('');
  readonly messages = signal<ChatMessage[]>([]);
  readonly documents = signal<DocumentSummary[]>([]);
  readonly showLibrary = signal(false);
  readonly isUploading = signal(false);
  readonly uploadMessage = signal('');
  readonly prompts = [
    'What is the approval process for emergency communication?',
    'Summarize the escalation rules.',
    'Which policy applies to a government customer?',
  ];

  constructor() {
    void this.checkApi();
    void this.loadDocuments();
  }

  async loadDocuments(): Promise<void> {
    try { this.documents.set(await firstValueFrom(this.copilotApi.getDocuments())); }
    catch { this.documents.set([]); }
  }

  async uploadFile(event: Event): Promise<void> {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    if (!file || this.isUploading()) return;
    this.isUploading.set(true);
    this.uploadMessage.set('');
    try {
      const uploaded = await firstValueFrom(this.copilotApi.uploadDocument(file));
      this.documents.update((items) => [uploaded, ...items]);
      this.uploadMessage.set(`${uploaded.fileName} added and split into ${uploaded.chunkCount} searchable chunks.`);
    } catch (error) {
      const detail = error instanceof HttpErrorResponse && typeof error.error?.title === 'string'
        ? error.error.title
        : 'The file could not be added. Check that the API is running.';
      this.uploadMessage.set(detail);
    } finally {
      this.isUploading.set(false);
      input.value = '';
    }
  }

  async checkApi(): Promise<void> {
    try {
      await firstValueFrom(this.copilotApi.getHealth());
      this.apiStatus.set('online');
    } catch {
      this.apiStatus.set('offline');
    }
  }

  choosePrompt(prompt: string): void {
    this.draft.set(prompt);
  }

  onComposerEnter(event: Event): void {
    if ((event as KeyboardEvent).shiftKey) return;
    event.preventDefault();
    void this.sendMessage();
  }

  async sendMessage(): Promise<void> {
    const question = this.draft().trim();
    if (!question || this.isSending()) return;

    this.messages.update((messages) => [...messages, { role: 'user', content: question }]);
    this.draft.set('');
    this.isSending.set(true);

    try {
      const response = await firstValueFrom(this.copilotApi.ask({ question }));
      this.messages.update((messages) => [...messages, {
        role: 'assistant',
        content: response.answer,
        citations: response.citations,
        grounded: response.isGrounded,
      }]);
    } catch (error) {
      const isApiUnavailable = error instanceof HttpErrorResponse && error.status === 0;
      if (isApiUnavailable) this.apiStatus.set('offline');
      const serverDetail = error instanceof HttpErrorResponse
        && typeof error.error?.detail === 'string'
        ? error.error.detail
        : null;
      this.messages.update((messages) => [...messages, {
        role: 'assistant',
        content: isApiUnavailable
          ? 'I could not reach the local API. Start the .NET service, then try again.'
          : serverDetail ?? 'The request could not be completed. Check the API status and try again.',
        grounded: false,
      }]);
    } finally {
      this.isSending.set(false);
    }
  }
}
