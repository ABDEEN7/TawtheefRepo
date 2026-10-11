import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class CandidateExamPendingOperationStorageService {
  get(keyPrefix: string, entityId: string): string | null {
    const value = sessionStorage.getItem(keyPrefix + entityId);

    return value && /^[0-9a-f]{8}(?:-[0-9a-f]{4}){3}-[0-9a-f]{12}$/i.test(value) ? value : null;
  }

  getOrCreate(keyPrefix: string, entityId: string): string {
    const existing = this.get(keyPrefix, entityId);

    if (existing) return existing;

    const created = crypto.randomUUID();

    sessionStorage.setItem(keyPrefix + entityId, created);

    return created;
  }

  clear(keyPrefix: string, entityId: string): void {
    sessionStorage.removeItem(keyPrefix + entityId);
  }
}
