import { inject, Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { forkJoin } from 'rxjs';
import {EndpointsService} from '../../../../core/http/endpoints.service';

export interface LookupDto {
  id: string;
  backendName: string;
  name: string;
  description?: string;
}

export interface CountryDto extends LookupDto {
  code: string;
}

@Injectable({ providedIn: 'root' })
export class ProfileLookupsService {
  private http = inject(HttpClient);
  private endpoints = inject(EndpointsService);

  loading = signal<boolean>(false);
  loaded = signal<boolean>(false);

  candidateTypes   = signal<LookupDto[]>([]);
  targetEntities   = signal<LookupDto[]>([]);
  genders          = signal<LookupDto[]>([]);
  religions        = signal<LookupDto[]>([]);
  maritalStatuses  = signal<LookupDto[]>([]);
  countries        = signal<CountryDto[]>([]);
  degrees          = signal<LookupDto[]>([]);
  universities     = signal<LookupDto[]>([]);
  majors           = signal<LookupDto[]>([]);
  studyTypes       = signal<LookupDto[]>([]);
  ratingGrades     = signal<LookupDto[]>([]);
  languages        = signal<LookupDto[]>([]);
  languageLevels   = signal<LookupDto[]>([]);

  loadAll() {
    if (this.loaded()) return;
    this.loading.set(true);

    forkJoin({
      candidateTypes:  this.http.get<LookupDto[]>(this.endpoints.profile.lookups.candidateTypes),
      targetEntities:  this.http.get<LookupDto[]>(this.endpoints.profile.lookups.targetEntities),
      genders:         this.http.get<LookupDto[]>(this.endpoints.profile.lookups.genders),
      religions:       this.http.get<LookupDto[]>(this.endpoints.profile.lookups.religions),
      maritalStatuses: this.http.get<LookupDto[]>(this.endpoints.profile.lookups.maritalStatuses),
      countries:       this.http.get<CountryDto[]>(this.endpoints.profile.lookups.countries),
      degrees:         this.http.get<LookupDto[]>(this.endpoints.profile.lookups.degrees),
      universities:    this.http.get<LookupDto[]>(this.endpoints.profile.lookups.universities),
      majors:          this.http.get<LookupDto[]>(this.endpoints.profile.lookups.majors),
      studyTypes:      this.http.get<LookupDto[]>(this.endpoints.profile.lookups.studyTypes),
      ratingGrades:    this.http.get<LookupDto[]>(this.endpoints.profile.lookups.ratingGrades),
      languages:       this.http.get<LookupDto[]>(this.endpoints.profile.lookups.languages),
      languageLevels:  this.http.get<LookupDto[]>(this.endpoints.profile.lookups.languageLevels),
    }).subscribe({
      next: (res) => {
        this.candidateTypes.set(res.candidateTypes);
        this.targetEntities.set(res.targetEntities);
        this.genders.set(res.genders);
        this.religions.set(res.religions);
        this.maritalStatuses.set(res.maritalStatuses);
        this.countries.set(res.countries);
        this.degrees.set(res.degrees);
        this.universities.set(res.universities);
        this.majors.set(res.majors);
        this.studyTypes.set(res.studyTypes);
        this.ratingGrades.set(res.ratingGrades);
        this.languages.set(res.languages);
        this.languageLevels.set(res.languageLevels);

        this.loaded.set(true);
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Failed to load profile lookups', err);
        this.loading.set(false);
      }
    });
  }
}
