import { Component, EventEmitter, Output, computed, inject, signal } from '@angular/core';
import { DataService } from '../../services/data.service';
import { TranslateService } from '@ngx-translate/core';
import {finalize} from 'rxjs/operators';
import {ProfileService} from '../../services/profile.service';
import {Skill} from '../../models/skill.model';
import {CandidateType} from '../../../../../core/enums/lookups.enum';
import {FileUtilsService} from '../../../../../core/utils/file-utils';
import {createStepValiditySignal} from '../../state/profile-step-validity.signal';
import {UploadedFileRef} from '../../models/profile-state.model';
import {NavigationService} from '../../../../../core/services/navigation.service';
import {Router} from '@angular/router';
import {routes} from '../../../../../routes/routes';

@Component({
  selector: 'app-step-review',
  templateUrl: './step-review.component.html',
  styleUrl: './step-review.component.scss',
  standalone: false
})
export class StepReviewComponent {
  @Output() back = new EventEmitter<void>();
  router = inject(Router);
  ds = inject(DataService);
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
    const type = this.ds.state().candidateType?.backendName as CandidateType | undefined;
    if (!type) return false;

    return [
      CandidateType.ResidentQatar,
      CandidateType.Qatari,
      CandidateType.SonOfQatariMother,
      CandidateType.WifeOfQatari
    ].includes(type);
  }
  hasNationalAddress = computed(() => {
    const s = this.ds.state();
    return this.isResidentQatar && !!(s.naZone || s.naStreet || s.naBuilding || s.naUnit || s.naFileName);
  });

  hasSponsor = computed(() => {
    const s = this.ds.state();
    return !!(s.sponsorType || s.sponsorEmployerName || s.sponsorEmployerNumber || s.sponsorCardName);
  });

  // UI flags
  submitting = signal(false);
  submitted  = signal(false);
  errorText  = signal<string | null>(null);

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

  submit() {
    if (!this.canSubmit()) return;
    this.submitting.set(true);
    this.submitted.set(false);
    this.errorText.set(null);
    this.profile
      .finalizeProfile()
      .pipe(finalize(() => this.submitting.set(false)))
      .subscribe({
        next: () => {
          this.submitted.set(true);
          this.router.navigate([routes.user.dashboard]);
        },
        error: (err) => {
          this.errorText.set(
            this.i18n.instant('wizard.review.submitError') +
            (err?.error?.message ? `: ${err.error.message}` : '')
          );
        }
      });
  }
}
