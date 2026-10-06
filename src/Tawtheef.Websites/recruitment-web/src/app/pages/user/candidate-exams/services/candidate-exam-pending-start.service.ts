import { Injectable } from '@angular/core';
import { CANDIDATE_EXAM_PENDING_START_STORAGE_KEY_PREFIX } from '../constants/candidate-exam-storage.constants';

@Injectable({ providedIn: 'root' })
export class CandidateExamPendingStartService {
  get(testSessionCandidateId: string): string | null {
    const value = sessionStorage.getItem(
      CANDIDATE_EXAM_PENDING_START_STORAGE_KEY_PREFIX + testSessionCandidateId,
    );

    return value && /^[0-9a-f]{8}(?:-[0-9a-f]{4}){3}-[0-9a-f]{12}$/i.test(value)
      ? value
      : null;
  }

  getOrCreate(testSessionCandidateId: string): string {
    const existing = this.get(testSessionCandidateId);

    if (existing) return existing;

    const created = crypto.randomUUID();

    // Persist before sending /start: the response may never reach the browser.
    sessionStorage.setItem(
      CANDIDATE_EXAM_PENDING_START_STORAGE_KEY_PREFIX + testSessionCandidateId,
      created,
    );

    return created;
  }

  clear(testSessionCandidateId: string): void {
    sessionStorage.removeItem(
      CANDIDATE_EXAM_PENDING_START_STORAGE_KEY_PREFIX + testSessionCandidateId,
    );
  }
}
