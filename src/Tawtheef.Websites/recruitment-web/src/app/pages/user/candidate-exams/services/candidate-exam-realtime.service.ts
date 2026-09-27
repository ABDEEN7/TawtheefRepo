import { inject, Injectable, signal } from '@angular/core';
import { HubConnection, HubConnectionBuilder, HubConnectionState } from '@microsoft/signalr';
import { TokenService } from '../../../../core/auth/token.service';
import { ApiConfigService } from '../../../../core/services/api-config.service';
import { CandidateExamResumeResult } from '../models/candidate-exam-realtime.model';

const CANDIDATE_EXAM_HUB_PATH = '/hubs/candidate-exam';

export type CandidateExamConnectionStatus =
  | 'disconnected'
  | 'connecting'
  | 'connected'
  | 'reconnecting';

@Injectable({ providedIn: 'root' })
export class CandidateExamRealtimeService {
  private readonly apiConfig = inject(ApiConfigService);
  private readonly tokenService = inject(TokenService);
  private readonly connectionStatusState = signal<CandidateExamConnectionStatus>('disconnected');

  private connection: HubConnection | null = null;
  private connectPromise: Promise<void> | null = null;
  private disconnectPromise: Promise<void> | null = null;
  private reconnectPromise: Promise<void> | null = null;
  private resolveReconnect: (() => void) | null = null;
  private rejectReconnect: ((reason: unknown) => void) | null = null;

  readonly connectionStatus = this.connectionStatusState.asReadonly();

  connect(): Promise<void> {
    const connection = this.getOrCreateConnection();

    if (connection.state === HubConnectionState.Connected) {
      return Promise.resolve();
    }

    if (this.connectPromise) {
      return this.connectPromise;
    }

    if (connection.state === HubConnectionState.Reconnecting) {
      return this.getOrCreateReconnectPromise();
    }

    if (connection.state === HubConnectionState.Disconnecting) {
      return (this.disconnectPromise ?? Promise.resolve()).then(() => this.connect());
    }

    this.connectionStatusState.set('connecting');
    this.connectPromise = connection
      .start()
      .then(() => this.connectionStatusState.set('connected'))
      .catch((error: unknown) => {
        this.connectionStatusState.set('disconnected');
        throw error;
      })
      .finally(() => {
        this.connectPromise = null;
      });

    return this.connectPromise;
  }

  registerAttempt(testAttemptId: string, activeSessionId: string): Promise<void> {
    const connection = this.requireConnectedConnection();
    return connection.invoke<void>('RegisterAttempt', testAttemptId, activeSessionId);
  }

  resumeAttempt(testAttemptId: string, accessCode: string): Promise<CandidateExamResumeResult> {
    const connection = this.requireConnectedConnection();
    return connection.invoke<CandidateExamResumeResult>('ResumeAttempt', testAttemptId, accessCode);
  }

  disconnect(): Promise<void> {
    if (this.disconnectPromise) {
      return this.disconnectPromise;
    }

    if (!this.connection || this.connection.state === HubConnectionState.Disconnected) {
      this.connectionStatusState.set('disconnected');
      this.rejectReconnectWait(new Error('Candidate Exam realtime connection was disconnected.'));
      return Promise.resolve();
    }

    this.rejectReconnectWait(new Error('Candidate Exam realtime connection was disconnected.'));
    this.disconnectPromise = this.connection.stop().finally(() => {
      this.connectionStatusState.set('disconnected');
      this.clearReconnectWait();
      this.disconnectPromise = null;
    });

    return this.disconnectPromise;
  }

  private getOrCreateConnection(): HubConnection {
    if (this.connection) return this.connection;

    const connection = new HubConnectionBuilder()
      .withUrl(`${this.apiConfig.apiOrigin}${CANDIDATE_EXAM_HUB_PATH}`, {
        accessTokenFactory: () => this.tokenService.getToken() ?? '',
      })
      .withAutomaticReconnect()
      .build();

    connection.onreconnecting(() => this.connectionStatusState.set('reconnecting'));
    connection.onreconnected(() => {
      this.connectionStatusState.set('connected');
      this.resolveReconnectWait();
    });
    connection.onclose((error) => {
      this.connectionStatusState.set('disconnected');
      this.rejectReconnectWait(
        error ?? new Error('Candidate Exam realtime connection closed during reconnect.'),
      );
    });

    this.connection = connection;
    return connection;
  }

  private getOrCreateReconnectPromise(): Promise<void> {
    if (this.reconnectPromise) return this.reconnectPromise;

    this.reconnectPromise = new Promise<void>((resolve, reject) => {
      this.resolveReconnect = resolve;
      this.rejectReconnect = reject;
    });

    return this.reconnectPromise;
  }

  private resolveReconnectWait(): void {
    const resolve = this.resolveReconnect;
    this.clearReconnectWait();
    resolve?.();
  }

  private rejectReconnectWait(reason: unknown): void {
    const reject = this.rejectReconnect;
    this.clearReconnectWait();
    reject?.(reason);
  }

  private clearReconnectWait(): void {
    this.reconnectPromise = null;
    this.resolveReconnect = null;
    this.rejectReconnect = null;
  }

  private requireConnectedConnection(): HubConnection {
    if (!this.connection || this.connection.state !== HubConnectionState.Connected) {
      throw new Error('Candidate Exam realtime connection is not connected.');
    }

    return this.connection;
  }
}
