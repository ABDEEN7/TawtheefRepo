import { CommonModule } from '@angular/common';
import { Component, computed, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { InputNumber } from 'primeng/inputnumber';
import { JobPointsConfigurationService } from './services/job-points-configuration.service';
import { JobPointConfiguration } from '../job-management/models/job-points-config';
import { I18nNamespaceDirective } from '../../../../shared/directives/i18n-namespace.directive';
import { NotificationService } from '../../../../core/services/notification.service';
import { Lang, LanguageService } from '../../../../core/services/language.service';
import { routes } from '../../../../routes/routes';
import { AuthService } from '../../../../core/auth/auth.service';
import { Permissions } from '../../../../core/constants/permissions';

@Component({
  selector: 'app-job-points-configuration',
  standalone: true,
  templateUrl: './job-points-configuration.component.html',
  styleUrls: ['./job-points-configuration.component.scss'],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    TranslatePipe,
    InputNumber,
    I18nNamespaceDirective,
  ],
})
export class JobPointsConfigurationComponent implements OnInit {
  private formBuilder = inject(FormBuilder);
  private service = inject(JobPointsConfigurationService);
  private notification = inject(NotificationService);
  private translate = inject(TranslateService);
  private language = inject(LanguageService);
  private destroyRef = inject(DestroyRef);
  private router = inject(Router);
  private authService = inject(AuthService);
  protected readonly Permissions = Permissions;

  readonly canManage = computed(() => this.authService.hasPermission(Permissions.JobPointsConfiguration.Manage));

  readonly currentLang = signal<Lang>(this.language.get());
  readonly isRtl = computed(() => this.currentLang() === 'ar');

  readonly form = this.formBuilder.group({
    applicantCategoryMaxPoints: [0, [Validators.required, Validators.min(0)]],
    educationMaxPoints: [0, [Validators.required, Validators.min(0)]],
    experienceMaxPoints: [0, [Validators.required, Validators.min(0)]],
    trainingMaxPoints: [0, [Validators.required, Validators.min(0)]],
    certificatesMaxPoints: [0, [Validators.required, Validators.min(0)]],
    skillsMaxPoints: [0, [Validators.required, Validators.min(0)]],
    languagesMaxPoints: [0, [Validators.required, Validators.min(0)]],
    maxPoints: [{ value: 0, disabled: true }, [Validators.required, Validators.min(0)]],
  });

  readonly pointKeys = [
    'applicantCategoryMaxPoints',
    'educationMaxPoints',
    'experienceMaxPoints',
    'trainingMaxPoints',
    'certificatesMaxPoints',
    'skillsMaxPoints',
    'languagesMaxPoints',
  ] as const;

  ngOnInit(): void {
    this.language.current$.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(lang => {
      this.currentLang.set(lang);
    });

    this.form.valueChanges.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(() => {
      this.updateMaxPoints();
    });

    this.loadConfiguration();
  }

  preventPaste(event: ClipboardEvent): void {
    event.preventDefault();
  }

  save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const payload = this.form.getRawValue() as JobPointConfiguration;

    this.service.saveConfiguration(payload).subscribe({
      next: () => {
        this.notification.success(this.translate.instant('common.savedSuccessfully'));
        this.router.navigate([routes.portal.JobList]);
      },
    });
  }

  public loadConfiguration(): void {
    this.service.getConfiguration().subscribe({
      next: configuration => {
        this.patchForm(configuration);
        if (!this.canManage()) {
          this.form.disable();
        }
      },
      error: () => {
        this.notification.error(this.translate.instant('common.loadFailed'));
      },
    });
  }

  private patchForm(configuration: JobPointConfiguration): void {
    this.form.patchValue(
      {
        applicantCategoryMaxPoints: configuration.applicantCategoryMaxPoints ?? 0,
        educationMaxPoints: configuration.educationMaxPoints ?? 0,
        experienceMaxPoints: configuration.experienceMaxPoints ?? 0,
        trainingMaxPoints: configuration.trainingMaxPoints ?? 0,
        certificatesMaxPoints: configuration.certificatesMaxPoints ?? 0,
        skillsMaxPoints: configuration.skillsMaxPoints ?? 0,
        languagesMaxPoints: configuration.languagesMaxPoints ?? 0,
      },
      { emitEvent: false }
    );

    this.updateMaxPoints();
  }

  private updateMaxPoints(): void {
    const total = this.pointKeys.reduce((sum, key) => {
      const value = this.form.get(key)?.value ?? 0;
      return sum + Number(value);
    }, 0);

    this.form.patchValue({ maxPoints: total }, { emitEvent: false });
  }
}
