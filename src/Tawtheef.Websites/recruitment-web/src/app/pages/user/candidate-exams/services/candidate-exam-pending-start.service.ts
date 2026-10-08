import { inject, Injectable } from '@angular/core';
import { CANDIDATE_EXAM_PENDING_START_STORAGE_KEY_PREFIX } from '../constants/candidate-exam-storage.constants';
import { CandidateExamPendingOperationStorageService } from './candidate-exam-pending-operation-storage.service';

@Injectable({ providedIn: 'root' })
export class CandidateExamPendingStartService {
  private readonly storage = inject(CandidateExamPendingOperationStorageService);

  get(testSessionCandidateId: string): string | null {
    return this.storage.get(
      CANDIDATE_EXAM_PENDING_START_STORAGE_KEY_PREFIX,
      testSessionCandidateId,
    );
  }

  getOrCreate(testSessionCandidateId: string): string {
    // Persist before sending /start: the response may never reach the browser.
    return this.storage.getOrCreate(
      CANDIDATE_EXAM_PENDING_START_STORAGE_KEY_PREFIX,
      testSessionCandidateId,
    );
  }

  clear(testSessionCandidateId: string): void {
    this.storage.clear(CANDIDATE_EXAM_PENDING_START_STORAGE_KEY_PREFIX, testSessionCandidateId);
  }
}
