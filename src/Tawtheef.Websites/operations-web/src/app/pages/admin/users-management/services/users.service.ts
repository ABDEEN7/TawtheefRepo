import {inject, Injectable, signal} from '@angular/core';
import {HttpService} from '../../../../core/http/http.service';
import {EndpointsService} from '../../../../core/http/endpoints.service';
import {PaginatedResult} from '../../../../core/models/paginated-result.model';
import {PaginationMetadata} from '../../../../core/models/pagination-metadata.model';
import {UserDto} from '../models/user.dto';
import {UserFilters} from '../models/user-filters.dto';
import {UserRolesResponse} from '../models/user-roles-response.dto';
import {map, tap} from 'rxjs/operators';
import {Observable} from 'rxjs';
import {RoleSummaryDto} from '../models/role-summary.dto';

@Injectable({ providedIn: 'root' })
export class UsersService {
  private http = inject(HttpService);
  private endpoints = inject(EndpointsService);

  private _users = signal<UserDto[]>([]);
  private _paginationMetadata = signal<PaginationMetadata | null>(null);

  public users = this._users.asReadonly();
  public paginationMetadata = this._paginationMetadata.asReadonly();

  getUsers(filters: UserFilters): void {
    this.http.get<PaginatedResult<UserDto>>(this.endpoints.users.listUsers, filters)
      .pipe(
        tap(response => {
          this._users.set(response.items || []);
          this._paginationMetadata.set(response.metadata);
        })
      )
      .subscribe();
  }

  getUserRoles(userId: string): Observable<UserRolesResponse> {
    return this.http.get<UserRolesResponse>(this.endpoints.users.userRoles(userId));
  }

  getUserAssignedRoleIds(userId: string): Observable<string[]> {
    return this.http.get<string[]>(this.endpoints.users.userRoleIds(userId));
  }

  getRoleLookups(): Observable<RoleSummaryDto[]> {
    return this.http.get<RoleSummaryDto[]>(this.endpoints.roles.lookups).pipe(map(roles => roles || []));
  }

  updateUserRoles(userId: string, roleIds: string[]): Observable<void> {
    return this.http.put<void>(this.endpoints.users.userRoles(userId), { roleIds });
  }

  updateBlockStatus(userId: string, isBlocked: boolean): Observable<void> {
    return this.http.put<void>(this.endpoints.users.blockStatus(userId), { isBlocked });
  }
}
