import {HttpClient, HttpParams} from '@angular/common/http';
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

  readonly notifications$: Observable<NotificationModel[]> = this.notificationsSubject.asObservable();
  readonly isLoading$: Observable<boolean> = this.loadingSubject.asObservable();

  refresh(limit = 10, notifyOnError = true): Observable<NotificationModel[]> {
    const params = new HttpParams().set('limit', limit);
    this.loadingSubject.next(true);

    return this.http.get<NotificationModel[]>(this.endpoints.notifications.list, {params}).pipe(
      tap(notifications => this.notificationsSubject.next(notifications)),
      catchError(() => {
        if (notifyOnError) {
          this.notifier.error(this.translate.instant('internal.nav.notifications.loadError'));
        }
        return of([]);
      }),
      finalize(() => this.loadingSubject.next(false))
    );
  }

  refreshAfterToken(limit = 10): void {
    this.refresh(limit, false).pipe(take(1)).subscribe();
  }

  get currentCount(): number {
    return this.notificationsSubject.value.length;
  }
}
