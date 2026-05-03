import { inject, Injectable } from '@angular/core';
import { UserDto } from '../models/user.dto';
import { UserFilters } from '../models/user-filters.dto';
import { UserRolesResponse } from '../models/user-roles-response.dto';
import { map } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { RoleSummaryDto } from '../models/role-summary.dto';
import { PaginatedResult } from '../../../../../core/models/paginated-result.model';
import { HttpService } from '../../../../../core/http/http.service';
import { EndpointsService } from '../../../../../core/http/endpoints.service';
import { dropdownOptionsModel } from '../../../../../shared/models/dropdown-options.model';

@Injectable({ providedIn: 'root' })
export class UsersService {
  private http = inject(HttpService);
  private endpoints = inject(EndpointsService);

  getUsers(filters: UserFilters): Observable<PaginatedResult<UserDto>> {
    return this.http.get<PaginatedResult<UserDto>>(this.endpoints.users.listUsers, filters)
      .pipe(
        map(response => {
          const users = (response.items || []).map(user => ({
            ...user,
            roles: user.roles ?? [],
            roleNames: user.roleNames ?? (user.roles ?? []).map(r => r.nameEn || r.nameAr)
          }));

          return {
            ...response,
            items: users
          };
        })
      );
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
