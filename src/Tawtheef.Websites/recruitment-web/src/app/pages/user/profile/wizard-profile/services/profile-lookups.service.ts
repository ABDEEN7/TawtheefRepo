import { inject, Injectable, signal } from '@angular/core';
import { forkJoin, Observable } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { EndpointsService } from '../../../../../core/http/endpoints.service';
import { dropdownOptionsModel, DropdownOptionVM } from '../../../../../shared/models/dropdown-options.model';
import { HttpService } from '../../../../../core/http/http.service';
import { UserService } from '../../../../../core/auth/user.service';
import { HttpHeaders } from '@angular/common/http';

export interface CountryDto extends dropdownOptionsModel {
  code: string;
}
export class CountryVM extends DropdownOptionVM {
  code: string;
  constructor(dto: CountryDto) {
    super(dto);
    this.code = dto.code;
  }
}

@Injectable({ providedIn: 'root' })
export class ProfileLookupsService {
  private http = inject(HttpService);
  private endpoints = inject(EndpointsService);
  private userService = inject(UserService);

  loading = signal<boolean>(false);
  loaded = signal<boolean>(false);

  candidateTypes = signal<DropdownOptionVM[]>([]);
  targetEntities = signal<DropdownOptionVM[]>([]);
  genders = signal<DropdownOptionVM[]>([]);
  religions = signal<DropdownOptionVM[]>([]);
  maritalStatuses = signal<DropdownOptionVM[]>([]);
  countries = signal<CountryVM[]>([]);
  degrees = signal<DropdownOptionVM[]>([]);
  studyTypes = signal<DropdownOptionVM[]>([]);
  ratingGrades = signal<DropdownOptionVM[]>([]);
  skillLevels = signal<DropdownOptionVM[]>([]);
  achievementTypes = signal<DropdownOptionVM[]>([]);
  languages = signal<DropdownOptionVM[]>([]);
  languageLevels = signal<DropdownOptionVM[]>([]);
  nationalities = signal<CountryVM[]>([]);
  interviewLocation = signal<CountryVM[]>([]);
  residenceCountry = signal<CountryVM[]>([]);
  graduationCountry = signal<CountryVM[]>([]);
  sponsorTypes = signal<DropdownOptionVM[]>([]);

  loadAll(): Observable<void> {
    if (this.loaded()) return new Observable(observer => {
      observer.next();
      observer.complete();
    });

    this.loading.set(true);

    const provider = this.userService.getCurrentUser()?.provider ?? '';
    return forkJoin({
      candidateTypes: this.http.get<DropdownOptionVM[]>(this.endpoints.profile.lookups.candidateTypes, { provider }),
      targetEntities: this.http.get<DropdownOptionVM[]>(this.endpoints.profile.lookups.targetEntities),
      genders: this.http.get<DropdownOptionVM[]>(this.endpoints.profile.lookups.genders),
      religions: this.http.get<DropdownOptionVM[]>(this.endpoints.profile.lookups.religions),
      maritalStatuses: this.http.get<DropdownOptionVM[]>(this.endpoints.profile.lookups.maritalStatuses),
      countries: this.http.get<CountryDto[]>(this.endpoints.profile.lookups.countries),
      nationalities: this.http.get<CountryDto[]>(this.endpoints.profile.lookups.nationalities),
      graduationCountries: this.http.get<CountryDto[]>(this.endpoints.profile.lookups.graduationCountries),
      degrees: this.http.get<DropdownOptionVM[]>(this.endpoints.profile.lookups.degrees),
      studyTypes: this.http.get<DropdownOptionVM[]>(this.endpoints.profile.lookups.studyTypes),
      ratingGrades: this.http.get<DropdownOptionVM[]>(this.endpoints.profile.lookups.ratingGrades),
      skillLevels: this.http.get<DropdownOptionVM[]>(this.endpoints.profile.lookups.skillLevels),
      achievementTypes: this.http.get<DropdownOptionVM[]>(this.endpoints.profile.lookups.achievementTypes),
      languages: this.http.get<DropdownOptionVM[]>(this.endpoints.profile.lookups.languages),
      languageLevels: this.http.get<DropdownOptionVM[]>(this.endpoints.profile.lookups.languageLevels),
      sponsorTypes: this.http.get<DropdownOptionVM[]>(this.endpoints.profile.lookups.sponsorTypes),
    }).pipe(
      map(res => {
        this.candidateTypes.set(this.toVMs(res.candidateTypes));
        this.targetEntities.set(this.toVMs(res.targetEntities));
        this.genders.set(this.toVMs(res.genders));
        this.religions.set(this.toVMs(res.religions));
        this.maritalStatuses.set(this.toVMs(res.maritalStatuses));
        this.countries.set(this.toCountryVMs(res.countries));
        this.nationalities.set(this.toCountryVMs(res.nationalities));
        this.interviewLocation.set(this.toCountryVMs(res.countries));
        this.residenceCountry.set(this.toCountryVMs(res.countries));
        this.graduationCountry.set(this.toCountryVMs(res.graduationCountries));
        this.degrees.set(this.toVMs(res.degrees));
        this.studyTypes.set(this.toVMs(res.studyTypes));
        this.ratingGrades.set(this.toVMs(res.ratingGrades));
        this.skillLevels.set(this.toVMs(res.skillLevels));
        this.achievementTypes.set(this.toVMs(res.achievementTypes));
        this.languages.set(this.toVMs(res.languages));
        this.languageLevels.set(this.toVMs(res.languageLevels));
        this.sponsorTypes.set(this.toVMs(res.sponsorTypes));

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
  searchSkills(query: string): Observable<DropdownOptionVM[]> {
    return this.http.get<DropdownOptionVM[]>(this.endpoints.profile.lookups.skill, { 'search': query }, {
      headers: new HttpHeaders({ 'X-Skip-Loading': 'true' })
    });
  }
  private toVMs<T extends dropdownOptionsModel>(arr: T[]): DropdownOptionVM[] {
    return (arr ?? []).map(x => new DropdownOptionVM(x));
  }
  private toCountryVMs(arr: CountryDto[]): CountryVM[] {
    return (arr ?? []).map(x => new CountryVM(x));
  }
}
