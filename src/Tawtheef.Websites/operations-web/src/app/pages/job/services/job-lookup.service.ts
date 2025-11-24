import { inject, Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { forkJoin } from 'rxjs';
import { Lookups } from '../../../core/models/lookups.model';
import { EndpointsService } from '../../../core/http/endpoints.service';
import { NotificationService } from '../../../core/services/notification.service';
import { GUID } from '../../../shared/types/guid.type';

@Injectable({ providedIn: 'root' })
export class JobLookupService {
 private http = inject(HttpClient);
  private endpoints = inject(EndpointsService);
  private notificationService = inject(NotificationService);

  private loading = signal<boolean>(false);
  private loaded = signal<boolean>(false);

  departments = signal<Lookups[]>([]);
  majors = signal<Lookups[]>([]);
  degrees = signal<Lookups[]>([]);
  workTypes = signal<Lookups[]>([]);
  jobCategories = signal<Lookups[]>([]);
  genders = signal<Lookups[]>([]);
  workLocations = signal<Lookups[]>([]);
  nationalities = signal<Lookups[]>([]);
  jobStatus = signal<Lookups[]>([]);
  jobInvitesStatus = signal<Lookups[]>([]);
  loadAll() {
    if (this.loaded()) return;
    this.loading.set(true);

    forkJoin({
      departments: this.http.get<Lookups[]>(this.endpoints.job.lookups.departments),
      majors: this.http.get<Lookups[]>(this.endpoints.job.lookups.majors),
      degrees: this.http.get<Lookups[]>(this.endpoints.job.lookups.degrees),
      workTypes: this.http.get<Lookups[]>(this.endpoints.job.lookups.workTypes),
      jobCategories: this.http.get<Lookups[]>(this.endpoints.job.lookups.jobCategories),
      genders: this.http.get<Lookups[]>(this.endpoints.job.lookups.genders),
      targetEntities: this.http.get<Lookups[]>(this.endpoints.job.lookups.targetEntities),
      nationalities: this.http.get<Lookups[]>(this.endpoints.job.lookups.nationalities),
      jobStatus : this.http.get<Lookups[]>(this.endpoints.job.lookups.jobStatus),
      jobInvitesStatus : this.http.get<Lookups[]>(this.endpoints.job.lookups.jobInvitesStatus)
    }).subscribe({
      next: (res) => {
        this.departments.set([...res.departments]);
        this.majors.set([...res.majors]);
        this.degrees.set([...res.degrees]);
        this.workTypes.set([...res.workTypes]);
        this.jobCategories.set([...res.jobCategories]);
        this.genders.set([...res.genders]);
        this.workLocations.set([...res.targetEntities]);
        this.nationalities.set([...res.nationalities]);
        this.jobStatus.set([...res.jobStatus]);
        this.jobInvitesStatus.set([...res.jobInvitesStatus])
        this.loaded.set(true);
        this.loading.set(false);
      },
      error: (err) => {
        this.notificationService.error(err)
        this.loading.set(false);
      }
    });
  }

  getRequestingDepartment(id : string) : string{
   return this.departments().find(dept => dept.id == id)?.name || ''
  }
  getJobCategoryLable(id : string) : string{
   return this.jobCategories().find(jobcat => jobcat.id == id)?.name || ''
  }

  getGender(ids : string[]) : string{
    let splitedNames : string[] = []
    ids.forEach(id =>{splitedNames.push(this.genders().find(gender =>gender.id == id)?.name || '')})
    return splitedNames.join(',')
  }
  getWorkLocations(id : string | undefined) :string{
       return this.workLocations().find(en => en.id == id)?.name || ''
  }

    getTypeOfWork(id : string | undefined) :string{
       return this.workTypes().find(wt => wt.id == id)?.name || ''
  }
   getMajor(id : string) :string{
       return this.majors().find(ma => ma.id == id)?.name  || ''
  }
  getDegrees(ids:GUID[]): string {
    let splitedNames : string[] = []
    ids.forEach(id =>{splitedNames.push(this.degrees().find(degree =>degree.id == id)?.name || '')})
    return splitedNames.join(',')
  }

    getNationalites(natId:string){
    return this.nationalities().find((country) => country?.id == natId)?.name || ''
  }

  getStatus(statusId: string | undefined){
    return this.jobStatus().find((jobStatus) => jobStatus?.id == statusId)?.name || ''
  }

  getJobInitesStatus(inviteStatusId : string | undefined){
    return this.jobInvitesStatus().find((inviteStatus)=>inviteStatus?.id == inviteStatusId)?.name || ''
  }
}