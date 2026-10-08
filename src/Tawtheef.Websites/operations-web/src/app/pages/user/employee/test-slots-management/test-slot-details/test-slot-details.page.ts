import { Component, computed, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { ButtonModule } from 'primeng/button';
import { catchError, finalize, of, switchMap, take } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { LanguageService } from '../../../../../core/services/language.service';
import { NotificationService } from '../../../../../core/services/notification.service';
import { DialogHelperService } from '../../../../../core/services/dialog-helper.service';
import { Permissions } from '../../../../../core/constants/permissions';
import { portalRoutes } from '../../../../../routes/portal-routes';
import { HasPermissionDirective } from '../../../../../shared/directives/has-permission.directive';
import { I18nNamespaceDirective } from '../../../../../shared/directives/i18n-namespace.directive';
import { FaDirArrowDirective } from '../../../../../shared/directives/dir-arrow.directive';
import { TestSlotDetailsDto, TestSlotSessionDto } from '../models/test-slot-details.dto';
import { TestSlotsService } from '../services/test-slots.service';
import { TestSlotBasicInfoComponent } from './components/test-slot-basic-info/test-slot-basic-info.component';
import { TestSlotCandidatesComponent } from './components/test-slot-candidates/test-slot-candidates.component';
import { TestSlotAssignmentsComponent } from './components/test-slot-assignments/test-slot-assignments.component';
import { TestSlotSessionsComponent } from '../../test-sessions/components/test-slot-sessions/test-slot-sessions.component';

@Component({
  selector: 'app-test-slot-details-page',
  standalone: true,
  templateUrl: './test-slot-details.page.html',
  styleUrl: './test-slot-details.page.scss',
  imports: [
    CommonModule,
    RouterLink,
    TranslatePipe,
    ButtonModule,
    I18nNamespaceDirective,
    FaDirArrowDirective,
    HasPermissionDirective,
    TestSlotBasicInfoComponent,
    TestSlotCandidatesComponent,
    TestSlotAssignmentsComponent,
    TestSlotSessionsComponent,
  ],
})
export class TestSlotDetailsPage implements OnInit {
  readonly routes = portalRoutes;
  readonly permissions = Permissions;
  readonly details = signal<TestSlotDetailsDto | null>(null);
  readonly loading = signal(true);
  readonly failed = signal(false);
  readonly revealedAccessCode = signal<string | null>(null);
  readonly starting = signal(false);
  readonly closing = signal(false);
  readonly sendingNotifications = signal(false);
  readonly canStart = computed(() => this.details()?.status.backendName === 'Ready');
  readonly canClose = computed(() => this.details()?.status.backendName === 'Started');
  private readonly service = inject(TestSlotsService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly language = inject(LanguageService);
  private readonly notifications = inject(NotificationService);
  private readonly dialogs = inject(DialogHelperService);
  private readonly translate = inject(TranslateService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly id = this.route.snapshot.paramMap.get('testSlotId');
  ngOnInit(): void {
    if (!this.id) {
      this.failed.set(true);
      this.loading.set(false);
      return;
    }
    this.language.current$
      .pipe(
        switchMap(() => this.loadDetailsRequest()),
        catchError(() => {
          this.failed.set(true);
          return of(null);
        }),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe((details) => {
        this.details.set(details);
        this.loading.set(false);
      });
  }

  loadDetails(): void {
    this.loading.set(true);
    this.failed.set(false);
    this.loadDetailsRequest().pipe(take(1), takeUntilDestroyed(this.destroyRef)).subscribe({
      next: details => {
        this.details.set(details);
        this.loading.set(false);
      },
      error: () => {
        this.failed.set(true);
        this.loading.set(false);
      },
    });
  }

  viewTestSession(testSessionId: string): void {
    void this.router.navigate([], {
      relativeTo: this.route,
      queryParams: { testSessionId },
      queryParamsHandling: 'merge',
    });
  }

  toggleAccessCode(): void {
    if (this.revealedAccessCode()) {
      this.revealedAccessCode.set(null);
      return;
    }
    if (!this.id) return;
    this.service
      .accessCode(this.id)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({ next: (result) => this.revealedAccessCode.set(result.accessCode) });
  }

  startPeriod(): void {
    const details = this.details();
    if (!details || !details.isCurrentUserRoomHead || !this.canStart() || this.starting()) return;

    this.starting.set(true);
    this.service
      .start(details.id)
      .pipe(finalize(() => this.starting.set(false)), takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => {
          this.notifications.success(this.translate.instant('TEST_SLOT_DETAILS.STARTED'));
          this.loadDetails();
        },
      });
  }

  closePeriod(): void {
    const details = this.details();
    if (!details || !details.isCurrentUserRoomHead || !this.canClose() || this.closing()) return;

    this.closing.set(true);
    const dialogRef = this.dialogs.openConfirmDialog({
      type: 'warning',
      primaryActionStyle: true,
      title: 'TEST_SLOT_DETAILS.CLOSE_PERIOD',
      description: 'TEST_SLOT_DETAILS.CLOSE_PERIOD_CONFIRMATION',
    });
    if (!dialogRef) {
      this.closing.set(false);
      return;
    }

    dialogRef.onClose.pipe(takeUntilDestroyed(this.destroyRef)).subscribe((confirmed) => {
      if (confirmed !== true) {
        this.closing.set(false);
        return;
      }
      this.submitClosePeriod(details.id);
    });
  }

  confirmCandidateNotifications(session?: Pick<TestSlotSessionDto, 'id' | 'sessionNo'>): void {
    const testSlotId = this.details()?.id;
    if (!testSlotId || this.sendingNotifications()) return;

    const dialogRef = this.dialogs.openConfirmDialog({
      type: 'submit',
      title: 'TEST_SLOT_DETAILS.CONFIRM_NOTIFICATION_TITLE',
      description: session
        ? 'TEST_SLOT_DETAILS.CONFIRM_SESSION_NOTIFICATION'
        : 'TEST_SLOT_DETAILS.CONFIRM_ALL_NOTIFICATIONS',
      descriptionParams: session ? { sessionNo: session.sessionNo } : undefined,
    });
    dialogRef?.onClose.pipe(take(1), takeUntilDestroyed(this.destroyRef)).subscribe((confirmed) => {
      if (confirmed === true) this.submitCandidateNotifications(testSlotId, session?.id ?? null);
    });
  }

  private submitCandidateNotifications(testSlotId: string, testSessionId: string | null): void {
    this.sendingNotifications.set(true);
    this.service
      .sendCandidateNotifications(testSlotId, testSessionId)
      .pipe(finalize(() => this.sendingNotifications.set(false)), takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => {
          const message = testSessionId
            ? 'TEST_SLOT_DETAILS.SESSION_NOTIFICATIONS_SENT'
            : 'TEST_SLOT_DETAILS.ALL_NOTIFICATIONS_SENT';
          this.notifications.success(this.translate.instant(message));
        },
        error: () => this.notifications.error(),
      });
  }

  private submitClosePeriod(id: string): void {
    this.service
      .close(id)
      .pipe(finalize(() => this.closing.set(false)), takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => {
          this.notifications.success(this.translate.instant('TEST_SLOT_DETAILS.CLOSED'));
          this.loadDetails();
        },
      });
  }

  private loadDetailsRequest() {
    return this.service.details(this.id!, this.language.get());
  }
}
