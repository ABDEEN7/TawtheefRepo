import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { finalize } from 'rxjs';

import { NotificationService } from '../../../../../../core/services/notification.service';
import { GUID } from '../../../../../../shared/types/guid.type';
import { JobCandidateProfile } from '../../models/job-candidate-profile.model';
import { JobResponse } from '../../models/job-response-model';
import { JobCandidatesService } from '../../services/job-candidates.service';
import { JobService } from '../../services/job.service';

interface PointsBreakdownItem {
  labelKey: string;
  points: number;
  percentage: number;
}

interface JobCandidateProfileDialogData {
  jobId: GUID;
  candidateId: GUID;
}

@Component({
  selector: 'app-job-candidate-profile-dialog',
  standalone: false,
  templateUrl: './job-candidate-profile.dialog.component.html',
  styleUrl: './job-candidate-profile.dialog.component.scss',
})
export class JobCandidateProfileDialogComponent implements OnInit {
  private readonly jobService = inject(JobService);
  private readonly jobCandidatesService = inject(JobCandidatesService);
  private readonly notificationService = inject(NotificationService);
  private readonly translateService = inject(TranslateService);
  private readonly dialogRef = inject(DynamicDialogRef);
  private readonly dialogConfig = inject(DynamicDialogConfig);

  jobId!: GUID;
  candidateId!: GUID;
  jobInfo?: JobResponse;

  isLoading = signal<boolean>(false);
  candidateProfile = signal<JobCandidateProfile | null>(null);
  pointsBreakdown = signal<PointsBreakdownItem[]>([]);

  totalPoints = computed(() => this.candidateProfile()?.points?.totalPoints ?? 0);

  candidateInitials = computed(() => {
    const name = this.candidateProfile()?.candidateName?.trim();

    if (!name) return '—';

    return name
      .split(' ')
      .filter(Boolean)
      .slice(0, 2)
      .map((part) => part[0])
      .join('')
      .toUpperCase();
  });

  highestScoreItem = computed(() => {
    return this.pointsBreakdown().length
      ? this.pointsBreakdown()[0]
      : null;
  });

  ngOnInit(): void {
    const dialogData = this.dialogConfig.data as JobCandidateProfileDialogData;

    this.jobId = dialogData.jobId;
    this.candidateId = dialogData.candidateId;

    this.loadJobInfo();
    this.loadCandidateProfile();
  }

  closeDialog(): void {
    this.dialogRef.close();
  }

  private loadJobInfo(): void {
    this.jobService.getById(this.jobId).subscribe({
      next: (job) => {
        if (job) {
          this.jobInfo = job;
        }
      },
    });
  }

  private loadCandidateProfile(): void {
    this.isLoading.set(true);

    this.jobCandidatesService
      .getCandidateProfile(this.jobId, this.candidateId)
      .pipe(finalize(() => this.isLoading.set(false)))
      .subscribe({
        next: (profile) => {
          this.candidateProfile.set(profile);
          this.pointsBreakdown.set(this.buildPointsBreakdown(profile));
        },
        error: () => {
          this.notificationService.error(
            this.translateService.instant('JOB_CANDIDATE_PROFILE_LOAD_FAILED')
          );
        },
      });
  }

  private buildPointsBreakdown(profile: JobCandidateProfile): PointsBreakdownItem[] {
    const totalPoints = profile.points.totalPoints || 0;

    const entries = [
      {
        labelKey: 'JOB_CANDIDATE_PROFILE_POINTS_CATEGORY',
        points: profile.points.categoryPoints,
      },
      {
        labelKey: 'JOB_CANDIDATE_PROFILE_POINTS_EDUCATION',
        points: profile.points.educationPoints,
      },
      {
        labelKey: 'JOB_CANDIDATE_PROFILE_POINTS_EXPERIENCE',
        points: profile.points.experiencePoints,
      },
      {
        labelKey: 'JOB_CANDIDATE_PROFILE_POINTS_TRAINING',
        points: profile.points.trainingPoints,
      },
      {
        labelKey: 'JOB_CANDIDATE_PROFILE_POINTS_SKILLS',
        points: profile.points.skillPoints,
      },
      {
        labelKey: 'JOB_CANDIDATE_PROFILE_POINTS_LANGUAGES',
        points: profile.points.languagePoints,
      },
      {
        labelKey: 'JOB_CANDIDATE_PROFILE_POINTS_CERTIFICATES',
        points: profile.points.certificatePoints,
      },
    ];

    return entries
      .map((entry) => ({
        ...entry,
        points: entry.points ?? 0,
        percentage: totalPoints > 0 ? Math.round(((entry.points ?? 0) / totalPoints) * 100) : 0,
      }))
      .sort((a, b) => b.points - a.points);
  }
}