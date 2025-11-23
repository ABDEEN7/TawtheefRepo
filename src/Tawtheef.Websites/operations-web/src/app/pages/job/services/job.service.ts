import {Injectable, inject, signal} from '@angular/core';
import {tap} from 'rxjs/operators';
import {Observable, of} from 'rxjs';
import {Job} from '../models/job.model';
import {JobBasics} from '../models/job-basics.models';
import {JobQuotas} from '../models/job-quotas.models';
import {PointsConfig} from '../models/points-config.model';
import {HttpService} from '../../../core/http/http.service';
import { GUID } from '../../../shared/types/guid.type';
import { EndpointsService } from '../../../core/http/endpoints.service';
import { NotificationService } from '../../../core/services/notification.service';

@Injectable({
  providedIn: 'root'
})
export class JobService {
  private httpService = inject(HttpService);
  private endpoints = inject(EndpointsService);
  private notificationService = inject(NotificationService);

  private _jobs = signal<Job[]>([]);
  private _currentJob = signal<Job>(this.createEmptyJob());

  // Public read-only signals
  public jobs = this._jobs.asReadonly();
  public currentJob = this._currentJob.asReadonly();

  // ==================== LOAD OPERATIONS ====================
  loadJobs(): void {
     this.httpService.get<Job[]>(this.endpoints.job.job).pipe(
      tap(jobs => this._jobs.set(jobs)),
    ).subscribe();
  }

  loadJob(id: GUID): Observable<Job> {
    if (!id) return of(this.createEmptyJob());

    return this.httpService.get<Job>(`job/${id}`).pipe(
      tap(job => this._currentJob.set(job)),
    );
  }

  // ==================== SAVE OPERATIONS ====================
  saveJob(job: Job): Observable<Job> {
    const command = {
    job: job
  };
    const operation = job.id
      ? this.httpService.put<Job>(this.endpoints.job.job + `/${job.id}`, command)
      : this.httpService.post<Job>(this.endpoints.job.job, command);

    return operation.pipe(
      tap(savedJob => this.updateJobInState(savedJob)),
    );
  }

  saveJobPointsConfig(jobId: GUID, pointsConfig: PointsConfig): Observable<Job> {
    const currentJob = this._currentJob();
    if (currentJob.id !== jobId) return of(currentJob);

    const updatedJob = {...currentJob, pointsConfig};
    return this.httpService.put<Job>(this.endpoints.job.job +`/${jobId}`, updatedJob).pipe(
      tap(savedJob => this._currentJob.set(savedJob))
    );
  }

  // ==================== UPDATE OPERATIONS ====================
  updateCurrentJobBasics(basics: Partial<JobBasics>,degreeIds : GUID[]): void {
    this._currentJob.update(job => ({
      ...job,
      degreeIds:[...degreeIds],
      basics: {...job.basics, ...basics}
    }));
  }

  updateCurrentJobQuotas(quotas: Partial<JobQuotas>): void {
    this._currentJob.update(job => ({
      ...job,
      quotas: {...job.quotas, ...quotas}
    }));
  }

  updateCurrentJobConditions(conditions: string[]): void {
    this._currentJob.update(job => ({...job, conditions}));
  }

  updateCurrentJobSkills(skills: string[]): void {
    this._currentJob.update(job => ({...job, skills}));
  }

  updateCurrentJobDescription(description: string, benefits: string): void {
    this._currentJob.update(job => ({...job, description, benefits}));
  }

  private updateJobInState(savedJob: Job): void {
    const currentJobs = this._jobs();

    if (savedJob.id) {
      const jobExists = currentJobs.some(j => j.id === savedJob.id);
      const updatedJobs = jobExists
        ? currentJobs.map(j => j.id === savedJob.id ? savedJob : j)
        : [...currentJobs, savedJob];

      this._jobs.set(updatedJobs);
    } else {
      this._jobs.set([...currentJobs, savedJob]);
    }
    this._currentJob.set(savedJob);
  }

  private createEmptyJob(): Job {
    return {
      basics: {
        requestingDeptId: '' as GUID,
        title: '',
        jobCategoryId: '' as GUID,
        genderId: '' as GUID,
        entityId: '' as GUID,
        majorId: '' as GUID,
        workTypeId: '' as GUID,
        vacancies: 0,
        deadline: null
      },
      quotas: {
        qatariCitizens: 0,
        qatarMother: 0,
        nonQatariSpouse: 0,
        gcc: 0,
        quGrads: 0,
        residents: 0,
        residentsBreakdown: []
      },
      conditions: [],
      skills: [],
      degreeIds: [],
      description: '',
      benefits: ''
    };
  }
}
