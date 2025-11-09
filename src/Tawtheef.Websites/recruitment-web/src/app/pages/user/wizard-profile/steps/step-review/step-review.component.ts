import { Component, EventEmitter, Output, computed, inject, signal } from '@angular/core';
import { DataService } from '../../services/data.service';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-step-review',
  templateUrl: './step-review.component.html',
  styleUrl: './step-review.component.scss',
  standalone: false
})
export class StepReviewComponent {
  @Output() back = new EventEmitter<void>();
  ds = inject(DataService);
  private i18n = inject(TranslateService);

  private requiredKeys = [
    'fullName','fullNameEn','qid','dob',
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

  async submit() {
    if (!this.canSubmit()) return;
    this.submitting.set(true);
    this.errorText.set(null);

    try {
      // مثال: نداء API حقيقي هنا
      // await this.http.post('/api/profile', this.ds.state()).toPromise();

      setTimeout(() => {
        this.submitting.set(false);
        this.submitted.set(true);
      }, 400);
    } catch {
      this.submitting.set(false);
      this.errorText.set(this.i18n.instant('wizard.review.errorGeneric'));
    }
  }
}
