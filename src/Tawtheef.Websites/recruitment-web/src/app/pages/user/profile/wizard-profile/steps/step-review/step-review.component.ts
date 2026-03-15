import { Component, EventEmitter, Output, computed, inject, signal, output, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { finalize, tap } from 'rxjs/operators';
import { Router } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { ProfileDataService } from '../../services/profile-data.service';
import { ProfileService } from '../../services/profile.service';
import { Skill } from '../../models/skill.model';
import { CandidateType } from '../../../../../../core/enums/lookups.enum';
import { FileUtilsService } from '../../../../../../core/utils/file-utils';
import { createStepValiditySignal } from '../../state/profile-step-validity.signal';
import { UploadedFileRef } from '../../models/profile-state.model';
import { routes } from '../../../../../../routes/routes';
import { AuthService } from '../../../../../../core/auth/auth.service';

@Component({
  selector: 'app-step-review',
  templateUrl: './step-review.component.html',
  styleUrl: './step-review.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    CommonModule,
    TranslatePipe,
    ButtonModule,
    TableModule
  ]
})
export class StepReviewComponent {
  back = output<void>();
  router = inject(Router);
  ds = inject(ProfileDataService);
  private i18n = inject(TranslateService);
  private profile = inject(ProfileService);
  private fileUtils = inject(FileUtilsService);

  private stepValidity = createStepValiditySignal(this.ds.state);

  missing = computed(() => {
    const validity = this.stepValidity();
    const set = new Set<string>();
    Object.values(validity).forEach(result => {
      result.errors?.forEach(err => set.add(err.i18nKey));
    });
    return Array.from(set);
  });

  canSubmit = computed(() => this.missing().length === 0);

  get isResidentQatar(): boolean {
    const provider = this.ds.state().provider;
    return ['QatarPass', 'QatarResidentOtp'].includes(provider);
  }

  hasSponsor = computed(() => {
    const s = this.ds.state();
    return !!(s.sponsorType || s.sponsorEmployerName || s.sponsorEmployerNumber || s.sponsorCardName);
  });

  // UI flags
  submitting = signal(false);
  submitted = signal(false);
  errorText = signal<string | null>(null);

  trackSkill = (_: number, v: Skill) => v.skillId;

  preview(ref?: UploadedFileRef | null, file?: File | null, fallbackName?: string | null, ev?: Event): void {
    ev?.stopPropagation();
    if (file) {
      this.fileUtils.previewBlob(file);
      return;
    }

    if (ref?.url) {
      this.fileUtils.previewUrl(ref.url, ref.resourceName || fallbackName || '', false);
    }
  }


  private auth = inject(AuthService);

  submit() {
    if (!this.canSubmit()) return;

    this.submitting.set(true);
    this.submitted.set(false);
    this.errorText.set(null);

    this.profile.finalizeProfile().pipe(
      tap(() => {
        this.submitted.set(true);
        // ✅ clear bootstrap cache because status changed
        this.auth.resetBootstrap();
      }),
      finalize(() => this.submitting.set(false))
    ).subscribe({
      next: () => {
        // ✅ Refresh token to update "profile.completed" claim
        this.auth.refreshToken().subscribe(() => {
          // ✅ redirect to dashboard with fresh state
          this.router.navigate([routes.user.dashboard], { replaceUrl: true });
        });
      },
      error: (err: unknown) => {
        this.errorText.set(typeof err === 'string' ? err : 'wizard.review.errorGeneric');
      }
    });
  }
}
