import { Injectable, inject, signal } from '@angular/core';
import { Observable, of, map } from 'rxjs';
import { tap } from 'rxjs/operators';
import { GUID } from '../../../../../shared/types/guid.type';
import { Job } from '../models/job.model';
import { JobQueryFilter } from '../models/job-query-filter.model';
import { JobResponse } from '../models/job-response-model';
import { TranslateService } from '@ngx-translate/core';
import { JobLookupService } from './job-lookup.service';
import { JobTabStatus } from '../enums/job-tab-status';
import { JobReviewResponse } from '../models/job-review-response';
import { JobCopyTemplate } from '../models/job-copy-template.model';
import { EndpointsService } from '../../../../../core/http/endpoints.service';
import { HttpService } from '../../../../../core/http/http.service';
import { NotificationService } from '../../../../../core/services/notification.service';
import { PaginatedRequest } from '../../../../../core/models/paginated-request.model';
import { PaginatedResult } from '../../../../../core/models/paginated-result.model';
import { JobStatus, Degree } from '../../../../../core/enums/lookups.enum';
import { JobSpecialization } from '../models/job-specialization.model';

@Injectable({
  providedIn: 'root',
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
  jobTabStatus = JobTabStatus;
  createNewDraft(): Job {
    const today = new Date();
    const defaultClosingDate = new Date();
    defaultClosingDate.setDate(today.getDate() + 30);

    const draft: Job = {
      jobTitleId: '' as GUID,
      titleAr: '',
      titleEn: '',
      jobNumber: '',
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

      jobSpecializations: [],
      degrees: [],
      conditions: [],
      responsibilities: [],
      skills: [],
      requiredAttachments: [],
    };

    this.currentJob.set(draft);
    this.jobStatus.set('draft');
    return draft;
  }

  getCurrentJob(): Job | null {
    return this.currentJob();
  }

  updateCurrentJobBasics(data: {
    jobTitleId: GUID;
    titleAr?: string;
    titleEn?: string;
    jobNumber?: string;
    sectorId: GUID;
    managementId: GUID;
    departmentId: GUID;
    yearsOfExperience: number;
    jobCategoryId: GUID;
    workLocationId: GUID;
    genderId: GUID | null;
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
        ...data,
      });
    }
  }

  updateCurrentJobOverview(overviewAr: string, overviewEn: string = ''): void {
    const current = this.currentJob();
    if (current) {
      this.currentJob.set({
        ...current,
        overviewAr,
        overviewEn,
      });
    }
  }

  updateCurrentJobQualifications(
    degrees: { degreeId: GUID }[],
    majorId: GUID | null,
    subMajorId: GUID | null,
    jobSpecializations: JobSpecialization[],
    descriptionAr: string,
    descriptionEn: string = '',
    majorName?: string,
    subMajorName?: string
  ): void {
    const current = this.currentJob();
    if (current) {
      this.currentJob.set({
        ...current,
        degrees,
        majorId,
        subMajorId,
        jobSpecializations,
        qualificationsDescriptionAr: descriptionAr,
        qualificationsDescriptionEn: descriptionEn,
        majorName,
        subMajorName,
      });
    }
  }

  updateCurrentJobResponsibilities(responsibilities: { textAr: string; textEn: string }[]): void {
    const current = this.currentJob();
    if (current) {
      this.currentJob.set({
        ...current,
        responsibilities,
      });
    }
  }

  updateCurrentJobConditions(conditions: { textAr: string; textEn: string }[]): void {
    const current = this.currentJob();
    if (current) {
      this.currentJob.set({
        ...current,
        conditions,
      });
    }
  }

  updateCurrentJobSkills(skills: { skillId: GUID; showToApplicants: boolean; isRequired: boolean }[]): void {
    const current = this.currentJob();
    if (current) {
      this.currentJob.set({
        ...current,
        skills,
      });
    }
  }

  updateCurrentJobAttachments(
    attachments: { titleAr: string; titleEn: string; isMandatory: boolean }[]
  ): void {
    const current = this.currentJob();
    if (current) {
      this.currentJob.set({
        ...current,
        requiredAttachments: attachments,
      });
    }
  }

  updateCurrentJobBenefits(benefitsAr: string, benefitsEn: string = ''): void {
    const current = this.currentJob();
    if (current) {
      this.currentJob.set({
        ...current,
        benefitsAr,
        benefitsEn,
      });
    }
  }

  validateRequiredFields(job: Job): { isValid: boolean; errors: string[] } {
    const errors: string[] = [];

    if (!job.jobTitleId) errors.push('VALIDATION.REQUIRED_FIELD');
    if (!job.sectorId) errors.push('VALIDATION.JOB.SECTOR_REQUIRED');
    if (!job.managementId) errors.push('VALIDATION.JOB.MANAGEMENT_REQUIRED');
    if (!job.jobCategoryId) errors.push('VALIDATION.JOB.JOB_CATEGORY_REQUIRED');
    if (!job.workLocationId) errors.push('VALIDATION.JOB.WORK_LOCATION_REQUIRED');

    const simplifiedDegrees = [
      Degree.Secondary,
      Degree.Preparatory,
      Degree.Primary,
    ];

    const needsMajor = job.degrees && job.degrees.length > 0 ?
      job.degrees.some(d => {
        const degreeId = d.degreeId;
        const degreeObj = this.lookupService.degrees().find(ld => ld.id === degreeId);
        return !simplifiedDegrees.includes(degreeObj?.backendName as any);
      }) : true;

    if (needsMajor && !job.majorId) errors.push('VALIDATION.JOB.MAJOR_REQUIRED');

    if (!job.workTypeId) errors.push('VALIDATION.JOB.WORK_TYPE_REQUIRED');
    if (job.numberOfVacancies <= 0) errors.push('JOB_WIZARD.VALIDATION.MIN_VACANCIES');
    if (!job.closingDate || new Date(job.closingDate) <= new Date())
      errors.push('JOB_WIZARD.VALIDATION.FUTURE_DATE_REQUIRED');
    if (job.minimumAge <= 0 || job.maximumAge <= 0 || job.maximumAge <= job.minimumAge)
      errors.push('JOB_WIZARD.VALIDATION.AGE_RANGE_INVALID');
    if (job.yearsOfExperience < 0) errors.push('JOB_WIZARD.VALIDATION.MIN_EXPERIENCE');
    return {
      isValid: errors.length === 0,
      errors,
    };
  }

  getCopyTemplate(jobId: GUID): Observable<JobCopyTemplate> {
    return this.httpService.get<JobCopyTemplate>(this.endpoints.job.copyTemplate(jobId));
  }

  createFromPrevious(
    jobId: GUID,
    payload: JobCopyTemplate
  ): Observable<GUID> {
    return this.httpService.post<GUID>(
      this.endpoints.job.copyFromPrevious(jobId),
      payload
    );
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

  submitTabReview(formData: FormData) {
    return this.httpService.post(this.endpoints.job.jobApproval, formData);
  }

  updateTabReview(payload: any) {
    return this.httpService.put(this.endpoints.job.jobApproval, payload);
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
        .map((errorKey) => this.translationService.instant(errorKey))
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

  // ──────────────────────────────────────────────
  //  Section-Specific Update Methods
  // ──────────────────────────────────────────────

  updateBasics(jobId: GUID, data: any): Observable<void> {
    const job = this.currentJob();
    return this.httpService.put<void>(this.endpoints.job.updateBasics(jobId), {
      ...data,
      rowVersion: job?.rowVersion
    });
  }

  updateOverview(jobId: GUID): Observable<void> {
    const job = this.currentJob();
    if (!job) return of(void 0);
    return this.httpService.put<void>(this.endpoints.job.updateOverview(jobId), {
      overviewAr: job.overviewAr,
      overviewEn: job.overviewEn,
      rowVersion: job.rowVersion
    });
  }

  updateQualifications(jobId: GUID): Observable<void> {
    const job = this.currentJob();
    if (!job) return of(void 0);
    return this.httpService.put<void>(this.endpoints.job.updateQualifications(jobId), {
      majorId: job.majorId,
      subMajorId: job.subMajorId,
      qualificationsDescriptionAr: job.qualificationsDescriptionAr,
      qualificationsDescriptionEn: job.qualificationsDescriptionEn,
      degrees: job.degrees,
      jobSpecializations: job.jobSpecializations,
      rowVersion: job.rowVersion
    });
  }

  updateResponsibilities(jobId: GUID): Observable<void> {
    const job = this.currentJob();
    if (!job) return of(void 0);
    return this.httpService.put<void>(this.endpoints.job.updateResponsibilities(jobId), {
      responsibilities: job.responsibilities,
      rowVersion: job.rowVersion
    });
  }

  updateConditions(jobId: GUID): Observable<void> {
    const job = this.currentJob();
    if (!job) return of(void 0);
    return this.httpService.put<void>(this.endpoints.job.updateConditions(jobId), {
      conditions: job.conditions,
      rowVersion: job.rowVersion
    });
  }

  updateSkills(jobId: GUID): Observable<void> {
    const job = this.currentJob();
    if (!job) return of(void 0);
    return this.httpService.put<void>(this.endpoints.job.updateSkills(jobId), {
      skills: job.skills,
      rowVersion: job.rowVersion
    });
  }

  updateAttachments(jobId: GUID): Observable<void> {
    const job = this.currentJob();
    if (!job) return of(void 0);
    return this.httpService.put<void>(this.endpoints.job.updateAttachments(jobId), {
      requiredAttachments: job.requiredAttachments,
      rowVersion: job.rowVersion
    });
  }

  updateBenefits(jobId: GUID): Observable<void> {
    const job = this.currentJob();
    if (!job) return of(void 0);
    return this.httpService.put<void>(this.endpoints.job.updateBenefits(jobId), {
      benefitsAr: job.benefitsAr,
      benefitsEn: job.benefitsEn,
      rowVersion: job.rowVersion
    });
  }

  delete(jobId: GUID): Observable<void> {
    return this.httpService.delete<void>(`${this.endpoints.job.job}/${jobId}`);
  }

  deleteSpecialization(id: GUID): Observable<void> {
    return this.httpService.delete<void>(this.endpoints.job.deleteJobSpecialization(id));
  }

  changeStatus(jobId: GUID, statusId: GUID): Observable<void> {
    return this.httpService.put<void>(
      this.endpoints.job.changeStatus(jobId, statusId),
      null
    );
  }

  checkJobInvitations(jobId: GUID): Observable<boolean> {
    return this.httpService.get<boolean>(this.endpoints.job.checkJobInvitations(jobId));
  }

  submitForApproval(jobId: GUID, statusId: GUID): Observable<void> {
    return this.changeStatus(jobId, statusId);
  }

  getAll(
    pagination: PaginatedRequest = { pageNumber: 1, pageSize: 10 },
    filter?: JobQueryFilter
  ): Observable<PaginatedResult<JobResponse>> {
    const payload = { pagination, filter };
    return this.httpService.post<PaginatedResult<JobResponse>>(
      this.endpoints.job.searchJob,
      payload
    );
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

  mapResponseToJob(jobResponse: JobResponse): Job {
    return {
      jobTitleId: jobResponse.jobTitleId,
      rowVersion: jobResponse.rowVersion,
      titleAr: jobResponse.titleAr,
      titleEn: jobResponse.titleEn,
      jobNumber: jobResponse.jobNumber,
      sectorId: jobResponse.sector.id as GUID,
      managementId: jobResponse.management.id as GUID,
      departmentId: jobResponse.department?.id as GUID,
      yearsOfExperience: jobResponse.yearsOfExperience,
      jobCategoryId: jobResponse.jobCategory.id as GUID,
      workLocationId: jobResponse.workLocation.id as GUID,
      genderId: jobResponse.gender?.id as GUID,
      majorId: jobResponse.major?.id as GUID,
      subMajorId: jobResponse.subMajor?.id as GUID,
      workTypeId: jobResponse.workType.id as GUID,
      numberOfVacancies: jobResponse.numberOfVacancies,
      closingDate: new Date(jobResponse.closingDate),
      minimumAge: jobResponse.minimumAge,
      maximumAge: jobResponse.maximumAge,
      overviewAr: jobResponse.overViewAr || '',
      overviewEn: jobResponse.overViewEn || '',
      benefitsAr: jobResponse.benefitsAr || '',
      benefitsEn: jobResponse.benefitsEn || '',
      jobStatus: jobResponse.jobStatus || undefined,
      qualificationsDescriptionAr: jobResponse.qualificationDescriptionAr || '',
      qualificationsDescriptionEn: jobResponse.qualificationDescriptionEn || '',
      jobSpecializations: jobResponse.jobSpecializations?.map((s) => ({
        id: s.id as GUID,
        majorId: s.major.id as GUID,
        subMajorId: s.subMajor?.id as GUID,
        major: s.major,
        subMajor: s.subMajor
      })) || [],
      majorName: jobResponse.major?.name || jobResponse.major?.additionalData?.nameAr || '',
      subMajorName: jobResponse.subMajor?.name || jobResponse.subMajor?.additionalData?.nameAr || '',
      degrees: jobResponse.degrees.map((d) => ({ degreeId: d.degreeId })),
      conditions: jobResponse.conditions.map((c) => ({
        textAr: c.textAr,
        textEn: c.textEn,
      })),
      responsibilities: jobResponse.responsibilities.map((r) => ({
        textAr: r.textAr,
        textEn: r.textEn,
      })),
      skills: jobResponse.skills.map((s) => ({
        skillId: s.skillId,
        showToApplicants: s.showToApplicants,
        isRequired: s.isRequired,
      })),
      requiredAttachments: jobResponse.requiredAttachments.map((a) => ({
        titleAr: a.titleAr,
        titleEn: a.titleEn,
        isMandatory: a.isMandatory,
      })),
      tabReviewNotes: jobResponse.tabReviewNotes || undefined,
    };
  }

  getJobById(jobId: GUID): Observable<Job> {
    return this.getById(jobId).pipe(
      map(response => this.mapResponseToJob(response))
    );
  }

  loadJobForEdit(jobId: GUID): Observable<JobResponse> {
    return this.getById(jobId).pipe(
      tap((jobResponse) => {
        const job = this.mapResponseToJob(jobResponse);
        this.currentJob.set(job);
        this.currentJobId = jobId;
        this.jobStatus.set(jobResponse.jobStatus.backendName);
      })
    );
  }

  getLatestReview(jobId: GUID): Observable<JobReviewResponse> {
    return this.httpService.get<JobReviewResponse>(this.endpoints.job.getLatestReview(jobId));
  }
  GetJobsCountByStatus(jobStatusId: GUID): Observable<number> {
    const url = `${this.endpoints.job.CountByStatus(jobStatusId)}`;
    return this.httpService.get<number>(url);
  }
}
