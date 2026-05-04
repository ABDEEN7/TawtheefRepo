import { inject, Injectable } from '@angular/core';
import { EMPTY } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { EndpointsService } from '../http/endpoints.service';
import { HttpService } from '../http/http.service';

export interface NavigationAuditEvent {
  menuKey?: string | null;
  menuLabel?: string | null;
  targetUrl?: string | null;
  previousUrl?: string | null;
}

@Injectable({ providedIn: 'root' })
export class NavigationAuditService {
  private http = inject(HttpService);
  private endpoints = inject(EndpointsService);

  logSidebarNavigation(event: NavigationAuditEvent): void {
    this.http.post<void>(this.endpoints.systemAdminLogs.navigation, event)
      .pipe(catchError(() => EMPTY))
      .subscribe();
  }
}
