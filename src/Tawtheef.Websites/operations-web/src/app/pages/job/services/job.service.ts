import {Injectable, inject, signal} from '@angular/core';
import {tap} from 'rxjs/operators';
import {Observable, of} from 'rxjs';
import {Job} from '../models/job.model';
import {JobBasics} from '../models/job-basics.models';
import {PointsConfig} from '../models/points-config.model';
import {HttpService} from '../../../core/http/http.service';
import { GUID } from '../../../shared/types/guid.type';
import { EndpointsService } from '../../../core/http/endpoints.service';
import { NotificationService } from '../../../core/services/notification.service';
import { JobQuota } from '../models/job-quotas.models';
import { JobResponseDto } from '../models/job-response-Dto';
import { PaginatedResult } from '../../../core/models/paginated-result.model';
import { PaginationMetadata } from '../../../core/models/pagination-metadata.model';
import { PaginatedRequest } from '../../../core/models/paginated-request.model';
import { JobQueryFilter } from '../models/job-query-filter.model';

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
        this._currentJob.set(job)
        this.setJobForEdit(job);
      }),
    );
  }

  // ==================== SAVE OPERATIONS ====================
  saveJob(job: Job): Observable<Job> {
    const command = {
    job: job
  };
    const operation = job.id
      ? this.httpService.put<Job>(this.endpoints.job.job, command)
      : this.httpService.post<Job>(this.endpoints.job.job, command);

    return operation;
  }

  saveJobPointsConfig(jobId: GUID, pointsConfig: PointsConfig): Observable<JobResponseDto | null> {
    const currentJob = this._currentJob();
    if (currentJob?.id !== jobId) return of(null);

    const updatedJob = {...currentJob, pointsConfig};
    return this.httpService.put<JobResponseDto>(this.endpoints.job.job +`/${jobId}`, updatedJob).pipe(
      tap(savedJob => this._currentJob.set(savedJob))
    );
  }

  // ==================== UPDATE OPERATIONS ====================
  updateCurrentJobBasics(updatedJob: Partial<JobBasics>): void {
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
      degreeIds : updatedJob.degreeIds || job.degreeIds
    }));
  }

  updateCurrentJobQuota(quotas: Partial<JobQuota>): void {
    this._newJob.update(job => ({
      ...job,
      quota: {...job.quota, ...quotas,}
    }));
  }

  updateCurrentJobConditions(conditions: string[]): void {
    this._newJob.update(job => ({...job, conditions}));
  }

  updateCurrentJobSkills(skills: string[]): void {
    this._newJob.update(job => ({...job, skills}));
  }

  updateCurrentJobDescription(description: string, benefits: string): void {
    this._newJob.update(job => ({...job, description, benefits}));
  }

  setJobForEdit(job: JobResponseDto): void {
    // Convert JobResponseDto to Job model for editing
    const editJob: Job = {
      id: job.id,
      requestingDepartmentId: job.requestingDepartment?.id || '' as GUID,
      title: job.title,
      jobCategoryId: job.jobCategory?.id || '' as GUID,
      genderId: job.gender?.id || '' as GUID,
      workLocationId: job.workLocation?.id || '' as GUID,
      majorId: job.major?.id || '' as GUID,
      workTypeId: job.workType?.id || '' as GUID,
      statusId : job.status?.id || '' as GUID,
      vacancies: job.vacancies,
      deadline: job.deadline ? new Date(job.deadline) : null,
      description: job.description,
      benefits: job.benefits,
      conditions: job.conditions || [],
      skills: job.skills || [],
      degreeIds: job.degrees?.map(d => d.id) || [],
      quota: job.quota ? {
        qatariCitizens: job.quota.qatariCitizens || 0,
        qatarMother: job.quota.qatarMother || 0,
        nonQatariSpouse: job.quota.nonQatariSpouse || 0,
        gcc: job.quota.gcc || 0,
        quGrads: job.quota.quGrads || 0,
        residents: job.quota.residents || 0,
        residentsBreakdowns: (job.quota.residentsBreakdowns || []).map(item => ({
        nationalityId: item.nationality?.id || '' as GUID,
        percentage : item.percentage
      }))
    } : this.createEmptyJob().quota
    };
    
    this._newJob.set(editJob);
  }

  private createEmptyJob(): Job {
    return {

      requestingDepartmentId: '' as GUID,
      title: '',
      jobCategoryId: '' as GUID,
      genderId: '' as GUID,
      workLocationId: '' as GUID,
      majorId: '' as GUID,
      workTypeId: '' as GUID,
      statusId:'' as GUID,
      vacancies: 0,
      deadline: null,
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
      degreeIds: [],
      description: '',
      benefits: ''
    };
  }
}
