import { inject, Injectable, signal } from '@angular/core';
import { forkJoin, Observable, of } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { GUID } from '../../../../../shared/types/guid.type';
import { dropdownOptionsModel, DropdownOptionVM } from '../../../../../shared/models/dropdown-options.model';
import { TranslateService } from '@ngx-translate/core';
import { HttpService } from "../../../../../core/http/http.service";
import { EndpointsService } from '../../../../../core/http/endpoints.service';
import { NotificationService } from '../../../../../core/services/notification.service';
import { JobCategory, JobStatus } from '../../../../../core/enums/lookups.enum';
import { CountryDto, CountryVM } from '../../countries-management/models/country.dto';

@Injectable({ providedIn: 'root' })
export class JobLookupService {
  private http = inject(HttpService);
  private endpoints = inject(EndpointsService);
  private notificationService = inject(NotificationService);
  private translationService = inject(TranslateService);

  private loading = signal<boolean>(false);
  public loaded = signal<boolean>(false);
  departments = signal<DropdownOptionVM[]>([]);
  majors = signal<DropdownOptionVM[]>([]);
  subMajors = signal<DropdownOptionVM[]>([]);
  degrees = signal<DropdownOptionVM[]>([]);
  workTypes = signal<DropdownOptionVM[]>([]);
  jobCategories = signal<DropdownOptionVM[]>([]);
  genders = signal<DropdownOptionVM[]>([]);
  workLocations = signal<DropdownOptionVM[]>([]);
  nationalities = signal<DropdownOptionVM[]>([]);
  jobStatus = signal<DropdownOptionVM[]>([]);
  jobInvitesStatus = signal<DropdownOptionVM[]>([]);
  candidateTypes = signal<DropdownOptionVM[]>([]);
  managements = signal<DropdownOptionVM[]>([]);
  sectors = signal<DropdownOptionVM[]>([]);
  skills = signal<DropdownOptionVM[]>([]);

  loadAll(): void {
    if (this.loading()) return;

    this.loading.set(true);

    forkJoin({
      sectors: this.http.get<DropdownOptionVM[]>(this.endpoints.job.lookups.sectors),
      majors: this.http.get<DropdownOptionVM[]>(this.endpoints.job.lookups.majors),
      degrees: this.http.get<DropdownOptionVM[]>(this.endpoints.job.lookups.degrees),
      workTypes: this.http.get<DropdownOptionVM[]>(this.endpoints.job.lookups.workTypes),
      jobCategories: this.http.get<DropdownOptionVM[]>(this.endpoints.job.lookups.jobCategories),
      genders: this.http.get<DropdownOptionVM[]>(this.endpoints.job.lookups.genders),
      targetEntities: this.http.get<DropdownOptionVM[]>(this.endpoints.job.lookups.targetEntities),
      nationalities: this.http.get<DropdownOptionVM[]>(this.endpoints.job.lookups.nationalities),
      jobStatus: this.http.get<DropdownOptionVM[]>(this.endpoints.job.lookups.jobStatus),
      jobInvitesStatus: this.http.get<DropdownOptionVM[]>(this.endpoints.job.lookups.jobInvitesStatus),
      candidateTypes: this.http.get<DropdownOptionVM[]>(this.endpoints.jobCandidates.lookups.candidateTypes),
    }).subscribe({
      next: (res) => {
        this.sectors.set(this.toVMs(res.sectors));
        this.majors.set(this.toVMs(res.majors));
        this.degrees.set(this.toVMs(res.degrees));
        this.workTypes.set(this.toVMs(res.workTypes));
        this.jobCategories.set(this.toVMs(res.jobCategories));
        this.genders.set(this.toVMs(res.genders));
        this.workLocations.set(this.toVMs(res.targetEntities));
        this.nationalities.set(this.toVMs(res.nationalities));
        this.jobStatus.set(this.toVMs(res.jobStatus));
        this.jobInvitesStatus.set(this.toVMs(res.jobInvitesStatus));
        this.candidateTypes.set(this.toVMs(res.candidateTypes));

        this.loaded.set(true);
        this.loading.set(false);
      },
      error: (err) => {
        this.loading.set(false);
      }
    });
  }

  getStatusIdByEnum(statusEnum: JobStatus): GUID {
    const status = this.jobStatus().find(item => item.backendName === statusEnum);
    return status?.id as GUID;
  }

  getStatusEnumById(statusId: GUID): JobStatus | undefined {
    const status = this.jobStatus().find(item => item.id === statusId);
    if (!status?.backendName) {
      return undefined;
    }
    return JobStatus[status.backendName as keyof typeof JobStatus];
  }

  getJobCategoryEnumById(categoryId: GUID): JobCategory | undefined {
    const category = this.jobCategories().find(item => item.id === categoryId);
    if (!category?.backendName) {
      return undefined;
    }
    return JobCategory[category.backendName as keyof typeof JobCategory];
  }

  isJobCategory(jobCategoryId: GUID, expectedCategory: JobCategory): boolean {
    const categoryEnum = this.getJobCategoryEnumById(jobCategoryId);
    return categoryEnum === expectedCategory;
  }
  loadSubMajorsByMajor(majorId: GUID): void {
    if (!majorId) {
      this.resetSubMajors();
      return;
    }

    this.http.get<DropdownOptionVM[]>(
      `${this.endpoints.job.lookups.subMajors}?majorId=${majorId}`
    ).subscribe({
      next: (subMajors) => this.subMajors.set(this.toVMs(subMajors)),
      error: () => {
        this.subMajors.set([]);
      }
    });
  }

  loadSkillsByMajor(majorId: GUID): void {
    if (!majorId) {
      this.resetSkills();
      return;
    }

    this.http.get<DropdownOptionVM[]>(
      `${this.endpoints.job.lookups.skills}?majorId=${majorId}`
    ).subscribe({
      next: (skills) => this.skills.set(this.toVMs(skills)),
      error: () => {
        this.skills.set([]);
      }
    });
  }

  loadManagementsBySector(sectorId: GUID): void {
    if (!sectorId) {
      this.resetManagements();
      return;
    }

    this.http.get<DropdownOptionVM[]>(
      `${this.endpoints.job.lookups.managements}?sectorId=${sectorId}`
    ).subscribe({
      next: (managements) => this.managements.set(this.toVMs(managements)),
      error: () => {
        this.managements.set([]);
      }
    });
  }

  loadDepartmentsByManagement(managementId: GUID): void {
    if (!managementId) {
      this.resetDepartments();
      return;
    }

    this.http.get<DropdownOptionVM[]>(
      `${this.endpoints.job.lookups.departments}?managementId=${managementId}`
    ).subscribe({
      next: (departments) => this.departments.set(this.toVMs(departments)),
      error: () => {
        this.departments.set([]);
      }
    });
  }

  loadJobStatus(): Observable<DropdownOptionVM[]> {
    return this.http.get<DropdownOptionVM[]>(
      this.endpoints.job.lookups.jobStatus
    ).pipe(
      tap(jobStatus => this.jobStatus.set(this.toVMs(jobStatus))),
      catchError(() => {
        this.jobStatus.set([]);
        return of([]);
      })
    );
  }

  loadJobCategories(): Observable<DropdownOptionVM[]> {
    return this.http.get<DropdownOptionVM[]>(
      this.endpoints.job.lookups.jobCategories
    ).pipe(
      tap(jobCategories => this.jobCategories.set(this.toVMs(jobCategories))),
      catchError(() => {
        this.jobCategories.set([]);
        return of([]);
      })
    );
  }

  loadCandidateTypes(): Observable<DropdownOptionVM[]> {
    return this.http.get<DropdownOptionVM[]>(
      this.endpoints.jobCandidates.lookups.candidateTypes
    ).pipe(
      tap(candidateTypes => this.candidateTypes.set(this.toVMs(candidateTypes))),
      catchError(() => {
        this.candidateTypes.set([]);
        return of([]);
      })
    );
  }

  loadNationalities(): Observable<DropdownOptionVM[]> {
    return this.http.get<DropdownOptionVM[]>(
      this.endpoints.job.lookups.nationalities
    ).pipe(
      tap(nationalities => this.nationalities.set(this.toVMs(nationalities))),
      catchError(() => {
        this.nationalities.set([]);
        return of([]);
      })
    );
  }

  loadGenders(): Observable<DropdownOptionVM[]> {
    return this.http.get<DropdownOptionVM[]>(
      this.endpoints.job.lookups.genders
    ).pipe(
      tap(genders => this.genders.set(this.toVMs(genders))),
      catchError(() => {
        this.genders.set([]);
        return of([]);
      })
    );
  }
  resetSubMajors(): void {
    this.subMajors.set([]);
  }

  resetSkills(): void {
    this.skills.set([]);
  }

  resetManagements(): void {
    this.managements.set([]);
  }

  resetDepartments(): void {
    this.departments.set([]);
  }

  clearCache(): void {
    this.loaded.set(false);
    this.departments.set([]);
    this.majors.set([]);
    this.subMajors.set([]);
    this.degrees.set([]);
    this.workTypes.set([]);
    this.jobCategories.set([]);
    this.genders.set([]);
    this.workLocations.set([]);
    this.nationalities.set([]);
    this.jobStatus.set([]);
    this.jobInvitesStatus.set([]);
    this.managements.set([]);
    this.sectors.set([]);
    this.skills.set([]);
  }

  private toVMs<T extends dropdownOptionsModel>(arr: T[]): DropdownOptionVM[] {
    return (arr ?? []).map(x => new DropdownOptionVM(x));
  }
  private toCountryVMs(arr: CountryDto[]): CountryVM[] {
    return (arr ?? []).map(x => new CountryVM(x));
  }
}
