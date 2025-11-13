import { Component, OnInit } from '@angular/core';
import {AuthResponse} from '../../../core/models/auth/auth-response.model';

export type ExternalMsgType = 'EXTERNAL_LOGIN_SUCCESS' | 'EXTERNAL_LOGIN_ERROR' | 'EXTERNAL_POPUP_CLOSED';
export interface ExternalMsg {
  type: ExternalMsgType;
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
      const payload = this.readPayloadFromHash();
      if (payload) {
        // Optional: state validation if you send it
        // const expected = localStorage.getItem('oauth_state');
        // if (payload.state && payload.state !== expected) { /* reject */ }

        // Post to opener (same-origin)
        this.safePostToOpener(payload);
      }
    } catch (e) {
      // As a last resort, emit an error back
      this.safePostToOpener({ type: 'EXTERNAL_LOGIN_ERROR', message: 'Invalid callback payload' });
      // eslint-disable-next-line no-console
      console.error(e);
    } finally {
      // Close popup after a tiny delay to ensure the postMessage is queued
      setTimeout(() => { try { window.close(); } catch { /* ignore */ } }, 20);
    }
  }

  private readPayloadFromHash(): ExternalMsg | null {
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
    // Convert binary string to utf-8
    const arr = Uint8Array.from(bytes, c => c.charCodeAt(0));
    const decoder = new TextDecoder('utf-8');
    return decoder.decode(arr);
  }

  private safePostToOpener(payload: ExternalMsg): void {
    try {
      if (window.opener && typeof window.opener.postMessage === 'function') {
        window.opener.postMessage(payload, window.location.origin);
      }
    } catch {
      // ignore—if COOP blocks, nothing more to do
    }
  }
}
