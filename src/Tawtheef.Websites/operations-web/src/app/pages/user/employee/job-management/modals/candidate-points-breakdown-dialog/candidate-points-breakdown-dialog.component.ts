import { Component, inject, OnInit } from '@angular/core';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { JobCandidatesService } from '../../services/job-candidates.service';
import { GUID } from '../../../../../../shared/types/guid.type';
import { CandidateEligibilityResult, CandidatePointsBreakdown } from '../../models/candidate-eligibility-result.model';
import { finalize } from 'rxjs/operators';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-candidate-points-breakdown-dialog',
  standalone: false,
  templateUrl: './candidate-points-breakdown-dialog.component.html',
  styleUrls: ['./candidate-points-breakdown-dialog.component.scss']
})
export class CandidatePointsBreakdownDialogComponent implements OnInit {
  private config = inject(DynamicDialogConfig);
  private ref = inject(DynamicDialogRef);
  private candidatesService = inject(JobCandidatesService);
  private translate = inject(TranslateService);

  jobId!: GUID;
  candidateId!: GUID;
  candidateName!: string;

  isLoading: boolean = false;
  result?: CandidateEligibilityResult;
  
  get isArabic(): boolean {
    return this.translate.currentLang === 'ar';
  }

  ngOnInit(): void {
    this.jobId = this.config.data?.jobId;
    this.candidateId = this.config.data?.candidateId;
    this.candidateName = this.config.data?.candidateName;

    this.loadBreakdown();
  }

  loadBreakdown(): void {
    this.isLoading = true;
    this.candidatesService.checkCandidateEligibility(this.jobId, this.candidateId)
      .pipe(finalize(() => this.isLoading = false))
      .subscribe({
        next: (res) => {
          this.result = res;
        }
      });
  }

  getBreakdownItems(breakdown?: CandidatePointsBreakdown) {
    if (!breakdown) return [];
    return [
      { label: 'JOB_CANDIDATE_PROFILE_POINTS_CATEGORY', value: breakdown.categoryPoints, icon: 'hgi-category' },
      { label: 'JOB_CANDIDATE_PROFILE_POINTS_EDUCATION', value: breakdown.educationPoints, icon: 'hgi-educational-study' },
      { label: 'JOB_CANDIDATE_PROFILE_POINTS_EXPERIENCE', value: breakdown.experiencePoints, icon: 'hgi-briefcase' },
      { label: 'JOB_CANDIDATE_PROFILE_POINTS_TRAINING', value: breakdown.trainingPoints, icon: 'hgi-license' },
      { label: 'JOB_CANDIDATE_PROFILE_POINTS_SKILLS', value: breakdown.skillPoints, icon: 'hgi-bulb' },
      { label: 'JOB_CANDIDATE_PROFILE_POINTS_LANGUAGES', value: breakdown.languagePoints, icon: 'hgi-translate' },
      { label: 'JOB_CANDIDATE_PROFILE_POINTS_CERTIFICATES', value: breakdown.certificatePoints, icon: 'hgi-certificate' }
    ];
  }

  close(): void {
    this.ref.close();
  }
}
