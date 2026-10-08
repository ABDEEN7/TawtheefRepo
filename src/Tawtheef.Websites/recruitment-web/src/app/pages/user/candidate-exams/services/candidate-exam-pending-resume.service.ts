import { inject, Injectable } from '@angular/core';
import { CANDIDATE_EXAM_PENDING_RESUME_STORAGE_KEY_PREFIX } from '../constants/candidate-exam-storage.constants';
import { CandidateExamPendingOperationStorageService } from './candidate-exam-pending-operation-storage.service';

@Injectable({ providedIn: 'root' })
export class CandidateExamPendingResumeService {
  private readonly storage = inject(CandidateExamPendingOperationStorageService);

  get(testAttemptId: string): string | null {
    return this.storage.get(CANDIDATE_EXAM_PENDING_RESUME_STORAGE_KEY_PREFIX, testAttemptId);
  }

  getOrCreate(testAttemptId: string): string {
    return this.storage.getOrCreate(
      CANDIDATE_EXAM_PENDING_RESUME_STORAGE_KEY_PREFIX,
      testAttemptId,
    );
  }

  clear(testAttemptId: string): void {
    this.storage.clear(CANDIDATE_EXAM_PENDING_RESUME_STORAGE_KEY_PREFIX, testAttemptId);
  }
}
