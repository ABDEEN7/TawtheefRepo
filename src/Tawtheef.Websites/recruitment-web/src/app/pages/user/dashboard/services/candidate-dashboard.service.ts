// candidate-dashboard.service.ts
import {inject, Injectable, signal} from '@angular/core';
import {Observable, of, delay, forkJoin} from 'rxjs';
import { LookupDto } from '../../wizard-profile/services/profile-lookups.service';
import {HttpClient} from '@angular/common/http';
import {EndpointsService} from '../../../../core/http/endpoints.service';

// Constants for job types
export const JOB_TYPES = {
  ACADEMIC: 'Academic',
  ADMINISTRATIVE: 'Administrative',
  LABOR: 'Labor'
} as const;

export const JOB_TYPE_LABELS = {
  [JOB_TYPES.ACADEMIC]: 'أكاديمي',
  [JOB_TYPES.ADMINISTRATIVE]: 'إداري',
  [JOB_TYPES.LABOR]: 'عمالي'
} as const;

// Constants for job statuses
export const JOB_INVITATION_STATUSES = {
  NEW_INVITATION: 'NewInvitation',
  CLOSED: 'Closed',
  UNDER_REVIEW: 'UnderReview',
  APPROVED: 'Approved',
  READED: 'Readed',
  REJECTED: 'Rejected',
  CANCELLED: 'Cancelled',
  REQUIRES_UPDATE: 'RequiresUpdate',
  SUBMITTED: 'Submitted'
} as const;

export const JOB_INVITATION_STATUS_LABELS = {
  [JOB_INVITATION_STATUSES.NEW_INVITATION]: 'دعوة جديدة',
  [JOB_INVITATION_STATUSES.CLOSED]: 'مغلق',
  [JOB_INVITATION_STATUSES.UNDER_REVIEW]: 'قيد المراجعة',
  [JOB_INVITATION_STATUSES.APPROVED]: 'معتمد',
  [JOB_INVITATION_STATUSES.READED]: 'تمت القراءة',
  [JOB_INVITATION_STATUSES.REJECTED]: 'مرفوض',
  [JOB_INVITATION_STATUSES.CANCELLED]: 'ملغي',
  [JOB_INVITATION_STATUSES.REQUIRES_UPDATE]: 'مطلوب تعديل',
  [JOB_INVITATION_STATUSES.SUBMITTED]: 'تم التقديم'
} as const;

// Constants for status pill classes
export const STATUS_PILL_CLASSES: Record<JobStatus, string> = {
  [JOB_INVITATION_STATUSES.NEW_INVITATION]: 'status-invited',
  [JOB_INVITATION_STATUSES.CLOSED]: 'status-closed',
  [JOB_INVITATION_STATUSES.UNDER_REVIEW]: 'status-underreview',
  [JOB_INVITATION_STATUSES.APPROVED]: 'status-applied',
  [JOB_INVITATION_STATUSES.READED]: 'status-withdrawn',
  [JOB_INVITATION_STATUSES.REJECTED]: 'status-closed',
  [JOB_INVITATION_STATUSES.CANCELLED]: 'status-closed',
  [JOB_INVITATION_STATUSES.REQUIRES_UPDATE]: 'status-withdrawn',
  [JOB_INVITATION_STATUSES.SUBMITTED]: 'status-applied'
} as const;

// Constants for type badge classes
export const TYPE_BADGE_CLASSES = {
  [JOB_TYPES.ACADEMIC]: 'badge-soft academic',
  [JOB_TYPES.ADMINISTRATIVE]: 'badge-soft administrative',
  [JOB_TYPES.LABOR]: 'badge-soft labor'
} as const;

// Filter options
export const FILTER_OPTIONS = {
  STATUS: [
    { value: '', label: 'الكل' },
    { value: JOB_INVITATION_STATUSES.NEW_INVITATION, label: JOB_INVITATION_STATUS_LABELS[JOB_INVITATION_STATUSES.NEW_INVITATION] },
    { value: JOB_INVITATION_STATUSES.SUBMITTED, label: JOB_INVITATION_STATUS_LABELS[JOB_INVITATION_STATUSES.SUBMITTED] },
    { value: JOB_INVITATION_STATUSES.UNDER_REVIEW, label: JOB_INVITATION_STATUS_LABELS[JOB_INVITATION_STATUSES.UNDER_REVIEW] },
    { value: JOB_INVITATION_STATUSES.CANCELLED, label: JOB_INVITATION_STATUS_LABELS[JOB_INVITATION_STATUSES.CANCELLED] },
    { value: JOB_INVITATION_STATUSES.CLOSED, label: JOB_INVITATION_STATUS_LABELS[JOB_INVITATION_STATUSES.CLOSED] }
  ],
  TYPE: [
    { value: '', label: 'الكل' },
    { value: JOB_TYPES.ACADEMIC, label: JOB_TYPE_LABELS[JOB_TYPES.ACADEMIC] },
    { value: JOB_TYPES.ADMINISTRATIVE, label: JOB_TYPE_LABELS[JOB_TYPES.ADMINISTRATIVE] },
    { value: JOB_TYPES.LABOR, label: JOB_TYPE_LABELS[JOB_TYPES.LABOR] }
  ]
} as const;

// Action configurations
export const ACTION_CONFIGS = {
  [JOB_INVITATION_STATUSES.NEW_INVITATION]: {
    showApply: true,
    showView: false,
    showTrack: false,
    showDetails: false,
    showWithdraw: false
  },
  [JOB_INVITATION_STATUSES.SUBMITTED]: {
    showApply: false,
    showView: true,
    showTrack: false,
    showDetails: false,
    showWithdraw: true
  },
  [JOB_INVITATION_STATUSES.UNDER_REVIEW]: {
    showApply: false,
    showView: false,
    showTrack: true,
    showDetails: false,
    showWithdraw: false
  },
  [JOB_INVITATION_STATUSES.REJECTED]: {
    showApply: false,
    showView: false,
    showTrack: false,
    showDetails: true,
    showWithdraw: false
  },
  [JOB_INVITATION_STATUSES.CLOSED]: {
    showApply: false,
    showView: false,
    showTrack: false,
    showDetails: true,
    showWithdraw: false
  },
  [JOB_INVITATION_STATUSES.APPROVED]: {
    showApply: false,
    showView: true,
    showTrack: true,
    showDetails: true,
    showWithdraw: false
  },
  [JOB_INVITATION_STATUSES.CANCELLED]: {
    showApply: false,
    showView: true,
    showTrack: false,
    showDetails: false,
    showWithdraw: false
  },
  [JOB_INVITATION_STATUSES.REQUIRES_UPDATE]: {
    showApply: false,
    showView: true,
    showTrack: false,
    showDetails: false,
    showWithdraw: false
  },
  [JOB_INVITATION_STATUSES.READED]: {
    showApply: false,
    showView: true,
    showTrack: false,
    showDetails: false,
    showWithdraw: false
  },
} as const;

// Type definitions
export type JobType = typeof JOB_TYPES[keyof typeof JOB_TYPES];
export type JobStatus = typeof JOB_INVITATION_STATUSES[keyof typeof JOB_INVITATION_STATUSES];

// Interfaces
export interface JobRecord {
  id: number;
  title: string;
  entity: string;
  type: JobType;
  status: JobStatus;
  date: string;
  jobId: number;
}

export interface ApiResponse<T> {
  data: T;
  message: string;
  success: boolean;
}

export interface DashboardStats {
  total: number;
  invited: number;
  applied: number;
  under_review: number;
  withdrawn: number;
  closed: number;
}

export interface FilterOption {
  value: string;
  label: string;
}

@Injectable({
  providedIn: 'root'
})
export class CandidateDashboardService {
  private http = inject(HttpClient);
  private endpoints = inject(EndpointsService);
  private readonly fakeData: JobRecord[] = [
    {
      id: 1,
      title: 'معلم رياضيات',
      entity: 'إدارة شؤون المدارس',
      type: JOB_TYPES.ACADEMIC,
      status: JOB_INVITATION_STATUSES.NEW_INVITATION,
      date: '2025-10-28',
      jobId: 1
    },
    {
      id: 2,
      title: 'أخصائي موارد بشرية',
      entity: 'إدارة الموارد البشرية',
      type: JOB_TYPES.ADMINISTRATIVE,
      status: JOB_INVITATION_STATUSES.SUBMITTED,
      date: '2025-10-28',
      jobId: 2
    },
    {
      id: 3,
      title: 'فني شبكات',
      entity: 'إدارة نظم المعلومات',
      type: JOB_TYPES.LABOR,
      status: JOB_INVITATION_STATUSES.NEW_INVITATION,
      date: '2025-10-25',
      jobId: 3
    },
    {
      id: 4,
      title: 'مشرف نشاط طلابي',
      entity: 'إدارة التقييم',
      type: JOB_TYPES.ACADEMIC,
      status: JOB_INVITATION_STATUSES.REJECTED,
      date: '2025-10-20',
      jobId: 4
    },
    {
      id: 5,
      title: 'منسق مختبرات',
      entity: 'إدارة التقييم',
      type: JOB_TYPES.ACADEMIC,
      status: JOB_INVITATION_STATUSES.UNDER_REVIEW,
      date: '2025-10-30',
      jobId: 5
    },
    {
      id: 6,
      title: 'كاتب إداري',
      entity: 'إدارة الموارد البشرية',
      type: JOB_TYPES.ADMINISTRATIVE,
      status: JOB_INVITATION_STATUSES.CLOSED,
      date: '2025-10-10',
      jobId: 6
    }
  ];
  loading = signal<boolean>(false);
  loaded = signal<boolean>(false);

  invitationStatuses   = signal<LookupDto[]>([]);
  jobCategories   = signal<LookupDto[]>([]);
  departments          = signal<LookupDto[]>([]);

  // Get all constants for use in components
  getConstants() {
    return {
      JOB_TYPES,
      JOB_TYPE_LABELS,
      JOB_INVITATION_STATUSES,
      JOB_INVITATION_STATUS_LABELS,
      STATUS_PILL_CLASSES,
      TYPE_BADGE_CLASSES,
      FILTER_OPTIONS,
      ACTION_CONFIGS
    };
  }

  // Simulate API call to get job records
  getJobRecords(): Observable<ApiResponse<JobRecord[]>> {
    return of({
      data: [...this.fakeData],
      message: 'Records fetched successfully',
      success: true
    }).pipe(
      delay(800)
    );
  }

  // Simulate API call to withdraw application
  withdrawApplication(recordId: number): Observable<ApiResponse<{id: number}>> {
    const record = this.fakeData.find(r => r.id === recordId);
    if (record) {
      record.status = JOB_INVITATION_STATUSES.REJECTED;
    }

    return of({
      data: { id: recordId },
      message: 'Application withdrawn successfully',
      success: true
    }).pipe(
      delay(500)
    );
  }

  // Simulate API call to get dashboard statistics
  getDashboardStats(): Observable<ApiResponse<DashboardStats>> {
    const stats: DashboardStats = {
      total: this.fakeData.length,
      invited: this.fakeData.filter(r => r.status === JOB_INVITATION_STATUSES.NEW_INVITATION).length,
      applied: this.fakeData.filter(r => r.status === JOB_INVITATION_STATUSES.SUBMITTED).length,
      under_review: this.fakeData.filter(r => r.status === JOB_INVITATION_STATUSES.UNDER_REVIEW).length,
      withdrawn: this.fakeData.filter(r => r.status === JOB_INVITATION_STATUSES.REJECTED).length,
      closed: this.fakeData.filter(r => r.status === JOB_INVITATION_STATUSES.CLOSED).length
    };

    return of({
      data: stats,
      message: 'Statistics fetched successfully',
      success: true
    }).pipe(
      delay(300)
    );
  }

  // Simulate API call to apply for a job
  applyForJob(jobId: number): Observable<ApiResponse<{jobId: number, applicationId: string}>> {
    return of({
      data: { jobId, applicationId: 'APP-' + Date.now() },
      message: 'Application submitted successfully',
      success: true
    }).pipe(
      delay(1000)
    );
  }

  // Simulate API call to get single job details
  getJobDetails(jobId: number): Observable<ApiResponse<JobRecord>> {
    const job = this.fakeData.find(r => r.jobId === jobId);
    if (job) {
      return of({
        data: job,
        message: 'Job details fetched successfully',
        success: true
      }).pipe(delay(400));
    } else {
      return of({
        data: {} as JobRecord,
        message: 'Job not found',
        success: false
      }).pipe(delay(400));
    }
  }

  loadCandidateLookups() {
    if (this.loaded()) return;
    this.loading.set(true);

    forkJoin({
      invitationStatuses:  this.http.get<LookupDto[]>(this.endpoints.dashboard.lookups.invitationStatuses),
      jobCategories:  this.http.get<LookupDto[]>(this.endpoints.dashboard.lookups.jobCategories),
      departments:         this.http.get<LookupDto[]>(this.endpoints.dashboard.lookups.departments),

    }).subscribe({
      next: (res) => {
        this.invitationStatuses.set(res.invitationStatuses);
        this.jobCategories.set(res.jobCategories);
        this.departments.set(res.departments);

        this.loaded.set(true);
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Failed to load profile lookups', err);
        this.loading.set(false);
      }
    });
  }
}
