import { Component } from '@angular/core';
import {of} from 'rxjs';

interface Step1 {
  icon: string;
  label: { ar: string; en: string };
}

interface Step {
  icon: string;
  label: string;
}
@Component({
  selector: 'app-user-profile-wizard',
  standalone: false,
  templateUrl: './user-profile-wizard.html',
  styleUrl: './user-profile-wizard.scss',
})
export class UserProfileWizard {
  currentStep = 1;
  isAvailable = false;

  steps1: Step1[] = [
    { icon: 'fa-user', label: { ar: 'البيانات الشخصية', en: 'Personal' } },
    { icon: 'fa-house', label: { ar: 'الاتصال والسكن', en: 'Contact' } },
    { icon: 'fa-graduation-cap', label: { ar: 'المؤهلات', en: 'Education' } },
    { icon: 'fa-briefcase', label: { ar: 'الخبرات', en: 'Experience' } },
    { icon: 'fa-language', label: { ar: 'المهارات واللغات', en: 'Skills & Languages' } },
    { icon: 'fa-paperclip', label: { ar: 'المرفقات', en: 'Attachments' } },
    { icon: 'fa-check-circle', label: { ar: 'المراجعة', en: 'Review' } }
  ];

  steps: Step[] = [
    { icon: 'fa-user', label: 'البيانات الشخصية' },
    { icon: 'fa-house', label: 'الاتصال والسكن' },
    { icon: 'fa-graduation-cap', label:  'المؤهلات' },
    { icon: 'fa-briefcase', label: 'الخبرات' },
    { icon: 'fa-language', label: 'المهارات واللغات' },
    { icon: 'fa-paperclip', label: 'المرفقات' },
    { icon: 'fa-check-circle', label: 'المراجعة' }
  ];

  get progressValue(): number {
    return Math.round((this.currentStep / this.steps.length) * 100);
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
