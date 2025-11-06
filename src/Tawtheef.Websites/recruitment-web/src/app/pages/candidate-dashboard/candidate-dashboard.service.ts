// candidate-dashboard.service.ts
import { Injectable } from '@angular/core';
import { Observable, of, delay } from 'rxjs';

// Constants for job types
export const JOB_TYPES = {
  ACADEMIC: 'academic',
  ADMINISTRATIVE: 'administrative',
  LABOR: 'labor'
} as const;

export const JOB_TYPE_LABELS = {
  [JOB_TYPES.ACADEMIC]: 'أكاديمي',
  [JOB_TYPES.ADMINISTRATIVE]: 'إداري',
  [JOB_TYPES.LABOR]: 'عمالي'
} as const;

// Constants for job statuses
export const JOB_STATUSES = {
  INVITED: 'invited',
  APPLIED: 'applied',
  UNDER_REVIEW: 'under_review',
  WITHDRAWN: 'withdrawn',
  CLOSED: 'closed'
} as const;

export const JOB_STATUS_LABELS = {
  [JOB_STATUSES.INVITED]: 'دعوة جديدة',
  [JOB_STATUSES.APPLIED]: 'تم التقديم',
  [JOB_STATUSES.UNDER_REVIEW]: 'قيد المراجعة',
  [JOB_STATUSES.WITHDRAWN]: 'ملغي',
  [JOB_STATUSES.CLOSED]: 'مغلقة'
} as const;

// Constants for status pill classes
export const STATUS_PILL_CLASSES = {
  [JOB_STATUSES.INVITED]: 'status-invited',
  [JOB_STATUSES.APPLIED]: 'status-applied',
  [JOB_STATUSES.UNDER_REVIEW]: 'status-underreview',
  [JOB_STATUSES.WITHDRAWN]: 'status-withdrawn',
  [JOB_STATUSES.CLOSED]: 'status-closed'
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
    { value: JOB_STATUSES.INVITED, label: JOB_STATUS_LABELS[JOB_STATUSES.INVITED] },
    { value: JOB_STATUSES.APPLIED, label: JOB_STATUS_LABELS[JOB_STATUSES.APPLIED] },
    { value: JOB_STATUSES.UNDER_REVIEW, label: JOB_STATUS_LABELS[JOB_STATUSES.UNDER_REVIEW] },
    { value: JOB_STATUSES.WITHDRAWN, label: JOB_STATUS_LABELS[JOB_STATUSES.WITHDRAWN] },
    { value: JOB_STATUSES.CLOSED, label: JOB_STATUS_LABELS[JOB_STATUSES.CLOSED] }
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
  [JOB_STATUSES.INVITED]: {
    showApply: true,
    showView: false,
    showTrack: false,
    showDetails: false,
    showWithdraw: false
  },
  [JOB_STATUSES.APPLIED]: {
    showApply: false,
    showView: true,
    showTrack: false,
    showDetails: false,
    showWithdraw: true
  },
  [JOB_STATUSES.UNDER_REVIEW]: {
    showApply: false,
    showView: false,
    showTrack: true,
    showDetails: false,
    showWithdraw: false
  },
  [JOB_STATUSES.WITHDRAWN]: {
    showApply: false,
    showView: false,
    showTrack: false,
    showDetails: true,
    showWithdraw: false
  },
  [JOB_STATUSES.CLOSED]: {
    showApply: false,
    showView: false,
    showTrack: false,
    showDetails: true,
    showWithdraw: false
  }
} as const;

// Type definitions
export type JobType = typeof JOB_TYPES[keyof typeof JOB_TYPES];
export type JobStatus = typeof JOB_STATUSES[keyof typeof JOB_STATUSES];

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
  private readonly fakeData: JobRecord[] = [
    {
      id: 1,
      title: 'معلم رياضيات',
      entity: 'إدارة شؤون المدارس',
      type: JOB_TYPES.ACADEMIC,
      status: JOB_STATUSES.INVITED,
      date: '2025-10-28',
      jobId: 1
    },
    {
      id: 2,
      title: 'أخصائي موارد بشرية',
      entity: 'إدارة الموارد البشرية',
      type: JOB_TYPES.ADMINISTRATIVE,
      status: JOB_STATUSES.APPLIED,
      date: '2025-10-28',
      jobId: 2
    },
    {
      id: 3,
      title: 'فني شبكات',
      entity: 'إدارة نظم المعلومات',
      type: JOB_TYPES.LABOR,
      status: JOB_STATUSES.INVITED,
      date: '2025-10-25',
      jobId: 3
    },
    {
      id: 4,
      title: 'مشرف نشاط طلابي',
      entity: 'إدارة التقييم',
      type: JOB_TYPES.ACADEMIC,
      status: JOB_STATUSES.WITHDRAWN,
      date: '2025-10-20',
      jobId: 4
    },
    {
      id: 5,
      title: 'منسق مختبرات',
      entity: 'إدارة التقييم',
      type: JOB_TYPES.ACADEMIC,
      status: JOB_STATUSES.UNDER_REVIEW,
      date: '2025-10-30',
      jobId: 5
    },
    {
      id: 6,
      title: 'كاتب إداري',
      entity: 'إدارة الموارد البشرية',
      type: JOB_TYPES.ADMINISTRATIVE,
      status: JOB_STATUSES.CLOSED,
      date: '2025-10-10',
      jobId: 6
    }
  ];

  // Get all constants for use in components
  getConstants() {
    return {
      JOB_TYPES,
      JOB_TYPE_LABELS,
      JOB_STATUSES,
      JOB_STATUS_LABELS,
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
      record.status = JOB_STATUSES.WITHDRAWN;
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
      invited: this.fakeData.filter(r => r.status === JOB_STATUSES.INVITED).length,
      applied: this.fakeData.filter(r => r.status === JOB_STATUSES.APPLIED).length,
      under_review: this.fakeData.filter(r => r.status === JOB_STATUSES.UNDER_REVIEW).length,
      withdrawn: this.fakeData.filter(r => r.status === JOB_STATUSES.WITHDRAWN).length,
      closed: this.fakeData.filter(r => r.status === JOB_STATUSES.CLOSED).length
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
}
