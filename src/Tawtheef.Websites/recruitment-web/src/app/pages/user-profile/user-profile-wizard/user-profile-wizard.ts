import { Component } from '@angular/core';
import {of} from 'rxjs';

interface Step {
  icon: string;
  label: { ar: string; en: string };
}
@Component({
  selector: 'app-user-profile-wizard',
  standalone: false,
  templateUrl: './user-profile-wizard.html',
  styleUrl: './user-profile-wizard.scss',
})
export class UserProfileWizard {
  currentLang: 'ar' | 'en' = 'ar';
  currentStep = 1;
  isAvailable = false;

  steps: Step[] = [
    { icon: 'fa-user', label: { ar: 'البيانات الشخصية', en: 'Personal' } },
    { icon: 'fa-house', label: { ar: 'الاتصال والسكن', en: 'Contact' } },
    { icon: 'fa-graduation-cap', label: { ar: 'المؤهلات', en: 'Education' } },
    { icon: 'fa-briefcase', label: { ar: 'الخبرات', en: 'Experience' } },
    { icon: 'fa-language', label: { ar: 'المهارات واللغات', en: 'Skills & Languages' } },
    { icon: 'fa-paperclip', label: { ar: 'المرفقات', en: 'Attachments' } },
    { icon: 'fa-check-circle', label: { ar: 'المراجعة', en: 'Review' } }
  ];

  get progressValue(): number {
    return Math.round((this.currentStep / this.steps.length) * 100);
  }

  toggleLanguage(): void {
    this.currentLang = this.currentLang === 'ar' ? 'en' : 'ar';
    document.documentElement.lang = this.currentLang;
    document.documentElement.dir = this.currentLang === 'ar' ? 'rtl' : 'ltr';
  }

  setStep(step: number): void {
    this.currentStep = step;
  }

  nextStep(): void {
    if (this.currentStep < this.steps.length) {
      this.currentStep++;
    }
  }

  prevStep(): void {
    if (this.currentStep > 1) {
      this.currentStep--;
    }
  }

  protected readonly of = of;
}
