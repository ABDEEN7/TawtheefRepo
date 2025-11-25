import { inject, Injectable, signal } from '@angular/core';
import {HttpClient, HttpParams} from '@angular/common/http';
import {forkJoin, Observable} from 'rxjs';
import {EndpointsService} from '../../../../core/http/endpoints.service';
import {SkillDto} from '../models/skill-dto.model';
import {dropdownOptionsModel} from '../../../../shared/models/dropdown-options.model';

export interface CountryDto extends dropdownOptionsModel {
  code: string;
}

@Injectable({ providedIn: 'root' })
export class ProfileLookupsService {
  private http = inject(HttpClient);
  private endpoints = inject(EndpointsService);

  loading = signal<boolean>(false);
  loaded = signal<boolean>(false);

  candidateTypes   = signal<dropdownOptionsModel[]>([]);
  targetEntities   = signal<dropdownOptionsModel[]>([]);
  genders          = signal<dropdownOptionsModel[]>([]);
  religions        = signal<dropdownOptionsModel[]>([]);
  maritalStatuses  = signal<dropdownOptionsModel[]>([]);
  countries        = signal<CountryDto[]>([]);
  degrees          = signal<dropdownOptionsModel[]>([]);
  universities     = signal<dropdownOptionsModel[]>([]);
  majors           = signal<dropdownOptionsModel[]>([]);
  studyTypes       = signal<dropdownOptionsModel[]>([]);
  ratingGrades     = signal<dropdownOptionsModel[]>([]);
  languages        = signal<dropdownOptionsModel[]>([]);
  languageLevels   = signal<dropdownOptionsModel[]>([]);
  nationalities        = signal<CountryDto[]>([]);
  interviewLocation        = signal<CountryDto[]>([]);
  residenceCountry        = signal<CountryDto[]>([]);
  graduationCountry        = signal<CountryDto[]>([]);
  sponsorTypes        = signal<dropdownOptionsModel[]>([]);

  loadAll() {
    if (this.loaded()) return;
    this.loading.set(true);

    forkJoin({
      candidateTypes:  this.http.get<dropdownOptionsModel[]>(this.endpoints.profile.lookups.candidateTypes),
      targetEntities:  this.http.get<dropdownOptionsModel[]>(this.endpoints.profile.lookups.targetEntities),
      genders:         this.http.get<dropdownOptionsModel[]>(this.endpoints.profile.lookups.genders),
      religions:       this.http.get<dropdownOptionsModel[]>(this.endpoints.profile.lookups.religions),
      maritalStatuses: this.http.get<dropdownOptionsModel[]>(this.endpoints.profile.lookups.maritalStatuses),
      countries:       this.http.get<CountryDto[]>(this.endpoints.profile.lookups.countries),
      degrees:         this.http.get<dropdownOptionsModel[]>(this.endpoints.profile.lookups.degrees),
      universities:    this.http.get<dropdownOptionsModel[]>(this.endpoints.profile.lookups.universities),
      majors:          this.http.get<dropdownOptionsModel[]>(this.endpoints.profile.lookups.majors),
      studyTypes:      this.http.get<dropdownOptionsModel[]>(this.endpoints.profile.lookups.studyTypes),
      ratingGrades:    this.http.get<dropdownOptionsModel[]>(this.endpoints.profile.lookups.ratingGrades),
      languages:       this.http.get<dropdownOptionsModel[]>(this.endpoints.profile.lookups.languages),
      languageLevels:  this.http.get<dropdownOptionsModel[]>(this.endpoints.profile.lookups.languageLevels),
      sponsorTypes:  this.http.get<dropdownOptionsModel[]>(this.endpoints.profile.lookups.sponsorTypes),
    }).subscribe({
      next: (res) => {
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
        this.universities.set(res.universities);
        this.majors.set(res.majors);
        this.studyTypes.set(res.studyTypes);
        this.ratingGrades.set(res.ratingGrades);
        this.languages.set(res.languages);
        this.languageLevels.set(res.languageLevels);
        this.sponsorTypes.set(res.sponsorTypes);

        this.loaded.set(true);
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Failed to load profile lookups', err);
        this.loading.set(false);
      }
    });
  }
  searchSkills(query: string): Observable<SkillDto[]> {
    const params = new HttpParams().set('q', query);
    return this.http.get<SkillDto[]>(this.endpoints.profile.lookups.skill, { params });
  }
}
