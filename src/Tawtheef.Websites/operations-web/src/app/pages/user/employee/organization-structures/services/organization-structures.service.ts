import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { HttpService } from '../../../../../core/http/http.service';
import { EndpointsService } from '../../../../../core/http/endpoints.service';
import { PaginatedResult } from '../../../../../core/models/paginated-result.model';
import { dropdownOptionsModel } from '../../../../../shared/models/dropdown-options.model';

import { SectorListItemModel } from '../models/sector-list-item.model';
import { ManagementListItemModel } from '../models/management-list-item.model';
import { DepartmentListItemModel } from '../models/department-list-item.model';
import { SectorFiltersModel } from '../models/sector-filters.model';
import { ManagementFiltersModel } from '../models/management-filters.model';
import { DepartmentFiltersModel } from '../models/department-filters.model';

@Injectable({ providedIn: 'root' })
export class OrganizationStructuresService {
  private http = inject(HttpService);
  private endpoints = inject(EndpointsService);

  getSectors(filters: SectorFiltersModel): Observable<PaginatedResult<SectorListItemModel>> {
    return this.http.get<PaginatedResult<SectorListItemModel>>(this.endpoints.organizationStructures.sectors.list, filters);
  }

  createSector(payload: Partial<SectorListItemModel>): Observable<void> {
    return this.http.post<void>(this.endpoints.organizationStructures.sectors.create, payload);
  }

  updateSector(id: string, payload: Partial<SectorListItemModel>): Observable<void> {
    return this.http.put<void>(this.endpoints.organizationStructures.sectors.update(id), payload);
  }

  changeSectorActivation(id: string, isActive: boolean, applyOnHierarchy = true): Observable<void> {
    return this.http.put<void>(this.endpoints.organizationStructures.sectors.changeActivation, { id, isActive, applyOnHierarchy });
  }

  getManagements(filters: ManagementFiltersModel): Observable<PaginatedResult<ManagementListItemModel>> {
    const params = {
      ...filters,
      sectorId: filters.sectorId || undefined
    };
    return this.http.get<PaginatedResult<ManagementListItemModel>>(this.endpoints.organizationStructures.managements.list, params);
  }

  createManagement(payload: Partial<ManagementListItemModel>): Observable<void> {
    return this.http.post<void>(this.endpoints.organizationStructures.managements.create, payload);
  }

  updateManagement(id: string, payload: Partial<ManagementListItemModel>): Observable<void> {
    return this.http.put<void>(this.endpoints.organizationStructures.managements.update(id), payload);
  }

  changeManagementActivation(id: string, isActive: boolean, applyOnHierarchy = true): Observable<void> {
    return this.http.put<void>(this.endpoints.organizationStructures.managements.changeActivation, { id, isActive, applyOnHierarchy });
  }

  getDepartments(filters: DepartmentFiltersModel): Observable<PaginatedResult<DepartmentListItemModel>> {
    const params = {
      ...filters,
      sectorId: filters.sectorId || undefined,
      managementId: filters.managementId || undefined
    };
    return this.http.get<PaginatedResult<DepartmentListItemModel>>(this.endpoints.organizationStructures.departments.list, params);
  }

  createDepartment(payload: Partial<DepartmentListItemModel>): Observable<void> {
    return this.http.post<void>(this.endpoints.organizationStructures.departments.create, payload);
  }

  updateDepartment(id: string, payload: Partial<DepartmentListItemModel>): Observable<void> {
    return this.http.put<void>(this.endpoints.organizationStructures.departments.update(id), payload);
  }

  changeDepartmentActivation(id: string, isActive: boolean): Observable<void> {
    return this.http.put<void>(this.endpoints.organizationStructures.departments.changeActivation, { id, isActive });
  }

  getSectorLookups(): Observable<dropdownOptionsModel[]> {
    return this.http.get<dropdownOptionsModel[]>(this.endpoints.organizationStructures.lookups.sectors);
  }

  getManagementLookups(sectorId: string): Observable<dropdownOptionsModel[]> {
    return this.http.get<dropdownOptionsModel[]>(this.endpoints.organizationStructures.lookups.managements(sectorId));
  }

  getDepartmentLookups(managementId: string): Observable<dropdownOptionsModel[]> {
    return this.http.get<dropdownOptionsModel[]>(this.endpoints.organizationStructures.lookups.departments(managementId));
  }
}
