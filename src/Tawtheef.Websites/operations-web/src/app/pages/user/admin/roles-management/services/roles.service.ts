import {Injectable, inject} from '@angular/core';
import { Observable } from 'rxjs';
import {RoleDto} from '../models/permission.model';
import {PermissionDto} from '../models/role.model';
import {map} from 'rxjs/operators';
import {HttpService} from '../../../../../core/http/http.service';
import {EndpointsService} from '../../../../../core/http/endpoints.service';
import {PaginatedRequest} from '../../../../../core/models/paginated-request.model';
import {PaginatedResult} from '../../../../../core/models/paginated-result.model';

@Injectable({ providedIn: 'root' })
export class RolesService {

  private http = inject(HttpService);
  private endpoints = inject(EndpointsService);

  // =========================
  // GET ALL ROLES
  // =========================
  getRoles(pagination?: PaginatedRequest): Observable<PaginatedResult<RoleDto>> {
    return this.http.get<PaginatedResult<RoleDto>>(this.endpoints.roles.listRoles, pagination);
  }

  // =========================
  // GET PERMISSIONS
  // =========================
  getPermissions(): Observable<PermissionDto[]> {
    return this.http.get<PermissionDto[]>(this.endpoints.roles.listPermissions);
  }

  // =========================
  // GET ROLE BY ID
  // =========================
  getRoleById(id: string): Observable<RoleDto> {
    return this.http.get<RoleDto>(this.endpoints.roles.roleDetails(id));
  }

  // =========================
  // ADD ROLE
  // =========================
  addRole(model: RoleDto): Observable<RoleDto> {
    return this.http.post<RoleDto>(this.endpoints.roles.createRole, model);
  }

  // =========================
  // UPDATE ROLE
  // =========================
  updateRole(model: RoleDto): Observable<RoleDto> {
    return this.http.put<RoleDto>(this.endpoints.roles.updateRole(model.id), model);
  }

  // =========================
  // DELETE ROLE
  // =========================
  deleteRole(id: string): Observable<void> {
    return this.http.delete<void>(this.endpoints.roles.deleteRole(id));
  }
}
