import { Injectable, inject, signal } from '@angular/core';
import { Observable, of } from 'rxjs';
import { tap } from 'rxjs/operators';
import { HttpService } from '../../../core/http/http.service';
import { EndpointsService } from '../../../core/http/endpoints.service';
import { GUID } from '../../../shared/types/guid.type';
import { Job } from '../models/job.model';
import { UpdateJobRequest } from '../models/update-job-request.model';
import { PaginatedRequest } from '../../../core/models/paginated-request.model';
import { JobQueryFilter } from '../models/job-query-filter.model';
import { PaginatedResult } from '../../../core/models/paginated-result.model';
import { JobResponse } from '../models/job-response-model';
import { UpdateJobCommand } from '../models/update-job-command.model';
import { NotificationService } from '../../../core/services/notification.service';
import { TranslateService } from '@ngx-translate/core';
import { JobLookupService } from './job-lookup.service';
import { JobStatus } from '../../../core/enums/lookups.enum';

@Injectable({
  providedIn: 'root'
})
export class JobService {
  private httpService = inject(HttpService);
  private endpoints = inject(EndpointsService);
  private notificationService = inject(NotificationService);
  private translationService = inject(TranslateService);
  private lookupService = inject(JobLookupService);
  
  private currentJob = signal<Job | null>(null);
  private currentJobId: GUID | null = null;
  private jobStatus = signal<string>('draft');

  createNewDraft(): Job {
    const today = new Date();
    const defaultClosingDate = new Date();
    defaultClosingDate.setDate(today.getDate() + 30);
    
    const draft: Job = {
      titleAr: '',
      titleEn: '',
      sectorId: '' as GUID,
      managementId: '' as GUID,
      departmentId: '' as GUID,
      yearsOfExperience: 0,
      jobCategoryId: '' as GUID,
      workLocationId: '' as GUID,
      genderId: null,
      majorId: '' as GUID,
      subMajorId: null,
      workTypeId: '' as GUID,
      numberOfVacancies: 1,
      closingDate: defaultClosingDate,
      minimumAge: 18,
      maximumAge: 60,
      
      overviewAr: '',
      overviewEn: '',
      benefitsAr: '',
      benefitsEn: '',
      qualificationsDescriptionAr: '',
      qualificationsDescriptionEn: '',
      
      degrees: [],
      conditions: [],
      responsibilities: [],
      skills: [],
      requiredAttachments: [],
      
      quota: undefined
    };
    
    this.currentJob.set(draft);
    this.jobStatus.set('draft');
    return draft;
  }

  getCurrentJob(): Job | null {
    return this.currentJob();
  }

  updateCurrentJobBasics(data: {
    titleAr: string;
    titleEn: string;
    sectorId: GUID;
    managementId: GUID;
    departmentId: GUID;
    yearsOfExperience: number;
    jobCategoryId: GUID;
    workLocationId: GUID;
    genderId: GUID | null;
    majorId: GUID;
    subMajorId: GUID | null;
    workTypeId: GUID;
    numberOfVacancies: number;
    closingDate: Date;
    minimumAge: number;
    maximumAge: number;
  }): void {
    const current = this.currentJob();
    if (current) {
      this.currentJob.set({ 
        ...current, 
        ...data 
      });
    }
  }

  updateCurrentJobOverview(overviewAr: string, overviewEn: string = ''): void {
    const current = this.currentJob();
    if (current) {
      this.currentJob.set({ 
        ...current, 
        overviewAr,
        overviewEn 
      });
    }
  }

  updateCurrentJobQualifications(
    degrees: { degreeId: GUID }[], 
    descriptionAr: string, 
    descriptionEn: string = ''
  ): void {
    const current = this.currentJob();
    if (current) {
      this.currentJob.set({ 
        ...current, 
        degrees,
        qualificationsDescriptionAr: descriptionAr,
        qualificationsDescriptionEn: descriptionEn
      });
    }
  }

  updateCurrentJobResponsibilities(responsibilities: { textAr: string; textEn: string }[]): void {
    const current = this.currentJob();
    if (current) {
      this.currentJob.set({ 
        ...current, 
        responsibilities 
      });
    }
  }

  updateCurrentJobConditions(conditions: { textAr: string; textEn: string }[]): void {
    const current = this.currentJob();
    if (current) {
      this.currentJob.set({ 
        ...current, 
        conditions 
      });
    }
  }

  updateCurrentJobSkills(skills: { skillId: GUID; showToApplicants: boolean }[]): void {
    const current = this.currentJob();
    if (current) {
      this.currentJob.set({ 
        ...current, 
        skills 
      });
    }
  }

  updateCurrentJobAttachments(attachments: { titleAr: string; titleEn: string; isMandatory: boolean }[]): void {
    const current = this.currentJob();
    if (current) {
      this.currentJob.set({ 
        ...current, 
        requiredAttachments: attachments 
      });
    }
  }

  updateCurrentJobBenefits(benefitsAr: string, benefitsEn: string = ''): void {
    const current = this.currentJob();
    if (current) {
      this.currentJob.set({ 
        ...current, 
        benefitsAr,
        benefitsEn 
      });
    }
  }

  updateCurrentJobQuota(quotaData: any): void {
    const current = this.currentJob();
    if (current) {
      this.currentJob.set({
        ...current,
        quota: quotaData
      });
    }
  }

  validateRequiredFields(job: Job): { isValid: boolean; errors: string[] } {
    const errors: string[] = [];

    if (!job.titleAr) errors.push('VALIDATION.JOB.TITLE_AR_REQUIRED');
    if (!job.titleEn) errors.push('VALIDATION.JOB.TITLE_EN_REQUIRED');
    if (!job.sectorId) errors.push('VALIDATION.JOB.SECTOR_REQUIRED');
    if (!job.managementId) errors.push('VALIDATION.JOB.MANAGEMENT_REQUIRED');
    if (!job.departmentId) errors.push('VALIDATION.JOB.DEPARTMENT_REQUIRED');
    if (!job.jobCategoryId) errors.push('VALIDATION.JOB.JOB_CATEGORY_REQUIRED');
    if (!job.workLocationId) errors.push('VALIDATION.JOB.WORK_LOCATION_REQUIRED');
    if (!job.majorId) errors.push('VALIDATION.JOB.MAJOR_REQUIRED');
    if (!job.workTypeId) errors.push('VALIDATION.JOB.WORK_TYPE_REQUIRED');
    
    return {
      isValid: errors.length === 0,
      errors
    };
  }

  saveJobDraft(jobData: Job): Observable<GUID> {
    return this.create(jobData).pipe(
      tap((jobId) => {
        this.currentJobId = jobId;
        this.notificationService.success(
          this.translationService.instant('JOB_WIZARD.MESSAGES.DRAFT_SAVED')
        );
      })
    );
  }

  submitJobForApproval(jobId: GUID): Observable<void> {
    const job = this.currentJob();
    if (!job) {
      const errorMessage = this.translationService.instant('JOB_WIZARD.ERRORS.NO_JOB_TO_SUBMIT');
      this.notificationService.error(errorMessage);
      return of(void 0);
    }

    const statusId = this.lookupService.getStatusIdByEnum(JobStatus.PendingApproval);
    if (!statusId) {
      const errorMessage = this.translationService.instant('JOB_WIZARD.ERRORS.STATUS_NOT_FOUND');
      this.notificationService.error(errorMessage);
      return of(void 0);
    }

    const validation = this.validateRequiredFields(job);
    if (!validation.isValid) {
      const errorMessage = validation.errors
        .map(errorKey => this.translationService.instant(errorKey))
        .join('\n');
      this.notificationService.error(errorMessage);
      return of(void 0);
    }

    return this.changeStatus(jobId, statusId).pipe(
      tap(() => {
        this.jobStatus.set('pending_approval');
        this.notificationService.success(
          this.translationService.instant('JOB_WIZARD.MESSAGES.SUBMITTED_FOR_APPROVAL')
        );
      })
    );
  }

  create(request: Job): Observable<GUID> {
    const payload = { Job: request };
    return this.httpService.post<GUID>(this.endpoints.job.job, payload);
  }

  getById(jobId: GUID): Observable<JobResponse> {
    return this.httpService.get<JobResponse>(`${this.endpoints.job.job}/${jobId}`);
  }

  update(jobId: GUID,): Observable<void> {
    const updateCommand: UpdateJobCommand = {
      jobId: jobId,
      job: this.currentJob() as UpdateJobRequest
    };
    return this.httpService.put<void>(`${this.endpoints.job.job}`, updateCommand);
  }

  delete(jobId: GUID): Observable<void> {
    return this.httpService.delete<void>(`${this.endpoints.job.job}/${jobId}`);
  }

  changeStatus(jobId: GUID, statusId: GUID): Observable<void> {
  return this.httpService.put<void>(
    `${this.endpoints.job.job}/${jobId}/status?statusId=${statusId}`,
    null
  );
}

  submitForApproval(jobId: GUID, statusId: GUID): Observable<void> {
    return this.changeStatus(jobId, statusId);
  }

  approve(jobId: GUID, statusId: GUID): Observable<void> {
    return this.changeStatus(jobId, statusId).pipe(
      tap(() => {
        this.notificationService.success(
          this.translationService.instant('JOB_WIZARD.MESSAGES.APPROVED')
        );
      })
    );
  }

  reject(jobId: GUID, statusId: GUID): Observable<void> {
    return this.changeStatus(jobId, statusId).pipe(
      tap(() => {
        this.notificationService.success(
          this.translationService.instant('JOB_WIZARD.MESSAGES.REJECTED')
        );
      })
    );
  }

  publish(jobId: GUID, statusId: GUID): Observable<void> {
    return this.changeStatus(jobId, statusId).pipe(
      tap(() => {
        this.notificationService.success(
          this.translationService.instant('JOB_WIZARD.MESSAGES.PUBLISHED')
        );
      })
    );
  }

  close(jobId: GUID, statusId: GUID): Observable<void> {
    return this.changeStatus(jobId, statusId).pipe(
      tap(() => {
        this.notificationService.success(
          this.translationService.instant('JOB_WIZARD.MESSAGES.CLOSED')
        );
      })
    );
  }

  cancel(jobId: GUID, statusId: GUID): Observable<void> {
    return this.changeStatus(jobId, statusId).pipe(
      tap(() => {
        this.notificationService.success(
          this.translationService.instant('JOB_WIZARD.MESSAGES.CANCELLED')
        );
      })
    );
  }

  getAll(
    pagination: PaginatedRequest = { pageNumber: 1, pageSize: 10 },
    filter?: JobQueryFilter
  ): Observable<PaginatedResult<JobResponse>> {
    const params = this.buildQueryParams(pagination, filter);
    return this.httpService.get<PaginatedResult<JobResponse>>(this.endpoints.job.job, params);
  }

  clearCurrentJob(): void {
    this.currentJob.set(null);
    this.currentJobId = null;
    this.jobStatus.set('draft');
  }

  getCurrentJobStatus(): string {
    return this.jobStatus();
  }

  getCurrentJobId(): GUID | null {
    return this.currentJobId;
  }

  loadJobForEdit(jobId: GUID): Observable<JobResponse> {
    return this.getById(jobId).pipe(
      tap((jobResponse) => {
        const job: Job = {
          titleAr: jobResponse.titleAr,
          titleEn: jobResponse.titleEn,
          sectorId: jobResponse.sector.id as GUID,
          managementId: jobResponse.management.id as GUID,
          departmentId: jobResponse.department.id as GUID,
          yearsOfExperience: jobResponse.yearsOfExperience,
          jobCategoryId: jobResponse.jobCategory.id as GUID,
          workLocationId: jobResponse.workLocation.id as GUID,
          genderId: jobResponse.gender?.id as GUID,
          majorId: jobResponse.major.id as GUID,
          subMajorId: jobResponse.subMajor?.id as GUID,
          workTypeId: jobResponse.workType.id as GUID,
          numberOfVacancies: jobResponse.numberOfVacancies,
          closingDate: new Date(jobResponse.closingDate),
          minimumAge: jobResponse.minimumAge,
          maximumAge: jobResponse.maximumAge,
          overviewAr: jobResponse.overviewAr || '',
          overviewEn: jobResponse.overviewEn || '',
          benefitsAr: jobResponse.benefitsAr || '',
          benefitsEn: jobResponse.benefitsEn || '',
          qualificationsDescriptionAr: jobResponse.qualificationsDescriptionAr || '',
          qualificationsDescriptionEn: jobResponse.qualificationsDescriptionEn || '',
          degrees: jobResponse.degrees.map(d => ({ degreeId: d.degreeId })),
          conditions: jobResponse.conditions.map(c => ({ 
            textAr: c.textAr, 
            textEn: c.textEn 
          })),
          responsibilities: jobResponse.responsibilities.map(r => ({ 
            textAr: r.textAr, 
            textEn: r.textEn 
          })),
          skills: jobResponse.skills.map(s => ({ 
            skillId: s.skillId, 
            showToApplicants: s.showToApplicants 
          })),
          requiredAttachments: jobResponse.requiredAttachments.map(a => ({ 
            titleAr: a.titleAr, 
            titleEn: a.titleEn, 
            isMandatory: a.isMandatory 
          })),
          quota: jobResponse.quota ? {
            qatariCitizens: jobResponse.quota.qatariCitizens,
            qatarMother: jobResponse.quota.qatarMother,
            nonQatariSpouse: jobResponse.quota.nonQatariSpouse,
            gcc: jobResponse.quota.gcc,
            quGrads: jobResponse.quota.quGrads,
            residents: jobResponse.quota.residents,
            residentsBreakdowns: jobResponse.quota.residentsBreakdowns?.map(b => ({
              nationalityId: b.nationalityId,
              percentage: b.percentage
            }))
          } : undefined
        };
        
        this.currentJob.set(job);
        this.currentJobId = jobId;
        this.jobStatus.set(jobResponse.status.backendName);
      })
    );
  }

  private buildQueryParams(pagination: PaginatedRequest, filter?: JobQueryFilter): any {
    const params: any = {
      pageNumber: (pagination.pageNumber ?? 1).toString(),
      pageSize: (pagination.pageSize ?? 10).toString()
    };

    if (pagination.sortBy) {
      params.sortBy = pagination.sortBy;
      params.sortDirection = pagination.sortDirection ?? 'desc';
    }

    if (filter) {
      Object.entries(filter).forEach(([key, value]) => {
        if (value !== undefined && value !== null && value !== '') {
          params[key] = this.formatParamValue(value);
        }
      });
    }

    return params;
  }

  private formatParamValue(value: any): string {
    if (value instanceof Date) {
      return value.toISOString();
    }
    return value.toString();
  }
}