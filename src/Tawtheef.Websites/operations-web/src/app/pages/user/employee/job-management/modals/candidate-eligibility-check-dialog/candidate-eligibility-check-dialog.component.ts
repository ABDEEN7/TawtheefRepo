import { Component, inject, OnInit } from '@angular/core';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { JobCandidatesService } from '../../services/job-candidates.service';
import { GUID } from '../../../../../../shared/types/guid.type';
import { CandidateEligibilityResult } from '../../models/candidate-eligibility-result.model';
import { finalize } from 'rxjs/operators';
import { CandidateSearchDto } from '../../models/candidate-search.model';
import { CandidateEligibilityStatusCode, candidateEligibilityStatusCodes } from '../../models/candidate-eligibility-condition.model';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-candidate-eligibility-check-dialog',
  standalone: false,
  templateUrl: './candidate-eligibility-check-dialog.component.html',
  styleUrls: ['./candidate-eligibility-check-dialog.component.scss']
})
export class CandidateEligibilityCheckDialogComponent implements OnInit {
  private config = inject(DynamicDialogConfig);
  private ref = inject(DynamicDialogRef);
  private translate = inject(TranslateService);
  private candidatesService = inject(JobCandidatesService);
  readonly candidateEligibilityStatusCodes = candidateEligibilityStatusCodes;

  jobId!: GUID;
  searchQuery: string = '';

  isSearched: boolean = false;
  loadingCandidates: boolean = false;
  candidates: CandidateSearchDto[] = [];
  selectedCandidateId?: GUID;

  loadingEligibility: boolean = false;
  result?: CandidateEligibilityResult;

  ngOnInit(): void {
    this.jobId = this.config.data?.jobId;
  }

  searchCandidates(): void {
    const query = this.searchQuery?.trim();
    if (!query || query.length < 2) {
      this.candidates = [];
      this.isSearched = false;
      return;
    }

    this.loadingCandidates = true;
    this.isSearched = true;
    this.candidatesService.searchAllCandidates(query)
      .pipe(finalize(() => this.loadingCandidates = false))
      .subscribe({
        next: (res) => {
          this.candidates = res;
        },
        error: () => {
          this.candidates = [];
        }
      });
  }

  selectCandidate(candidateId: GUID): void {
    this.selectedCandidateId = candidateId;
    this.checkEligibility(candidateId);
  }

  checkEligibility(candidateId: GUID): void {
    this.loadingEligibility = true;
    this.result = undefined;

    this.candidatesService.checkCandidateEligibility(this.jobId, candidateId)
      .pipe(finalize(() => this.loadingEligibility = false))
      .subscribe({
        next: (res) => {
          this.result = res;
        }
      });
  }

  close(): void {
    this.ref.close();
  }


  translateEligibilityValue(code?: string, params?: any): string {
    if (!code) {
      return '';
    }

    if (this.isTranslationKey(code)) {
      return this.translate.instant(code, params);
    }

    const staticValueKey = `candidateEligibility.values.${code}`;
    const translated = this.translate.instant(staticValueKey, params);

    if (translated !== staticValueKey) {
      return translated;
    }

    return code;
  }

  private isTranslationKey(value: string): boolean {
    return value.includes('.');
  }

  getBreakdownItems(breakdown: any) {
    if (!breakdown) return [];
    return [
      { label: 'JOB_CANDIDATE_PROFILE_POINTS_CATEGORY', value: breakdown.categoryPoints },
      { label: 'JOB_CANDIDATE_PROFILE_POINTS_EDUCATION', value: breakdown.educationPoints },
      { label: 'JOB_CANDIDATE_PROFILE_POINTS_EXPERIENCE', value: breakdown.experiencePoints },
      { label: 'JOB_CANDIDATE_PROFILE_POINTS_TRAINING', value: breakdown.trainingPoints },
      { label: 'JOB_CANDIDATE_PROFILE_POINTS_SKILLS', value: breakdown.skillPoints },
      { label: 'JOB_CANDIDATE_PROFILE_POINTS_LANGUAGES', value: breakdown.languagePoints },
      { label: 'JOB_CANDIDATE_PROFILE_POINTS_CERTIFICATES', value: breakdown.certificatePoints }
    ];
  }
}

