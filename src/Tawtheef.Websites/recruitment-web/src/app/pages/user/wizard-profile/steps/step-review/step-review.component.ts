import { Component, EventEmitter, Output, computed, inject, signal } from '@angular/core';
import { DataService } from '../../services/data.service';
import { TranslateService } from '@ngx-translate/core';
import {EndpointsService} from '../../../../../core/http/endpoints.service';
import {HttpClient} from '@angular/common/http';
import {finalize} from 'rxjs/operators';

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
  private http = inject(HttpClient);
  private endpoints = inject(EndpointsService);

  private requiredKeys = [
    'fullNameAr','fullNameEn','qid','dob',
    'country','dialCode','phone','address','email'
  ] as const;

  missing = computed(() => {
    const s = this.ds.state();
    const miss: string[] = [];
    this.requiredKeys.forEach(k => { if (!s[k]) miss.push(k as string); });
    return miss;
  });

  canSubmit = computed(() => this.missing().length === 0);

  // UI flags
  submitting = signal(false);
  submitted  = signal(false);
  errorText  = signal<string | null>(null);

  trackSkill = (_: number, v: string) => v;

  submit() {
    if (!this.canSubmit()) return;
    this.submitting.set(true);
    this.submitted.set(false);
    this.errorText.set(null);
    this.http.post(this.endpoints.user.profile.save, {
      submit: true,
      ...this.ds.state()
    }).subscribe({
      next: () => {
        this.submitted.set(true);
        this.submitting.set(false);
      },
      error: (err) => {
        this.errorText.set(
          this.i18n.instant('wizard.review.submitError') +
          (err?.error?.message ? `: ${err.error.message}` : '')
        );
        this.submitting.set(false);
      }
    });
  }
}
