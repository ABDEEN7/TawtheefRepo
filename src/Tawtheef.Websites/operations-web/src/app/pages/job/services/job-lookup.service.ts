import { inject, Injectable, signal } from '@angular/core';
import { forkJoin, Observable, of, BehaviorSubject } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { EndpointsService } from '../../../core/http/endpoints.service';
import { NotificationService } from '../../../core/services/notification.service';
import { GUID } from '../../../shared/types/guid.type';
import { HttpService } from '../../../core/http/http.service';
import { dropdownOptionsModel } from '../../../shared/models/dropdown-options.model';
import { Gender, JobCategory, JobStatus, WorkType } from '../../../core/enums/lookups.enum';
import { TranslateService } from '@ngx-translate/core';

@Injectable({ providedIn: 'root' })
export class JobLookupService {
  private http = inject(HttpService);
  private endpoints = inject(EndpointsService);
  private notificationService = inject(NotificationService);
  private translationService = inject(TranslateService);

  private loading = signal<boolean>(false);
  public loaded = signal<boolean>(false);
  private loadSubject = new BehaviorSubject<boolean>(false);

  departments = signal<dropdownOptionsModel[]>([]);
  majors = signal<dropdownOptionsModel[]>([]);
  subMajors = signal<dropdownOptionsModel[]>([]);
  degrees = signal<dropdownOptionsModel[]>([]);
  workTypes = signal<dropdownOptionsModel[]>([]);
  jobCategories = signal<dropdownOptionsModel[]>([]);
  genders = signal<dropdownOptionsModel[]>([]);
  workLocations = signal<dropdownOptionsModel[]>([]);
  nationalities = signal<dropdownOptionsModel[]>([]);
  jobStatus = signal<dropdownOptionsModel[]>([]);
  jobInvitesStatus = signal<dropdownOptionsModel[]>([]);
  managements = signal<dropdownOptionsModel[]>([]);
  sectors = signal<dropdownOptionsModel[]>([]);
  skills = signal<dropdownOptionsModel[]>([]);

  loaded$ = this.loadSubject.asObservable();

  loadAll(): void {
    if (this.loaded() || this.loading()) return;
    
    this.loading.set(true);

    forkJoin({
      sectors: this.http.get<dropdownOptionsModel[]>(this.endpoints.job.lookups.sectors),
      majors: this.http.get<dropdownOptionsModel[]>(this.endpoints.job.lookups.majors),
      degrees: this.http.get<dropdownOptionsModel[]>(this.endpoints.job.lookups.degrees),
      workTypes: this.http.get<dropdownOptionsModel[]>(this.endpoints.job.lookups.workTypes),
      jobCategories: this.http.get<dropdownOptionsModel[]>(this.endpoints.job.lookups.jobCategories),
      genders: this.http.get<dropdownOptionsModel[]>(this.endpoints.job.lookups.genders),
      targetEntities: this.http.get<dropdownOptionsModel[]>(this.endpoints.job.lookups.targetEntities),
      nationalities: this.http.get<dropdownOptionsModel[]>(this.endpoints.job.lookups.nationalities),
      jobStatus: this.http.get<dropdownOptionsModel[]>(this.endpoints.job.lookups.jobStatus),
      jobInvitesStatus: this.http.get<dropdownOptionsModel[]>(this.endpoints.job.lookups.jobInvitesStatus),
    }).subscribe({
      next: (res) => {
        this.sectors.set(res.sectors);
        this.majors.set(res.majors);
        this.degrees.set(res.degrees);
        this.workTypes.set(res.workTypes);
        this.jobCategories.set(res.jobCategories);
        this.genders.set(res.genders);
        this.workLocations.set(res.targetEntities);
        this.nationalities.set(res.nationalities);
        this.jobStatus.set(res.jobStatus);
        this.jobInvitesStatus.set(res.jobInvitesStatus);
        
        this.loaded.set(true);
        this.loading.set(false);
        this.loadSubject.next(true);
      },
      error: (err) => {
        const errorMessage = this.translationService.instant('lookups.load_failed');
        this.notificationService.error(errorMessage);
        this.loading.set(false);
        this.loadSubject.error(err);
      }
    });
  }

  loadAllAndWait(): Observable<boolean> {
    if (this.loaded()) {
      return of(true);
    }
    
    this.loadAll();
    return this.loaded$.pipe(
      catchError(() => of(false))
    );
  }

  getStatusIdByEnum(statusEnum: JobStatus): GUID  {
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

  getStatusDisplayName(statusId: GUID): string {
    const status = this.jobStatus().find(item => item.id === statusId);
    return status?.name || this.translationService.instant('common.unknown');
  }

  getJobCategoryIdByEnum(categoryEnum: JobCategory): GUID | undefined {
    const category = this.jobCategories().find(item => item.backendName === categoryEnum);
    return category?.id as GUID;
  }

  getJobCategoryEnumById(categoryId: GUID): JobCategory | undefined {
    const category = this.jobCategories().find(item => item.id === categoryId);
    if (!category?.backendName) {
      return undefined;
    }
    return JobCategory[category.backendName as keyof typeof JobCategory];
  }

  getWorkTypeIdByEnum(workTypeEnum: WorkType): GUID | undefined {
    const workType = this.workTypes().find(item => item.backendName === workTypeEnum);
    return workType?.id as GUID;
  }

  getGenderIdByEnum(genderEnum: Gender): GUID | undefined {
    const gender = this.genders().find(item => item.backendName === genderEnum);
    return gender?.id as GUID;
  }

  isJobCategory(jobCategoryId: GUID, expectedCategory: JobCategory): boolean {
    const categoryEnum = this.getJobCategoryEnumById(jobCategoryId);
    return categoryEnum === expectedCategory;
  }

  isAcademicJob(jobCategoryId: GUID): boolean {
    return this.isJobCategory(jobCategoryId, JobCategory.Academic);
  }

  isAdministrativeJob(jobCategoryId: GUID): boolean {
    return this.isJobCategory(jobCategoryId, JobCategory.Administrative);
  }

  isLaborJob(jobCategoryId: GUID): boolean {
    return this.isJobCategory(jobCategoryId, JobCategory.Labor);
  }

  loadSubMajorsByMajor(majorId: GUID): void {
    if (!majorId) {
      this.resetSubMajors();
      return;
    }
    
    this.http.get<dropdownOptionsModel[]>(
      `${this.endpoints.job.lookups.subMajors}?majorId=${majorId}`
    ).subscribe({
      next: (subMajors) => this.subMajors.set(subMajors),
      error: (err) => {
        const errorMessage = this.translationService.instant('lookups.sub_majors_load_failed');
        this.notificationService.error(errorMessage);
        this.subMajors.set([]);
      }
    });
  }

  loadSkillsByMajor(majorId: GUID): void {
    if (!majorId) {
      this.resetSkills();
      return;
    }
    
    this.http.get<dropdownOptionsModel[]>(
      `${this.endpoints.job.lookups.skills}?majorId=${majorId}`
    ).subscribe({
      next: (skills) => this.skills.set(skills),
      error: (err) => {
        const errorMessage = this.translationService.instant('lookups.skills_load_failed');
        this.notificationService.error(errorMessage);
        this.skills.set([]);
      }
    });
  }

  loadManagementsBySector(sectorId: GUID): void {
    if (!sectorId) {
      this.resetManagements();
      return;
    }
    
    this.http.get<dropdownOptionsModel[]>(
      `${this.endpoints.job.lookups.managements}?sectorId=${sectorId}`
    ).subscribe({
      next: (managements) => this.managements.set(managements),
      error: (err) => {
        const errorMessage = this.translationService.instant('lookups.managements_load_failed');
        this.notificationService.error(errorMessage);
        this.managements.set([]);
      }
    });
  }

  loadDepartmentsByManagement(managementId: GUID): void {
    if (!managementId) {
      this.resetDepartments();
      return;
    }
    
    this.http.get<dropdownOptionsModel[]>(
      `${this.endpoints.job.lookups.departments}?managementId=${managementId}`
    ).subscribe({
      next: (departments) => this.departments.set(departments),
      error: (err) => {
        const errorMessage = this.translationService.instant('lookups.departments_load_failed');
        this.notificationService.error(errorMessage);
        this.departments.set([]);
      }
    });
  }

  loadJobStatus()
  {
    this.http.get<dropdownOptionsModel[]>(
      `${this.endpoints.job.lookups.jobStatus}`
    ).subscribe({
      next: (jobStatus) => this.jobStatus.set(jobStatus),
      error: (err) => {
        this.jobStatus.set([]);
      }
    });
  }

  loadJobCategories()
  {
    this.http.get<dropdownOptionsModel[]>(
      `${this.endpoints.job.lookups.jobCategories}`
    ).subscribe({
      next: (cat) => this.jobCategories.set(cat),
      error: (err) => {
        this.jobCategories.set([]);
      }
    });
  }
  

  getJobCategoryLabel(id: GUID): string {
    return this.jobCategories().find(jobcat => jobcat.id === id)?.name || '';
  }

  getWorkLocationName(id: GUID | undefined): string {
    if (!id) return '';
    return this.workLocations().find(en => en.id === id)?.name || '';
  }

   getNationalitiesName(id: GUID | undefined): string {
    if (!id) return '';
    return this.nationalities().find(en => en.id === id)?.name || '';
  }

  getWorkTypeName(id: GUID | undefined): string {
    if (!id) return '';
    return this.workTypes().find(wt => wt.id === id)?.name || '';
  }

  getMajorName(id: GUID): string {
    return this.majors().find(ma => ma.id === id)?.name || '';
  }

  getSubMajorName(id: GUID | undefined): string {
    if (!id) return '';
    return this.subMajors().find(sm => sm.id === id)?.name || '';
  }

  getDegreeNames(ids: GUID[]): string {
    if (!ids || ids.length === 0) return '';
    
    const names: string[] = [];
    ids.forEach(id => {
      const name = this.degrees().find(degree => degree.id === id)?.name;
      if (name) names.push(name);
    });
    return names.join(', ') || this.translationService.instant('common.not_specified');
  }

  getNationalityName(natId: GUID | undefined): string {
    if (!natId) return this.translationService.instant('common.not_specified');
    return this.nationalities().find((country) => country?.id === natId)?.name || this.translationService.instant('common.unknown');
  }

  getStatusName(statusId: GUID | undefined): string {
    if (!statusId) return this.translationService.instant('common.not_specified');
    return this.jobStatus().find((jobStatus) => jobStatus?.id === statusId)?.name || this.translationService.instant('common.unknown');
  }

  getManagementName(id: GUID | undefined): string {
    if (!id) return '';
    return this.managements().find(m => m.id === id)?.name || '';
  }

  getDepartmentName(id: GUID | undefined): string {
    if (!id) return '';
    return this.departments().find(d => d.id === id)?.name || '';
  }

  getSectorName(id: GUID | undefined): string {
    if (!id) return '';
    return this.sectors().find(s => s.id === id)?.name || '';
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
    this.loadSubject.next(false);
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
}