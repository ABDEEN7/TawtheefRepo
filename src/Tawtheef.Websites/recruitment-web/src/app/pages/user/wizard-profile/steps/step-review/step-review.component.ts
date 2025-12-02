import { Component, EventEmitter, Output, computed, inject, signal } from '@angular/core';
import { DataService } from '../../services/data.service';
import { TranslateService } from '@ngx-translate/core';
import {finalize} from 'rxjs/operators';
import {ProfileService} from '../../services/profile.service';
import {Skill} from '../../models/skill.model';
import {CandidateType} from '../../../../../core/enums/lookups.enum';

@Component({
  selector: 'app-step-review',
  templateUrl: './step-review.component.html',
  styleUrl: './step-review.component.scss',
  standalone: false
})
export class StepReviewComponent {
  savingDraft = false;
  @Output() back = new EventEmitter<void>();
  ds = inject(DataService);
  private i18n = inject(TranslateService);
  private profile = inject(ProfileService);

  private requiredKeys = [
    'fullNameAr','fullNameEn','qid','dob',
    'country','phone','address','email'
  ] as const;

  missing = computed(() => {
    const s = this.ds.state();
    const miss: string[] = [];
    this.requiredKeys.forEach(k => { if (!s[k]) miss.push(k as string); });
    return miss;
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
