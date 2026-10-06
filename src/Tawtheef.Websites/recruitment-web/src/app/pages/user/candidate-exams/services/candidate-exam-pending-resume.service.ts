import { Injectable } from '@angular/core';
import { CANDIDATE_EXAM_PENDING_RESUME_STORAGE_KEY_PREFIX } from '../constants/candidate-exam-storage.constants';

@Injectable({ providedIn: 'root' })
export class CandidateExamPendingResumeService {
  get(testAttemptId: string): string | null {
    const value = sessionStorage.getItem(CANDIDATE_EXAM_PENDING_RESUME_STORAGE_KEY_PREFIX + testAttemptId);

    return value && /^[0-9a-f]{8}(?:-[0-9a-f]{4}){3}-[0-9a-f]{12}$/i.test(value) ? value : null;
  }

  getOrCreate(testAttemptId: string): string {
    const existing = this.get(testAttemptId);

    if (existing) return existing;

    const created = crypto.randomUUID();

    sessionStorage.setItem(CANDIDATE_EXAM_PENDING_RESUME_STORAGE_KEY_PREFIX + testAttemptId, created);

    return created;
  }

  clear(testAttemptId: string): void {
    sessionStorage.removeItem(CANDIDATE_EXAM_PENDING_RESUME_STORAGE_KEY_PREFIX + testAttemptId);
  }
}
