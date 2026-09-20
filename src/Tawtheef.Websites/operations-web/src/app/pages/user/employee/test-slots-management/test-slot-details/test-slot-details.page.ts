import { Component, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { ButtonModule } from 'primeng/button';
import { catchError, of, switchMap, take } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { LanguageService } from '../../../../../core/services/language.service';
import { Permissions } from '../../../../../core/constants/permissions';
import { portalRoutes } from '../../../../../routes/portal-routes';
import { HasPermissionDirective } from '../../../../../shared/directives/has-permission.directive';
import { I18nNamespaceDirective } from '../../../../../shared/directives/i18n-namespace.directive';
import { FaDirArrowDirective } from '../../../../../shared/directives/dir-arrow.directive';
import { TestSlotDetailsDto } from '../models/test-slot-details.dto';
import { TestSlotsService } from '../services/test-slots.service';
import { TestSlotBasicInfoComponent } from './test-slot-basic-info/test-slot-basic-info.component';
import { TestSlotCandidatesComponent } from './test-slot-candidates/test-slot-candidates.component';
import { TestSlotAssignmentsComponent } from './test-slot-assignments/test-slot-assignments.component';
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
  private readonly service = inject(TestSlotsService);
  private readonly route = inject(ActivatedRoute);
  private readonly language = inject(LanguageService);
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

  private loadDetailsRequest() {
    return this.service.details(this.id!, this.language.get());
  }
}
