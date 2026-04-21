import { CommonModule } from '@angular/common';
import { Component, computed, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { InputNumber } from 'primeng/inputnumber';
import { JobCategoryCandidateSettingsService } from './services/job-category-candidate-settings.service';
import { JobCategoryCandidateSettings } from './models/job-category-candidate-settings.model';
import { I18nNamespaceDirective } from '../../../../shared/directives/i18n-namespace.directive';
import { NotificationService } from '../../../../core/services/notification.service';
import { Lang, LanguageService } from '../../../../core/services/language.service';
import { AuthService } from '../../../../core/auth/auth.service';
import { Permissions } from '../../../../core/constants/permissions';

@Component({
  selector: 'app-job-category-candidate-settings',
  standalone: true,
  templateUrl: './job-category-candidate-settings.component.html',
  styleUrls: ['./job-category-candidate-settings.component.scss'],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    TranslatePipe,
    InputNumber,
    I18nNamespaceDirective,
  ],
})
export class JobCategoryCandidateSettingsComponent implements OnInit {
  private formBuilder = inject(FormBuilder);
  private service = inject(JobCategoryCandidateSettingsService);
  private notification = inject(NotificationService);
  private translate = inject(TranslateService);
  private language = inject(LanguageService);
  private destroyRef = inject(DestroyRef);
  private authService = inject(AuthService);

  readonly currentLang = signal<Lang>(this.language.get());
  readonly isRtl = computed(() => this.currentLang() === 'ar');

  readonly form = this.formBuilder.group({
    academicJobVacancies: [0, [Validators.required, Validators.min(0)]],
    laborJobVacancies: [0, [Validators.required, Validators.min(0)]],
    administrativeJobVacancies: [0, [Validators.required, Validators.min(0)]],
  });

  readonly canManage = computed(() => this.authService.hasPermission(Permissions.JobCategoryCandidateSettings.Manage));

  ngOnInit(): void {
    this.language.current$.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(lang => {
      this.currentLang.set(lang);
    });

    this.loadSettings();

    if (!this.canManage()) {
      this.form.disable();
    }
  }

  preventPaste(event: ClipboardEvent): void {
    event.preventDefault();
  }

  save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const payload = this.form.getRawValue() as JobCategoryCandidateSettings;

    this.service.saveSettings(payload).subscribe({
      next: () => {
        this.notification.success(this.translate.instant('common.savedSuccessfully'));
      },
    });
  }

  public loadSettings(): void {
    this.service.getSettings().subscribe({
      next: settings => {
        this.form.patchValue(
          {
            academicJobVacancies: settings.academicJobVacancies ?? 0,
            laborJobVacancies: settings.laborJobVacancies ?? 0,
            administrativeJobVacancies: settings.administrativeJobVacancies ?? 0,
          },
          { emitEvent: false }
        );
      },
      error: () => {
        this.notification.error(this.translate.instant('common.loadFailed'));
      },
    });
  }
}
