import {Injectable, inject, signal} from '@angular/core';
import {tap} from 'rxjs/operators';
import {Observable, of} from 'rxjs';
import {Job} from '../models/job.model';
import {JobBasics} from '../models/job-basics.models';
import {PointsConfig} from '../models/points-config.model';
import {HttpService} from '../../../core/http/http.service';
import {GUID} from '../../../shared/types/guid.type';
import {EndpointsService} from '../../../core/http/endpoints.service';
import {NotificationService} from '../../../core/services/notification.service';
import {JobQuota} from '../models/job-quotas.models';
import {JobResponseDto} from '../models/job-response-model';
import {PaginatedResult} from '../../../core/models/paginated-result.model';
import {PaginationMetadata} from '../../../core/models/pagination-metadata.model';
import {PaginatedRequest} from '../../../core/models/paginated-request.model';
import {JobQueryFilter} from '../models/job-query-filter.model';
import {JobCondition} from '../models/job-condition.model';
import {JobSkill} from '../models/job-skill.model';
import {JobDegree} from '../models/job-degree.model';
import {JobResponsibility} from '../models/job-responsibility.model';
import {RequiredAttachment} from '../models/required-attachment.model';
import { text } from '@primeuix/themes/aura/inlinemessage';

@Injectable({
  providedIn: 'root'
})
export class JobService {
  private httpService = inject(HttpService);
  private endpoints = inject(EndpointsService);
  private notificationService = inject(NotificationService);

  private _jobs = signal<JobResponseDto[]>([]);
  private _newJob = signal<Job>(this.createEmptyJob());

  private _currentJob = signal<JobResponseDto | null>(null);
  private _paginationMetadata = signal<PaginationMetadata | null>(null);

  public jobs = this._jobs.asReadonly();
  public currentJob = this._currentJob.asReadonly();
  public newJob = this._newJob.asReadonly();
  public paginationMetadata = this._paginationMetadata.asReadonly();

  loadJobs(pagination?: PaginatedRequest, filter?: JobQueryFilter): void {
    const queryParams: any = {};

    if (pagination) {
      if (pagination.pageNumber !== undefined) queryParams['Pagination.PageNumber'] = pagination.pageNumber;
      if (pagination.pageSize !== undefined) queryParams['Pagination.PageSize'] = pagination.pageSize;
      if (pagination.sortBy) queryParams['Pagination.SortBy'] = pagination.sortBy;
      if (pagination.sortDirection) queryParams['Pagination.SortDirection'] = pagination.sortDirection;
    }

    // Add filter parameters
    if (filter) {
      if (filter.searchTerm) queryParams['Filter.SearchTerm'] = filter.searchTerm;
      if (filter.departmentId) queryParams['Filter.DepartmentId'] = filter.departmentId;
      if (filter.statusId) queryParams['Filter.StatusId'] = filter.statusId;
      if (filter.jobCategoryId) queryParams['Filter.JobCategoryId'] = filter.jobCategoryId;
      if (filter.workTypeId) queryParams['Filter.WorkTypeId'] = filter.workTypeId;
      if (filter.deadlineFrom) queryParams['Filter.DeadlineFrom'] = filter.deadlineFrom;
      if (filter.deadlineTo) queryParams['Filter.DeadlineTo'] = filter.deadlineTo;
      if (filter.minVacancies !== undefined) queryParams['Filter.MinVacancies'] = filter.minVacancies;
      if (filter.maxVacancies !== undefined) queryParams['Filter.MaxVacancies'] = filter.maxVacancies;
    }

    this.httpService.get<PaginatedResult<JobResponseDto>>(this.endpoints.job.job, queryParams).pipe(
      tap(response => {
        this._jobs.set(response.items || []);
        this._paginationMetadata.set(response.metadata);
      }),
    ).subscribe();
  }

  loadJob(id: GUID): Observable<JobResponseDto> {
    return this.httpService.get<JobResponseDto>(`${this.endpoints.job.job}/${id}`).pipe(
      tap(job => {
        this._currentJob.set(job);
        this.setJobForEdit(job);
      }),
    );
  }

  // ==================== SAVE OPERATIONS ====================
  saveJob(job: Job): Observable<any> {
    // Prepare the request body according to Swagger spec
    const requestBody = {
      job: {
        // Basic Information
        title: job.title,
        vacancies: job.vacancies,
        deadline: job.deadline ? job.deadline.toISOString() : null,
        description: job.description,
        benefits: job.benefits,
        overview: job.overview || '',
        qualificationsDescription: job.qualificationsDescription || '',
        publishAt: job.publishAt ? job.publishAt.toISOString() : null,

        // Requirements
        minimumExperienceYears: job.minimumExperienceYears,
        minimumAge: job.minimumAge,
        maximumAge: job.maximumAge,

        // Foreign keys - ensure they're valid GUIDs (not empty strings)
        sectorId: job.sectorId || null,
        managementId: job.managementId || null,
        requestingDepartmentId: job.requestingDepartmentId || null,
        jobCategoryId: job.jobCategoryId || null,
        genderId: job.genderId || null,
        workLocationId: job.workLocationId || null,
        majorId: job.majorId || null,
        subMajorId: job.subMajorId || null,
        workTypeId: job.workTypeId || null,
        statusId: job.statusId, // This can be null for new jobs

        // Quota
        quota: {
          qatariCitizens: job.quota.qatariCitizens || 0,
          qatarMother: job.quota.qatarMother || 0,
          nonQatariSpouse: job.quota.nonQatariSpouse || 0,
          gcc: job.quota.gcc || 0,
          quGrads: job.quota.quGrads || 0,
          residents: job.quota.residents || 0,
          residentsBreakdowns: (job.quota.residentsBreakdowns || []).map(item => ({
            nationalityId: item.nationalityId || null,
            percentage: item.percentage || 0
          }))
        },

        // Collections
        degrees: (job.degrees || []).map(deg => ({
          degreeId: deg.degreeId || null
        })),

        conditions: (job.conditions || []).map((cond, index) => ({
          text: cond.text,
        })),

        skills: (job.skills || []).map(skill => ({
          skillId: skill.skillId || null,
          showToApplicants: skill.showToApplicants !== undefined ? skill.showToApplicants : true
        })),

        responsibilities: (job.responsibilities || []).map((resp, index) => ({
          text: resp.text,
        })),

        requiredAttachments: (job.requiredAttachments || []).map(att => ({
          title: att.title || '',
          isMandatory: att.isMandatory !== undefined ? att.isMandatory : true
        }))
      }
    };

    console.log('Saving job with data:', requestBody);

    const operation = job.id
      ? this.httpService.put(`${this.endpoints.job.job}/${job.id}`, requestBody)
      : this.httpService.post(this.endpoints.job.job, requestBody);

    return operation;
  }

  saveJobPointsConfig(jobId: GUID, pointsConfig: PointsConfig): Observable<JobResponseDto | null> {
    const currentJob = this._currentJob();
    if (currentJob?.id !== jobId) return of(null);

    const updatedJob = {...currentJob, pointsConfig};
    return this.httpService.put<JobResponseDto>(this.endpoints.job.job + `/${jobId}`, updatedJob).pipe(
      tap(savedJob => this._currentJob.set(savedJob))
    );
  }

  // ==================== UPDATE OPERATIONS ====================
  updateCurrentJobBasics(updatedJob: Partial<Job>): void {
    this._newJob.update(job => ({
      ...job,
      requestingDepartmentId: updatedJob.requestingDepartmentId || job.requestingDepartmentId,
      title: updatedJob.title || job.title,
      jobCategoryId: updatedJob.jobCategoryId || job.jobCategoryId,
      genderId: updatedJob.genderId || job.genderId,
      workLocationId: updatedJob.workLocationId || job.workLocationId,
      majorId: updatedJob.majorId || job.majorId,
      workTypeId: updatedJob.workTypeId || job.workTypeId,
      vacancies: updatedJob.vacancies ?? job.vacancies,
      deadline: updatedJob.deadline ?? job.deadline,
      sectorId: updatedJob.sectorId || job.sectorId,
      managementId: updatedJob.managementId || job.managementId,
      overview: updatedJob.overview || job.overview,
      qualificationsDescription: updatedJob.qualificationsDescription || job.qualificationsDescription,
      publishAt: updatedJob.publishAt ?? job.publishAt,
      minimumAge: updatedJob.minimumAge ?? job.minimumAge,
      maximumAge: updatedJob.maximumAge ?? job.maximumAge,
      minimumExperienceYears: updatedJob.minimumExperienceYears ?? job.minimumExperienceYears
    }));
  }

  updateCurrentJobQuota(quotas: Partial<JobQuota>): void {
    this._newJob.update(job => ({
      ...job,
      quota: {...job.quota, ...quotas}
    }));
  }

  updateCurrentJobConditions(conditions: JobCondition[]): void {
    this._newJob.update(job => ({...job, conditions}));
  }

  updateCurrentJobResponsibilities(responsibilities: JobResponsibility[]): void {
    this._newJob.update(job => ({...job, responsibilities}));
  }

  updateCurrentJobSkills(skills: JobSkill[]): void {
    this._newJob.update(job => ({...job, skills}));
  }

  updateCurrentJobDescription(description: string): void {
    this._newJob.update(job => ({...job, description}));
  }

  updateCurrentJobBenefits(benefits: string): void {
    this._newJob.update(job => ({...job, benefits}));
  }

 updateCurrentJobAttachments(requiredAttachments: RequiredAttachment[]) {
  this._newJob.update(job => ({...job, requiredAttachments}))
}

  updateCurrentJobOverView(overview: string): void {
    this._newJob.update(job => ({...job, overview}));
  }

  updateCurrentJobQualifications(degrees: JobDegree[], description: string): void {
  this._newJob.update(job=>({...job,degrees,qualificationsDescription:description}))
  }

  setJobForEdit(job: JobResponseDto): void {
    const editJob: Job = {
      id: job.id,
      requestingDepartmentId: job.department?.id || '' as GUID,
      title: job.title,
      jobCategoryId: job.jobCategory?.id || '' as GUID,
      genderId: job.gender?.id || '' as GUID,
      workLocationId: job.workLocation?.id || '' as GUID,
      majorId: job.major?.id || '' as GUID,
      workTypeId: job.workType?.id || '' as GUID,
      statusId: job.status?.id || null,
      vacancies: job.vacancies,
      deadline: job.deadline ? new Date(job.deadline) : null,
      description: job.description,
      benefits: job.benefits,
      overview: job.overview,
      qualificationsDescription: job.qualificationsDescription,
      publishAt: job.publishAt ? new Date(job.publishAt) : null,
      conditions: (job.conditions || []).map(cond => ({
        text: cond.text || '',
      })),
      skills: (job.skills || []).map(skill => ({
        skillId: skill.skill?.skill.id || '' as GUID,
        showToApplicants: skill.showToApplicants
      })),
      degrees: job.degrees ? job.degrees.map(deg => ({
        degreeId: deg.degree?.id || '' as GUID,
      })) : [],
      quota: job.quota ? {
        qatariCitizens: job.quota.qatariCitizens || 0,
        qatarMother: job.quota.qatarMother || 0,
        nonQatariSpouse: job.quota.nonQatariSpouse || 0,
        gcc: job.quota.gcc || 0,
        quGrads: job.quota.quGrads || 0,
        residents: job.quota.residents || 0,
        residentsBreakdowns: (job.quota.residentsBreakdowns || []).map(item => ({
          nationalityId: item.nationality?.id || '' as GUID,
          percentage: item.percentage
        }))
      } : this.createEmptyJob().quota,
      minimumExperienceYears: job.minimumExperienceYears,
      minimumAge: job.minimumAge,
      maximumAge: job.maximumAge,
      sectorId: job.sector?.id || '' as GUID,
      managementId: job.management?.id || '' as GUID,
      subMajorId: job.subMajor?.id || '' as GUID,
      responsibilities: (job.responsibilities || []).map(resp => ({
        text: resp.text || '',
      })),
      requiredAttachments: (job.requiredAttachments || []).map(att => ({
        title: att.title,
        isMandatory: att.isMandatory
      }))
    };

    this._newJob.set(editJob);
  }

  private createEmptyJob(): Job {
    return {
      quota: {
        qatariCitizens: 0,
        qatarMother: 0,
        nonQatariSpouse: 0,
        gcc: 0,
        quGrads: 0,
        residents: 0,
        residentsBreakdowns: []
      },
      conditions: [],
      skills: [],
      degrees: [],
      description: '',
      benefits: '',
      title: '',
      vacancies: 0,
      deadline: null,
      overview: '',
      qualificationsDescription: '',
      publishAt: null,
      minimumExperienceYears: 0,
      minimumAge: 0,
      maximumAge: 0,
      requestingDepartmentId: '' as GUID,
      sectorId: '' as GUID,
      managementId: '' as GUID,
      jobCategoryId: '' as GUID,
      workLocationId: '' as GUID,
      majorId: '' as GUID,
      subMajorId: '' as GUID,
      workTypeId: '' as GUID,
      statusId: null,
      genderId: '' as GUID,
      responsibilities: [],
      requiredAttachments: []
    };
  }
}