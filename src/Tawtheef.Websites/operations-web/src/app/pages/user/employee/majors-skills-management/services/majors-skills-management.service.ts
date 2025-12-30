import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { HttpService } from '../../../../../core/http/http.service';
import { EndpointsService } from '../../../../../core/http/endpoints.service';

import { PaginatedResult } from '../../../../../core/models/paginated-result.model';
import { dropdownOptionsModel } from '../../../../../shared/models/dropdown-options.model';

import { MajorSkillListItemModel } from '../models/major-skill-list-item.model';
import { MajorSkillFiltersModel } from '../models/major-skill-filters.model';
import { MajorSkillDetailsModel } from '../models/major-skill-details.model';

import { MajorFiltersModel } from '../models/major-filters.model';
import { MajorListItemModel } from '../models/major-list-item.model';

import { SkillFiltersModel } from '../models/skill-filters.model';
import { SkillListItemModel } from '../models/skill-list-item.model';

export type SkillTypeOption = dropdownOptionsModel;

@Injectable({ providedIn: 'root' })
export class MajorsSkillsManagementService {
  private http = inject(HttpService);
  private endpoints = inject(EndpointsService);

  // =============== Major-Skill Mapping ===============
  getMajorSkills(filters: MajorSkillFiltersModel): Observable<PaginatedResult<MajorSkillListItemModel>> {
    return this.http.get<PaginatedResult<MajorSkillListItemModel>>(this.endpoints.majorSkillsManagement.list, filters);
  }

  getMajorSkillDetails(id: string): Observable<MajorSkillDetailsModel> {
    return this.http.get<MajorSkillDetailsModel>(this.endpoints.majorSkillsManagement.details(id));
  }

  createMajorSkill(payload: Partial<MajorSkillDetailsModel>): Observable<void> {
    return this.http.post<void>(this.endpoints.majorSkillsManagement.create, payload);
  }

  updateMajorSkill(payload: Partial<MajorSkillDetailsModel>): Observable<void> {
    return this.http.put<void>(this.endpoints.majorSkillsManagement.update, payload);
  }

  changeMajorSkillActivation(id: string, isActive: boolean): Observable<void> {
    return this.http.put<void>(this.endpoints.majorSkillsManagement.changeActivation, { id, isActive });
  }

  // =============== Skills ===============
  getSkills(filters: SkillFiltersModel): Observable<PaginatedResult<SkillListItemModel>> {
    return this.http.get<PaginatedResult<SkillListItemModel>>(this.endpoints.skillsManagement.list, filters);
  }

  createSkill(payload: any): Observable<void> {
    return this.http.post<void>(this.endpoints.skillsManagement.create, payload);
  }

  updateSkill(payload: any): Observable<void> {
    return this.http.put<void>(this.endpoints.skillsManagement.update, payload);
  }

  changeSkillActivation(id: string, isActive: boolean): Observable<void> {
    return this.http.put<void>(this.endpoints.skillsManagement.changeActivation, { id, isActive });
  }

  // =============== Majors ===============
  getMainMajors(filters: MajorFiltersModel): Observable<MajorListItemModel[]> {
    return this.http.get<MajorListItemModel[]>(this.endpoints.majorsManagement.list, { search: filters.search });
  }

  createMajor(payload: any): Observable<void> {
    return this.http.post<void>(this.endpoints.majorsManagement.create, payload);
  }

  updateMajor(payload: any): Observable<void> {
    return this.http.put<void>(this.endpoints.majorsManagement.update, payload);
  }

  changeMajorActivation(id: string, isActive: boolean): Observable<void> {
    return this.http.put<void>(this.endpoints.majorsManagement.changeActivation, { id, isActive });
  }

  // =============== Lookups ===============
  getSubMajors(parentMajorId: string): Observable<dropdownOptionsModel[]> {
    return this.http.get<dropdownOptionsModel[]>(
      `${this.endpoints.job.lookups.subMajors}?majorId=${parentMajorId}`
    );
  }

  /**
   * NOTE:
   * You were deriving "Skill Types" from skills list. Stateless service should *not*
   * mutate/store. Either:
   * 1) create a real endpoint (recommended): endpoints.skillsManagement.skillTypes
   * 2) or keep this helper that just returns the raw page and let the component map it.
   */
  getSkillsPageForTypes(pageNumber = 1, pageSize = 50): Observable<PaginatedResult<SkillListItemModel>> {
    return this.http.get<PaginatedResult<SkillListItemModel>>(this.endpoints.skillsManagement.list, {
      pageNumber,
      pageSize
    });
  }
}
