import {HttpClient, HttpHeaders, HttpParams} from '@angular/common/http';
import {inject, Injectable} from '@angular/core';
import {TranslateService} from '@ngx-translate/core';
import {BehaviorSubject, Observable, of} from 'rxjs';
import {catchError, finalize, take, tap} from 'rxjs/operators';
import {NotificationModel} from '../../shared/models/notification.model';
import {EndpointsService} from '../http/endpoints.service';
import {NotificationService} from './notification.service';

@Injectable({providedIn: 'root'})
export class InAppNotificationService {
  private readonly http = inject(HttpClient);
  private readonly endpoints = inject(EndpointsService);
  private readonly notifier = inject(NotificationService);
  private readonly translate = inject(TranslateService);

  private readonly notificationsSubject = new BehaviorSubject<NotificationModel[]>([]);
  private readonly loadingSubject = new BehaviorSubject<boolean>(false);

  private readonly unreadCountSubject = new BehaviorSubject<number>(0);
  readonly notifications$: Observable<NotificationModel[]> = this.notificationsSubject.asObservable();
  readonly isLoading$: Observable<boolean> = this.loadingSubject.asObservable();
  readonly unreadCount$: Observable<number> = this.unreadCountSubject.asObservable();

  refresh(limit = 10, notifyOnError = true, unreadOnly = false, createdDateBefore?: string, append = false): Observable<NotificationModel[]> {
    let params = new HttpParams()
        .set('limit', limit)
        .set('unreadOnly', unreadOnly);
    
    if (createdDateBefore) {
        params = params.set('createdDateBefore', createdDateBefore);
    }

    this.loadingSubject.next(true);

    return this.http.get<NotificationModel[]>(this.endpoints.notifications.list, {
      params,
      headers: new HttpHeaders({ 'X-Skip-Loading': 'true' })
    }).pipe(
      tap(notifications => {
          if (append) {
              this.notificationsSubject.next([...this.notificationsSubject.value, ...notifications]);
          } else {
              this.notificationsSubject.next(notifications);
          }
          this.refreshUnreadCount().subscribe();
      }),
      catchError(() => {
        if (notifyOnError) {
          this.notifier.error(this.translate.instant('internal.nav.notifications.loadError'));
        }
        return of([]);
      }),
      finalize(() => this.loadingSubject.next(false))
    );
  }

  refreshUnreadCount(): Observable<number> {
    return this.http.get<number>(this.endpoints.notifications.unreadCount, {
      headers: new HttpHeaders({ 'X-Skip-Loading': 'true' })
    }).pipe(
      tap(count => this.unreadCountSubject.next(count)),
      catchError(() => of(0))
    );
  }

  markAsRead(id: string): Observable<void> {
    return this.http.put<void>(this.endpoints.notifications.markAsRead(id), null).pipe(
      tap(() => this.refreshUnreadCount().subscribe())
    );
  }

  markManyAsRead(notificationIds?: string[]): Observable<void> {
    return this.http.put<void>(this.endpoints.notifications.markManyAsRead, notificationIds || null).pipe(
      tap(() => this.refreshUnreadCount().subscribe())
    );
  }

  dismiss(id: string): Observable<void> {
    return this.http.put<void>(this.endpoints.notifications.dismiss(id), null).pipe(
      tap(() => this.refreshUnreadCount().subscribe())
    );
  }

  dismissMany(notificationIds?: string[]): Observable<void> {
    return this.http.put<void>(this.endpoints.notifications.dismissMany, notificationIds || null).pipe(
      tap(() => this.refreshUnreadCount().subscribe())
    );
  }

  refreshAfterToken(limit = 10): void {
    this.refresh(limit, false).pipe(take(1)).subscribe();
  }

  get currentCount(): number {
    return this.notificationsSubject.value.length;
  }
}
