import {Injectable, inject, signal} from '@angular/core';
import { Observable } from 'rxjs';
import {RoleDto} from '../models/permission.model';
import {PermissionDto} from '../models/role.model';
import {map, tap} from 'rxjs/operators';
import {HttpService} from '../../../../../core/http/http.service';
import {EndpointsService} from '../../../../../core/http/endpoints.service';
import {PaginationMetadata} from '../../../../../core/models/pagination-metadata.model';
import {PaginatedRequest} from '../../../../../core/models/paginated-request.model';
import {PaginatedResult} from '../../../../../core/models/paginated-result.model';

@Injectable({ providedIn: 'root' })
export class RolesService {

  private http = inject(HttpService);
  private endpoints = inject(EndpointsService);
  private _paginationMetadata = signal<PaginationMetadata | null>(null);
  private _roles = signal<RoleDto[]>([]);

  public roles = this._roles.asReadonly();
  public paginationMetadata = this._paginationMetadata.asReadonly();

  // =========================
  // GET ALL ROLES
  // =========================
  getRoles(pagination?: PaginatedRequest): Observable<void> {
    return this.http.get<PaginatedResult<RoleDto>>(this.endpoints.roles.listRoles, pagination).pipe(
      tap(response => {
        this._paginationMetadata.set(response.metadata);
        this._roles.set(response.items || []);
      }),
      map(() => void 0)
    );
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
