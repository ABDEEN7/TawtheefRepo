import {inject, Injectable, signal} from '@angular/core';
import {HttpService} from '../../../../../core/http/http.service';
import {EndpointsService} from '../../../../../core/http/endpoints.service';
import {PaginatedResult} from '../../../../../core/models/paginated-result.model';
import {PaginationMetadata} from '../../../../../core/models/pagination-metadata.model';
import {dropdownOptionsModel} from '../../../../../shared/models/dropdown-options.model';
import {MajorSkillListItemModel} from '../models/major-skill-list-item.model';
import {MajorSkillFiltersModel} from '../models/major-skill-filters.model';
import {MajorSkillDetailsModel} from '../models/major-skill-details.model';
import {MajorFiltersModel} from '../models/major-filters.model';
import {SkillFiltersModel} from '../models/skill-filters.model';
import {SkillListItemModel} from '../models/skill-list-item.model';
import {MajorListItemModel} from '../models/major-list-item.model';
import {map, tap} from 'rxjs/operators';
import {Observable, of} from 'rxjs';

@Injectable({ providedIn: 'root' })
export class MajorsSkillsManagementService {
  private http = inject(HttpService);
  private endpoints = inject(EndpointsService);

  private _majorSkills = signal<MajorSkillListItemModel[]>([]);
  private _majorSkillsMeta = signal<PaginationMetadata | null>(null);

  private _skills = signal<SkillListItemModel[]>([]);
  private _skillsMeta = signal<PaginationMetadata | null>(null);

  private _mainMajors = signal<MajorListItemModel[]>([]);
  private _subMajors = signal<dropdownOptionsModel[]>([]);
  private _skillTypes = signal<dropdownOptionsModel[]>([]);

  majorSkills = this._majorSkills.asReadonly();
  majorSkillsMeta = this._majorSkillsMeta.asReadonly();

  skills = this._skills.asReadonly();
  skillsMeta = this._skillsMeta.asReadonly();

  mainMajors = this._mainMajors.asReadonly();
  subMajors = this._subMajors.asReadonly();
  skillTypes = this._skillTypes.asReadonly();

  loadMajorSkills(filters: MajorSkillFiltersModel): Observable<void> {
    return this.http.get<PaginatedResult<MajorSkillListItemModel>>(this.endpoints.majorSkillsManagement.list, filters)
      .pipe(
        tap(res => {
          this._majorSkills.set(res.items || []);
          this._majorSkillsMeta.set(res.metadata);
        }),
        map(() => void 0)
      );
  }

  loadMajorSkillDetails(id: string): Observable<MajorSkillDetailsModel> {
    return this.http.get<MajorSkillDetailsModel>(this.endpoints.majorSkillsManagement.details(id));
  }

  createMajorSkill(payload: Partial<MajorSkillDetailsModel>): Observable<void> {
    return this.http.post<void>(this.endpoints.majorSkillsManagement.create, payload);
  }

  updateMajorSkill(payload: Partial<MajorSkillDetailsModel>): Observable<void> {
    return this.http.put<void>(this.endpoints.majorSkillsManagement.update, payload);
  }

  changeMajorSkillActivation(id: string, isActive: boolean): Observable<void> {
    return this.http.put<void>(this.endpoints.majorSkillsManagement.changeActivation, {id, isActive});
  }

  loadSkills(filters: SkillFiltersModel): Observable<void> {
    return this.http.get<PaginatedResult<SkillListItemModel>>(this.endpoints.skillsManagement.list, filters)
      .pipe(
        tap(res => {
          const mapped = (res.items || []).map(s => ({
            ...s,
            isActive: (s as any).isActive ?? s.additionalData?.isActive ?? true,
            skillTypeId: (s as any).skillTypeId ?? s.additionalData?.skillTypeId,
            skillTypeName: (s as any).skillTypeName ?? s.additionalData?.skillTypeName
          }));
          this._skills.set(mapped);
          this._skillsMeta.set(res.metadata);
        }),
        map(() => void 0)
      );
  }

  createSkill(payload: any): Observable<void> {
    return this.http.post<void>(this.endpoints.skillsManagement.create, payload);
  }

  updateSkill(payload: any): Observable<void> {
    return this.http.put<void>(this.endpoints.skillsManagement.update, payload);
  }

  changeSkillActivation(id: string, isActive: boolean): Observable<void> {
    return this.http.put<void>(this.endpoints.skillsManagement.changeActivation, {id, isActive});
  }

  loadMainMajors(filters: MajorFiltersModel): Observable<void> {
    return this.http.get<MajorListItemModel[]>(this.endpoints.majorsManagement.list, {search: filters.search})
      .pipe(
        tap(res => this._mainMajors.set(res || [])),
        map(() => void 0)
      );
  }

  createMajor(payload: any): Observable<void> {
    return this.http.post<void>(this.endpoints.majorsManagement.create, payload);
  }

  updateMajor(payload: any): Observable<void> {
    return this.http.put<void>(this.endpoints.majorsManagement.update, payload);
  }

  changeMajorActivation(id: string, isActive: boolean): Observable<void> {
    return this.http.put<void>(this.endpoints.majorsManagement.changeActivation, {id, isActive});
  }

  loadSubMajors(parentMajorId: string): Observable<void> {
    if (!parentMajorId) {
      this._subMajors.set([]);
      return of(void 0);
    }

    return this.http.get<dropdownOptionsModel[]>(`${this.endpoints.job.lookups.subMajors}?majorId=${parentMajorId}`)
      .pipe(
        tap(res => this._subMajors.set(res || [])),
        map(() => void 0)
      );
  }

  loadSkillTypes(): Observable<void> {
    return this.http.get<PaginatedResult<SkillListItemModel>>(this.endpoints.skillsManagement.list, {
      pageNumber: 1,
      pageSize: 50
    }).pipe(
      tap(res => {
        const uniqueTypes = new Map<string, dropdownOptionsModel>();
        (res.items || []).forEach(skill => {
          const skillTypeId = (skill as any).skillTypeId || skill.additionalData?.skillTypeId;
          const skillTypeName = (skill as any).skillTypeName || skill.additionalData?.skillTypeName;
          if (skillTypeId && !uniqueTypes.has(skillTypeId)) {
            uniqueTypes.set(skillTypeId, {
              id: skillTypeId,
              name: skillTypeName || '',
              backendName: skillTypeName || '',
              description: skillTypeName || '',
              additionalData: null
            } as dropdownOptionsModel);
          }
        });
        this._skillTypes.set(Array.from(uniqueTypes.values()));
      }),
      map(() => void 0)
    );
  }
}
