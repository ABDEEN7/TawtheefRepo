import { Component, Input, OnInit, EventEmitter, Output, inject, output } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { JobResponse } from '../../models/job-response-model';
import { DetailItem } from '../../models/detail-Item';
import { JobPointsCalculationService } from '../../services/job-points-calculation.service';
import {
  APPLICANT_CATEGORY_ITEMS,
  CERTIFICATES_ITEMS,
  EDUCATION_ITEMS,
  EXPERIENCE_ITEMS,
  LANGUAGE_ITEMS,
  SKILLS_ITEMS,
  TRAINING_ITEMS,
} from '../../constants/job-points-constants';
import { LanguageAbilityType } from '../../types/language-ability-type';

@Component({
  selector: 'app-job-points-tab',
  standalone: false,
  templateUrl: './job-points-tab.component.html',
  styleUrls: ['./job-points-tab.component.scss'],
})
export class JobPointsTabComponent implements OnInit {
  @Input() mainForm!: FormGroup;
  @Input() form!: FormGroup;
  @Input() job: JobResponse | null = null;

  @Output() save = new EventEmitter<void>();
  @Output() ready = new EventEmitter<void>();
  @Output() goPrevious = new EventEmitter<void>();

  private fb = inject(FormBuilder);
  private pointsCalculationService = inject(JobPointsCalculationService);

  applicantCategoryFormGroup!: FormGroup;
  educationFormGroup!: FormGroup;
  experienceFormGroup!: FormGroup;
  trainingFormGroup!: FormGroup;
  skillsFormGroup!: FormGroup;
  languagesFormGroup!: FormGroup;
  certificatesFormGroup!: FormGroup;

  applicantCategoryItems = APPLICANT_CATEGORY_ITEMS;
  trainingItems = TRAINING_ITEMS;
  experienceItems = EXPERIENCE_ITEMS;
  educationItems = EDUCATION_ITEMS;
  skillsItems = SKILLS_ITEMS;
  languageItems = LANGUAGE_ITEMS;
  certificatesItems = CERTIFICATES_ITEMS;

  get speakingFormGroup(): FormGroup {
    return this.languagesFormGroup.get('speaking') as FormGroup;
  }

  get readingFormGroup(): FormGroup {
    return this.languagesFormGroup.get('reading') as FormGroup;
  }

  get conversationFormGroup(): FormGroup {
    return this.languagesFormGroup.get('conversation') as FormGroup;
  }

  ngOnInit(): void {
    this.skillsItems =
      this.job?.skills.map((s) => ({
        key: s.skill.backendName,
        label: s.skill.name,
        id: s.skill.id,
      })) || [];

    this.educationItems =
      this.job?.degrees.map((d) => ({
        key: d.degree.backendName,
        label: d.degree.name,
        id: d.degree.id,
      })) || [];

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

    this.experienceItems.forEach((item) => {
      if (!this.experienceFormGroup.get(item.key)) {
        this.experienceFormGroup.addControl(item.key, this.fb.control(0));
      }
    });

    this.addControls(this.trainingFormGroup, this.trainingItems);
    this.addControls(this.skillsFormGroup, this.skillsItems);
    this.addControlsNested(this.languagesFormGroup, this.languageItems); 
    this.addControls(this.certificatesFormGroup, this.certificatesItems);
  }

  private addControls(group: FormGroup, items: DetailItem[]): void {
    items.forEach((item) => {
      if (!group.get(item.key)) {
        group.addControl(item.key, this.fb.control(0));
      }
    });
  }

  private addControlsNested(group: FormGroup, items: DetailItem[]): void {
    items.forEach((item) => {
      const path = item.key.split('.');

      if (path.length === 1) {
        if (!group.get(item.key)) {
          group.addControl(item.key, this.fb.control(0));
        }
      } else if (path.length === 2) {
        const [parentKey, childKey] = path;
        let parentGroup = group.get(parentKey) as FormGroup;

        if (!parentGroup) {
          parentGroup = this.fb.group({});
          group.addControl(parentKey, parentGroup);
        }
        if (!parentGroup.get(childKey)) {
          parentGroup.addControl(childKey, this.fb.control(0));
        }
      }
    });
  }

  get experienceTotal(): number {
    const exp = this.experienceFormGroup?.getRawValue();
    return (exp?.pointsPerYear || 0) * (exp?.maxYears || 0);
  }

  applicantCategorySum() {
    return this.pointsCalculationService.sumCategory(this.applicantCategoryFormGroup);
  }

  educationSum() {
    return this.pointsCalculationService.sumCategory(this.educationFormGroup);
  }

  trainingSum() {
    return this.pointsCalculationService.sumCategory(this.trainingFormGroup);
  }

  skillsSum() {
    return this.pointsCalculationService.sumCategory(this.skillsFormGroup);
  }

  languagesSum(): number {
    return this.pointsCalculationService.sumLanguagesCategory(this.languagesFormGroup);
  }

  certificatesSum() {
    return this.pointsCalculationService.sumCategory(this.certificatesFormGroup);
  }

  areAllCategoriesValid(): boolean {
    return this.pointsCalculationService.areAllCategoriesValid(
      this.mainForm,
      this.form,
      this.pointsCalculationService.sections
    );
  }

  saveDetails(): void {
    if (!this.areAllCategoriesValid()) return;
    this.save.emit();
  }

  maxValue(groupName: string): number {
    return this.mainForm.get(groupName)?.value || 0;
  }

  getAbilitySum(abilityName: LanguageAbilityType): number {
    const abilityGroup = this.languagesFormGroup.get(abilityName) as FormGroup;
    return this.pointsCalculationService.sumAbilityLevels(abilityGroup);
  }

  getAbilityMax(abilityName: LanguageAbilityType): number {
    return this.languagesFormGroup.get(abilityName)?.get('max')?.value || 0;
  }

  isAbilityValid(abilityName: LanguageAbilityType): boolean {
    const abilityGroup = this.languagesFormGroup.get(abilityName) as FormGroup;
    return this.pointsCalculationService.isAbilityValid(abilityGroup);
  }
}
