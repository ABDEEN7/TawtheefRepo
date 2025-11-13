import { Component, OnInit } from '@angular/core';
import {AuthResponse} from '../../../core/models/auth-response.model';

export type ExternalMessageType = 'EXTERNAL_LOGIN_SUCCESS' | 'EXTERNAL_LOGIN_ERROR' | 'EXTERNAL_POPUP_CLOSED';
export interface ExternalMessage {
  type: ExternalMessageType;
  userData?: AuthResponse;
  message?: string;
  content?: string;
  state?: string;
}

@Component({
  selector: 'app-popup-callback',
  standalone: true,
  template: `
    <div style="font-family:sans-serif;padding:16px;text-align:center">
      <p>Completing sign-in…</p>
    </div>
  `
})
export class PopupCallbackComponent implements OnInit {
  ngOnInit(): void {
    try {
      const payload = this.readPayload();
      if (payload) this.safePostToOpener(payload);
    } catch (e) {
      this.safePostToOpener({ type: 'EXTERNAL_LOGIN_ERROR', message: 'Invalid callback payload' });
      console.error(e);
    } finally {
      setTimeout(() => { try { window.close(); } catch {} }, 20);
    }
  }

  private readPayload(): ExternalMessage | null {
    const hash = window.location.hash?.replace(/^#/, '') ?? '';
    const params = new URLSearchParams(hash);
    const b64 = params.get('payload');
    if (!b64) return null;

    const json = this.base64UrlDecode(b64);
    return JSON.parse(json);
  }

  private base64UrlDecode(b64: string): string {
    const pad = b64.length % 4 === 2 ? '==' : b64.length % 4 === 3 ? '=' : '';
    const normalized = b64.replace(/-/g, '+').replace(/_/g, '/') + pad;
    const bytes = atob(normalized);
    const arr = Uint8Array.from(bytes, c => c.charCodeAt(0));
    const decoder = new TextDecoder('utf-8');
    return decoder.decode(arr);
  }

  private safePostToOpener(payload: ExternalMessage): void {
    try {
      if (window.opener && typeof window.opener.postMessage === 'function') {
        // same-origin postMessage: use the SPA origin
        window.opener.postMessage(payload, window.location.origin);
      }
    } catch {
      // ignore
    }
  }
}
