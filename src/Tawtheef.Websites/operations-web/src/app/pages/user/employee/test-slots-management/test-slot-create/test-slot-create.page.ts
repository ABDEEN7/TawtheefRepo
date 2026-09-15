import { Component, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { ButtonModule } from 'primeng/button';
import { DialogService } from 'primeng/dynamicdialog';
import { catchError, finalize, of } from 'rxjs';
import { LanguageService } from '../../../../../core/services/language.service';
import { portalRoutes } from '../../../../../routes/portal-routes';
import { ConfirmationDialogComponent } from '../../../../../shared/dialogs/confirmation-dialog/confirmation-dialog.component';
import { FaDirArrowDirective } from '../../../../../shared/directives/dir-arrow.directive';
import { I18nNamespaceDirective } from '../../../../../shared/directives/i18n-namespace.directive';
import { dropdownOptionsModel } from '../../../../../shared/models/dropdown-options.model';
import { TestSlotsService } from '../services/test-slots.service';
import { SlotDetailsComponent } from './steps/slot-details/slot-details.component';
import { testSlotCreateForm } from './helper/test-slot-create.form';
import { validateSlotDetails } from './helper/test-slot-create.validation';

@Component({
  selector: 'app-test-slot-create-page',
  standalone: true,
  templateUrl: './test-slot-create.page.html',
  styleUrl: './test-slot-create.page.scss',
  imports: [
    ReactiveFormsModule,
    TranslatePipe,
    ButtonModule,
    FaDirArrowDirective,
    I18nNamespaceDirective,
    SlotDetailsComponent,
  ],
  providers: [DialogService],
})
export class TestSlotCreatePage implements OnInit {
  private readonly service = inject(TestSlotsService);
  private readonly router = inject(Router);
  private readonly translate = inject(TranslateService);
  private readonly dialogs = inject(DialogService);
  private readonly destroyRef = inject(DestroyRef);
  readonly language = inject(LanguageService);
  readonly form = testSlotCreateForm();
  readonly rooms = signal<dropdownOptionsModel[]>([]);
  readonly wizardSteps = [
    { id: 1, label: 'TEST_SLOT_WIZARD.SLOT_DETAILS', icon: 'hgi hgi-stroke hgi-calendar-03' },
    { id: 2, label: 'TEST_SLOT_WIZARD.TEAM_ASSIGNMENT', icon: 'hgi hgi-stroke hgi-user-group' },
    { id: 3, label: 'TEST_SLOT_WIZARD.REVIEW', icon: 'hgi hgi-stroke hgi-clipboard-check-01' },
  ];
  step = 1;
  loading = true;
  loadFailed = false;
  errors: string[] = [];

  ngOnInit(): void {
    this.language.current$
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(language => this.loadRooms(language));
  }

  goTo(target: number): void {
    if (target < 1 || target > this.wizardSteps.length) return;
    this.errors = [];

    if (target > this.step && this.step === 1) {
      this.form.markAllAsTouched();
      this.errors = validateSlotDetails(this.form.getRawValue());
    }

    if (!this.errors.length) this.step = target;
  }

  cancel(): void {
    if (!this.form.dirty) {
      void this.router.navigateByUrl(portalRoutes.testSlotsManagement);
      return;
    }

    this.dialogs
      .open(ConfirmationDialogComponent, {
        header: this.translate.instant('TEST_SLOT_WIZARD.CANCEL'),
        width: 'min(32rem, 95vw)',
        modal: true,
        data: { type: 'warning', description: 'TEST_SLOT_WIZARD.LEAVE_MESSAGE' },
      })
      ?.onClose.pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(confirmed => {
        if (confirmed === true) void this.router.navigateByUrl(portalRoutes.testSlotsManagement);
      });
  }

  private loadRooms(language: string): void {
    this.loading = true;
    this.loadFailed = false;
    this.service
      .getAvailableRoomsForTestSlotWizard(language)
      .pipe(
        catchError(() => {
          this.loadFailed = true;
          return of([]);
        }),
        finalize(() => (this.loading = false)),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe(rooms => this.rooms.set(rooms));
  }
}
