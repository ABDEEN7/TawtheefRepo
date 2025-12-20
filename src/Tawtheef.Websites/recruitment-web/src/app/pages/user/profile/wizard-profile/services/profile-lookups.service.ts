import { inject, Injectable, signal } from '@angular/core';
import {forkJoin, Observable} from 'rxjs';
import {catchError, map} from 'rxjs/operators';
import {EndpointsService} from '../../../../../core/http/endpoints.service';
import {dropdownOptionsModel} from '../../../../../shared/models/dropdown-options.model';
import {HttpService} from '../../../../../core/http/http.service';
import {UserService} from '../../../../../core/auth/user.service';

export interface CountryDto extends dropdownOptionsModel {
  code: string;
}

@Injectable({ providedIn: 'root' })
export class ProfileLookupsService {
  private http = inject(HttpService);
  private endpoints = inject(EndpointsService);
  private userService = inject(UserService);

  loading = signal<boolean>(false);
  loaded = signal<boolean>(false);

  candidateTypes   = signal<dropdownOptionsModel[]>([]);
  targetEntities   = signal<dropdownOptionsModel[]>([]);
  genders          = signal<dropdownOptionsModel[]>([]);
  religions        = signal<dropdownOptionsModel[]>([]);
  maritalStatuses  = signal<dropdownOptionsModel[]>([]);
  countries        = signal<CountryDto[]>([]);
  degrees          = signal<dropdownOptionsModel[]>([]);
  studyTypes       = signal<dropdownOptionsModel[]>([]);
  ratingGrades     = signal<dropdownOptionsModel[]>([]);
  skillLevels     = signal<dropdownOptionsModel[]>([]);
  achievementTypes = signal<dropdownOptionsModel[]>([]);
  languages        = signal<dropdownOptionsModel[]>([]);
  languageLevels   = signal<dropdownOptionsModel[]>([]);
  nationalities        = signal<CountryDto[]>([]);
  interviewLocation        = signal<CountryDto[]>([]);
  residenceCountry        = signal<CountryDto[]>([]);
  graduationCountry        = signal<CountryDto[]>([]);
  sponsorTypes        = signal<dropdownOptionsModel[]>([]);
  offices        = signal<dropdownOptionsModel[]>([]);

  loadAll(): Observable<void> {
    if (this.loaded()) return new Observable(observer => {
      observer.next();
      observer.complete();
    });

    this.loading.set(true);

    const provider = this.userService.getPrefill()?.provider ?? '';
    return forkJoin({
      candidateTypes: this.http.get<dropdownOptionsModel[]>(this.endpoints.profile.lookups.candidateTypes, { provider }),
      targetEntities: this.http.get<dropdownOptionsModel[]>(this.endpoints.profile.lookups.targetEntities),
      genders: this.http.get<dropdownOptionsModel[]>(this.endpoints.profile.lookups.genders),
      religions: this.http.get<dropdownOptionsModel[]>(this.endpoints.profile.lookups.religions),
      maritalStatuses: this.http.get<dropdownOptionsModel[]>(this.endpoints.profile.lookups.maritalStatuses),
      countries: this.http.get<CountryDto[]>(this.endpoints.profile.lookups.countries),
      degrees: this.http.get<dropdownOptionsModel[]>(this.endpoints.profile.lookups.degrees),
      studyTypes: this.http.get<dropdownOptionsModel[]>(this.endpoints.profile.lookups.studyTypes),
      ratingGrades: this.http.get<dropdownOptionsModel[]>(this.endpoints.profile.lookups.ratingGrades),
      skillLevels: this.http.get<dropdownOptionsModel[]>(this.endpoints.profile.lookups.skillLevels),
      achievementTypes: this.http.get<dropdownOptionsModel[]>(this.endpoints.profile.lookups.achievementTypes),
      languages: this.http.get<dropdownOptionsModel[]>(this.endpoints.profile.lookups.languages),
      languageLevels: this.http.get<dropdownOptionsModel[]>(this.endpoints.profile.lookups.languageLevels),
      sponsorTypes: this.http.get<dropdownOptionsModel[]>(this.endpoints.profile.lookups.sponsorTypes),
      offices: this.http.get<dropdownOptionsModel[]>(this.endpoints.profile.lookups.offices),
    }).pipe(
      map(res => {
        this.candidateTypes.set(res.candidateTypes);
        this.targetEntities.set(res.targetEntities);
        this.genders.set(res.genders);
        this.religions.set(res.religions);
        this.maritalStatuses.set(res.maritalStatuses);
        this.countries.set(res.countries);
        this.nationalities.set(res.countries);
        this.interviewLocation.set(res.countries);
        this.residenceCountry.set(res.countries);
        this.graduationCountry.set(res.countries);
        this.degrees.set(res.degrees);
        this.studyTypes.set(res.studyTypes);
        this.ratingGrades.set(res.ratingGrades);
        this.skillLevels.set(res.skillLevels);
        this.achievementTypes.set(res.achievementTypes);
        this.languages.set(res.languages);
        this.languageLevels.set(res.languageLevels);
        this.sponsorTypes.set(res.sponsorTypes);
        this.offices.set(res.offices);

        this.loaded.set(true);
        this.loading.set(false);
      }),
      catchError(err => {
        console.error('Failed to load profile lookups', err);
        this.loading.set(false);
        throw err;
      })
    );
  }
  searchSkills(query: string): Observable<dropdownOptionsModel[]> {
    return this.http.get<dropdownOptionsModel[]>(this.endpoints.profile.lookups.skill, { 'search': query });
  }
}
