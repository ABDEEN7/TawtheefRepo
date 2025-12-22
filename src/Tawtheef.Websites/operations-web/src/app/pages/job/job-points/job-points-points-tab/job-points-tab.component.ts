import { Component, Input, OnInit, EventEmitter, Output, inject } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { JobResponse } from '../../models/job-response-model';

interface DetailItem {
  key: string;
  label: string;
  id?: string;
}

@Component({
  selector: 'app-job-points-tab',
  standalone: false,
  templateUrl: './job-points-tab.component.html',
  styleUrls: ['./job-points-tab.component.scss']
})
export class JobPointsTabComponent implements OnInit {
  @Input() mainForm!: FormGroup;
  @Input() form!: FormGroup;
  @Input() job: JobResponse | null = null;

  @Output() save = new EventEmitter<void>();
  @Output() ready = new EventEmitter<void>();

  private fb = inject(FormBuilder);

  // FormGroups
  applicantCategoryFormGroup!: FormGroup;
  educationFormGroup!: FormGroup;
  experienceFormGroup!: FormGroup;
  trainingFormGroup!: FormGroup;
  skillsFormGroup!: FormGroup;
  languagesFormGroup!: FormGroup;
  certificatesFormGroup!: FormGroup;

  // Items
  applicantCategoryItems: DetailItem[] = [
    { key: 'qatari', label: 'JOB_POINTS.APPLICANT_CATEGORY.QATARI' },
    { key: 'qatarMother', label: 'JOB_POINTS.APPLICANT_CATEGORY.QATAR_MOTHER' },
    { key: 'gcc', label: 'JOB_POINTS.APPLICANT_CATEGORY.GCC' },
    { key: 'qatarGraduate', label: 'JOB_POINTS.APPLICANT_CATEGORY.QATAR_GRADUATE' },
    { key: 'qatarGraduatePrev', label: 'JOB_POINTS.APPLICANT_CATEGORY.QATAR_GRADUATE_PREV' }
  ];

  trainingItems: DetailItem[] = [
    { key: 'highLinked', label: 'JOB_POINTS.TRAINING.HIGH_LINKED' },
    { key: 'mediumLinked', label: 'JOB_POINTS.TRAINING.MEDIUM_LINKED' },
    { key: 'lowLinked', label: 'JOB_POINTS.TRAINING.LOW_LINKED' }
  ];

  experienceItems: DetailItem[] = [
    { key: 'pointsPerYear', label: 'JOB_POINTS.EXPERIENCE.POINTS_PER_YEAR' },
    { key: 'maxYears', label: 'JOB_POINTS.EXPERIENCE.MAX_YEARS' }
  ];

  educationItems: DetailItem[] = [];
  skillsItems: DetailItem[] = [];
  languagesItems: DetailItem[] = [
    { key: 'excellent', label: 'JOB_POINTS.LANGUAGE_LEVEL.EXCELLENT' },
    { key: 'veryGood', label: 'JOB_POINTS.LANGUAGE_LEVEL.VERY_GOOD' },
    { key: 'good', label: 'JOB_POINTS.LANGUAGE_LEVEL.GOOD' }
  ];

  certificatesItems: DetailItem[] = [
    { key: 'certificates', label: 'JOB_POINTS.CERTIFICATES' }
  ];

  ngOnInit(): void {
    // Dynamic items from job
    this.skillsItems = this.job?.skills.map(s => ({
      key: s.skill.backendName,
      label: s.skill.name,
      id: s.skill.id
    })) || [];

    this.educationItems = this.job?.degrees.map(d => ({
      key: d.degree.backendName,
      label: d.degree.name,
      id: d.degree.id
    })) || [];

    // Assign form groups
    this.applicantCategoryFormGroup = this.form.get('applicantCategory') as FormGroup;
    this.educationFormGroup = this.form.get('education') as FormGroup;
    this.experienceFormGroup = this.form.get('experience') as FormGroup;
    this.trainingFormGroup = this.form.get('training') as FormGroup;
    this.skillsFormGroup = this.form.get('skills') as FormGroup;
    this.languagesFormGroup = this.form.get('languages') as FormGroup;
    this.certificatesFormGroup = this.form.get('certificates') as FormGroup;

    this.initControls();
    this.ready.emit();
  }

  private initControls(): void {
    this.addControls(this.applicantCategoryFormGroup, this.applicantCategoryItems);
    this.addControls(this.educationFormGroup, this.educationItems);
    this.addControls(this.experienceFormGroup, this.experienceItems);
    this.addControls(this.trainingFormGroup, this.trainingItems);
    this.addControls(this.skillsFormGroup, this.skillsItems);
    this.addControls(this.languagesFormGroup, this.languagesItems);
    this.addControls(this.certificatesFormGroup, this.certificatesItems);
  }

  private addControls(group: FormGroup, items: DetailItem[]): void {
    items.forEach(item => {
      if (!group.get(item.key)) {
        group.addControl(item.key, this.fb.control(0));
      }
    });
  }

  // ---------------------- Totals ----------------------
  get experienceTotal(): number {
    const exp = this.experienceFormGroup?.getRawValue();
    return (exp?.pointsPerYear || 0) * (exp?.maxYears || 0);
  }

  sumCategory(group: FormGroup | null): number {
    if (!group) return 0;
    return Object.keys(group.controls)
      .filter(k => !['pointsPerYear', 'maxYears', 'total'].includes(k))
      .reduce((sum, k) => sum + (group.get(k)?.value || 0), 0);
  }

  applicantCategorySum() { return this.sumCategory(this.applicantCategoryFormGroup); }
  educationSum() { return this.sumCategory(this.educationFormGroup); }
  trainingSum() { return this.sumCategory(this.trainingFormGroup); }
  skillsSum() { return this.sumCategory(this.skillsFormGroup); }
  languagesSum() { return this.sumCategory(this.languagesFormGroup); }
  certificatesSum() { return this.sumCategory(this.certificatesFormGroup); }

  // ---------------------- Validation ----------------------
  areAllCategoriesValid(): boolean {
    return (
      this.applicantCategorySum() === (this.mainForm.get('applicantCategory')?.value || 0) &&
      this.educationSum() === (this.mainForm.get('education')?.value || 0) &&
      this.experienceTotal === (this.mainForm.get('experience')?.value || 0) &&
      this.trainingSum() === (this.mainForm.get('training')?.value || 0) &&
      this.skillsSum() === (this.mainForm.get('skills')?.value || 0) &&
      this.languagesSum() === (this.mainForm.get('languages')?.value || 0) &&
      this.certificatesSum() === (this.mainForm.get('certificates')?.value || 0)
    );
  }

  saveDetails(): void {
    if (!this.areAllCategoriesValid()) return;
    this.save.emit();
  }

  maxValue(groupName: string): number {
    return this.mainForm.get(groupName)?.value || 0;
  }
}
