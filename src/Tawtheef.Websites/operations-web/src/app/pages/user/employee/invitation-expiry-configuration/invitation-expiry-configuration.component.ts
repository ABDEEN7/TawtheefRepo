import { CommonModule } from '@angular/common';
import { Component, computed, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { InputNumber } from 'primeng/inputnumber';
import { AuthService } from '../../../../core/auth/auth.service';
import { Permissions } from '../../../../core/constants/permissions';
import { NotificationService } from '../../../../core/services/notification.service';
import { Lang, LanguageService } from '../../../../core/services/language.service';
import { I18nNamespaceDirective } from '../../../../shared/directives/i18n-namespace.directive';
import { InvitationExpiryConfiguration } from './models/invitation-expiry-configuration.model';
import { InvitationExpiryConfigurationService } from './services/invitation-expiry-configuration.service';

@Component({
  selector: 'app-invitation-expiry-configuration',
  standalone: true,
  templateUrl: './invitation-expiry-configuration.component.html',
  styleUrls: ['./invitation-expiry-configuration.component.scss'],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    TranslatePipe,
    InputNumber,
    I18nNamespaceDirective,
  ],
})
export class InvitationExpiryConfigurationComponent implements OnInit {
  private formBuilder = inject(FormBuilder);
  private service = inject(InvitationExpiryConfigurationService);
  private notification = inject(NotificationService);
  private translate = inject(TranslateService);
  private language = inject(LanguageService);
  private destroyRef = inject(DestroyRef);
  private authService = inject(AuthService);

  readonly currentLang = signal<Lang>(this.language.get());
  readonly isRtl = computed(() => this.currentLang() === 'ar');

  readonly form = this.formBuilder.group({
    expiryDays: [7, [Validators.required, Validators.min(0), Validators.max(365)]],
  });

  readonly canManage = computed(() => this.authService.hasPermission(Permissions.InvitationExpiryConfiguration.Manage));

  ngOnInit(): void {
    this.language.current$.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(lang => {
      this.currentLang.set(lang);
    });

    this.loadConfiguration();

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

    const payload = this.form.getRawValue() as InvitationExpiryConfiguration;

    this.service.saveConfiguration(payload).subscribe({
      next: () => {
        this.notification.success(this.translate.instant('common.savedSuccessfully'));
      },
    });
  }

  loadConfiguration(): void {
    this.service.getConfiguration().subscribe({
      next: configuration => {
        this.form.patchValue({ expiryDays: configuration.expiryDays ?? 7 }, { emitEvent: false });
      },
      error: () => {
        this.notification.error(this.translate.instant('common.loadFailed'));
      },
    });
  }
}
