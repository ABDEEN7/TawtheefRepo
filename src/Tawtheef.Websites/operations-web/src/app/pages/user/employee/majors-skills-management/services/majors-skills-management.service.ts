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
    return this.http.get<PaginatedResult<SkillListItemModel>>(this.endpoints.majorSkillsManagement.skills.list, filters);
  }

  createSkill(payload: any): Observable<void> {
    return this.http.post<void>(this.endpoints.majorSkillsManagement.skills.create, payload);
  }

  updateSkill(payload: any): Observable<void> {
    return this.http.put<void>(this.endpoints.majorSkillsManagement.skills.update, payload);
  }

  changeSkillActivation(id: string, isActive: boolean): Observable<void> {
    return this.http.put<void>(this.endpoints.majorSkillsManagement.skills.changeActivation, { id, isActive });
  }

  // =============== Majors ===============
  getMainMajors(filters: MajorFiltersModel): Observable<PaginatedResult<dropdownOptionsModel>> {
    return this.http.get<PaginatedResult<dropdownOptionsModel>>(this.endpoints.majorSkillsManagement.majors.main_list, filters);
  }

  createMajor(payload: any): Observable<void> {
    return this.http.post<void>(this.endpoints.majorSkillsManagement.majors.create, payload);
  }

  updateMajor(payload: any): Observable<void> {
    return this.http.put<void>(this.endpoints.majorSkillsManagement.majors.update, payload);
  }

  changeMajorActivation(id: string, isActive: boolean): Observable<void> {
    return this.http.put<void>(this.endpoints.majorSkillsManagement.majors.changeActivation, { id, isActive });
  }

  // =============== Lookups ===============
  getSubMajors(parentMajorId: string): Observable<PaginatedResult<dropdownOptionsModel>> {
    return this.http.get<PaginatedResult<dropdownOptionsModel>>(`${this.endpoints.majorSkillsManagement.majors.sub_list}?parentId=${parentMajorId}`);
  }

  getSkillsPageForTypes(): Observable<SkillListItemModel[]> {
    return this.http.get<SkillListItemModel[]>(this.endpoints.majorSkillsManagement.lookups.skillTypes);
  }

  getSubMajorsPaged(filters: MajorFiltersModel & { parentMajorId: string }): Observable<PaginatedResult<MajorListItemModel>>{
    return this.http.get<PaginatedResult<MajorListItemModel>>(this.endpoints.majorSkillsManagement.majors.sub_list, filters);
  }

}
