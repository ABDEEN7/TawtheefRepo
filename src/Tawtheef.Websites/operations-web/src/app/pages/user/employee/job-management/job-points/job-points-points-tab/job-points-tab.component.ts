import { Component, Input, OnInit, EventEmitter, Output, inject } from '@angular/core';
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
import { JobLookupService } from '../../services/job-lookup.service';
import { JobPointsDetailType } from '../../types/job-points-detail.type';
import { SaveJobPointsRequestDto } from '../../models/save-job-points-request-dto';
import { SaveJobPointsDetailDto } from '../../models/save-job-points-detail-dto';
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

  @Output() save = new EventEmitter<SaveJobPointsRequestDto>();

  @Output() ready = new EventEmitter<void>();
  @Output() goPrevious = new EventEmitter<void>();

  private fb = inject(FormBuilder);
  private pointsCalculationService = inject(JobPointsCalculationService);
  private lookupService = inject(JobLookupService);

  private formGroupMappings = {
    applicantCategory: {
      items: APPLICANT_CATEGORY_ITEMS,
      getter: () => this.form.get('applicantCategory') as FormGroup,
      sum: () => this.pointsCalculationService.sumCategory(this.applicantCategoryFormGroup),
      dynamicItems: () =>
        this.lookupService?.candidateTypes().map((s) => ({
          key: s.backendName,
          label: s.name,
          id: s.id,
        })) || [],
    },
    education: {
      items: EDUCATION_ITEMS,
      getter: () => this.form.get('education') as FormGroup,
      sum: () => this.pointsCalculationService.sumCategory(this.educationFormGroup),
      dynamicItems: () =>
        this.job?.degrees.map((d) => ({
          key: d.degree.backendName,
          label: d.degree.name,
          id: d.degree.id,
        })) || [],
    },
    experience: {
      items: EXPERIENCE_ITEMS,
      getter: () => this.form.get('experience') as FormGroup,
      total: () => {
        const exp = this.experienceFormGroup?.getRawValue();
        return (exp?.pointsPerYear || 0) * (exp?.maxYears || 0);
      },
    },
    training: {
      items: TRAINING_ITEMS,
      getter: () => this.form.get('training') as FormGroup,
      sum: () => this.pointsCalculationService.sumCategory(this.trainingFormGroup),
    },
    skills: {
      items: SKILLS_ITEMS,
      getter: () => this.form.get('skills') as FormGroup,
      sum: () => this.pointsCalculationService.sumCategory(this.skillsFormGroup),
      dynamicItems: () =>
        this.job?.skills.map((s) => ({
          key: s.skill.backendName,
          label: s.skill.name,
          id: s.skill.id,
        })) || [],
    },
    languages: {
      items: LANGUAGE_ITEMS,
      getter: () => this.form.get('languages') as FormGroup,
      sum: () => this.pointsCalculationService.sumLanguagesCategory(this.languagesFormGroup),
    },
    certificates: {
      items: CERTIFICATES_ITEMS,
      getter: () => this.form.get('certificates') as FormGroup,
      sum: () => this.pointsCalculationService.sumCategory(this.certificatesFormGroup),
    },
  };

  get applicantCategoryFormGroup(): FormGroup {
    return this.formGroupMappings.applicantCategory.getter();
  }
  get educationFormGroup(): FormGroup {
    return this.formGroupMappings.education.getter();
  }
  get experienceFormGroup(): FormGroup {
    return this.formGroupMappings.experience.getter();
  }
  get trainingFormGroup(): FormGroup {
    return this.formGroupMappings.training.getter();
  }
  get skillsFormGroup(): FormGroup {
    return this.formGroupMappings.skills.getter();
  }
  get languagesFormGroup(): FormGroup {
    return this.formGroupMappings.languages.getter();
  }
  get certificatesFormGroup(): FormGroup {
    return this.formGroupMappings.certificates.getter();
  }

  trainingItems = TRAINING_ITEMS;
  experienceItems = EXPERIENCE_ITEMS;
  certificatesItems = CERTIFICATES_ITEMS;

  educationItems: DetailItem[] = [];
  skillsItems: DetailItem[] = [];
  applicantCategoryItems: DetailItem[] = [];
  languageItems = LANGUAGE_ITEMS;

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
    this.lookupService.loadCandidateTypes().subscribe((resp) => {
      this.applicantCategoryItems =
        (resp ? this.formGroupMappings.applicantCategory.dynamicItems?.() : null) ||
        APPLICANT_CATEGORY_ITEMS;
      this.educationItems = this.formGroupMappings.education.dynamicItems?.() || EDUCATION_ITEMS;
      this.skillsItems = this.formGroupMappings.skills.dynamicItems?.() || SKILLS_ITEMS;

      this.initFormControls();
      this.ready.emit();
    });
  }

  private initFormControls(): void {
    Object.keys(this.formGroupMappings).forEach((key) => {
      const mapping = this.formGroupMappings[key as keyof typeof this.formGroupMappings];
      const group = mapping.getter();
      const items =
        key === 'education'
          ? this.educationItems
          : key === 'skills'
          ? this.skillsItems
          : key === 'applicantCategory'
          ? this.applicantCategoryItems
          : mapping.items;

      if (key === 'languages') {
        this.addControlsNested(group, items);
      } else if (key === 'experience') {
        this.addExperienceControls(group, items);
      } else {
        this.addControls(group, items);
      }
    });
  }

  private addControls(group: FormGroup, items: DetailItem[]): void {
    items.forEach((item) => {
      if (!group.get(item.key)) {
        group.addControl(item.key, this.fb.control(0));
      }
    });
  }

  private addExperienceControls(group: FormGroup, items: DetailItem[]): void {
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
        this.ensureControlExists(group, item.key);
      } else if (path.length === 2) {
        const [parentKey, childKey] = path;
        const parentGroup = this.getOrCreateNestedGroup(group, parentKey);
        this.ensureControlExists(parentGroup, childKey);
      }
    });
  }

  private getOrCreateNestedGroup(parentGroup: FormGroup, groupName: string): FormGroup {
    let nestedGroup = parentGroup.get(groupName) as FormGroup;
    if (!nestedGroup) {
      nestedGroup = this.fb.group({});
      parentGroup.addControl(groupName, nestedGroup);
    }
    return nestedGroup;
  }

  private ensureControlExists(group: FormGroup, controlName: string): void {
    if (!group.get(controlName)) {
      group.addControl(controlName, this.fb.control(0));
    }
  }

  applicantCategorySum(): number {
    return this.formGroupMappings.applicantCategory.sum!();
  }
  educationSum(): number {
    return this.formGroupMappings.education.sum!();
  }
  trainingSum(): number {
    return this.formGroupMappings.training.sum!();
  }
  skillsSum(): number {
    return this.formGroupMappings.skills.sum!();
  }
  languagesSum(): number {
    return this.formGroupMappings.languages.sum!();
  }
  certificatesSum(): number {
    return this.formGroupMappings.certificates.sum!();
  }
  get experienceTotal(): number {
    return this.formGroupMappings.experience.total!();
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
    const payload = this.buildSavePayload();
    this.save.emit(payload);
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

  private buildSavePayload(): SaveJobPointsRequestDto {
    const jobId = this.job?.id;
    if (!jobId) {
      throw new Error('Job is not loaded yet (missing jobId).');
    }

    const applicantCategory = this.applicantCategorySum();
    const education = this.educationSum();
    const experience = this.experienceTotal;
    const training = this.trainingSum();
    const skills = this.skillsSum();
    const languages = this.languagesSum();
    const certificates = this.certificatesSum();
    const total =
      applicantCategory + education + experience + training + skills + languages + certificates;

    const details: SaveJobPointsDetailDto[] = [
      ...this.buildCategoryDetails('ApplicantCategory', this.applicantCategoryFormGroup, this.applicantCategoryItems),
      ...this.buildCategoryDetails('Education', this.educationFormGroup, this.educationItems),
      ...this.buildCategoryDetails('Experience', this.experienceFormGroup, this.experienceItems),
      ...this.buildCategoryDetails('Training', this.trainingFormGroup, this.trainingItems),
      ...this.buildCategoryDetails('Skills', this.skillsFormGroup, this.skillsItems),
      ...this.buildCategoryDetails('Languages', this.languagesFormGroup, this.languageItems),
      ...this.buildCategoryDetails('Certificates', this.certificatesFormGroup, this.certificatesItems),
    ];

    return {
      jobId,
      applicantCategory,
      education,
      experience,
      training,
      skills,
      languages,
      certificates,
      total,
      details,
    };
  }

  private buildCategoryDetails(
    type: JobPointsDetailType,
    group: FormGroup,
    items: DetailItem[]
  ): SaveJobPointsDetailDto[] {
    const raw = group.getRawValue(); 
    return items.map((item) => {
      const points = this.readValueByKeyPath(raw, item.key);

      return {
        type,
        code: item.key,
        name: item.label,
        referenceId: (item as any).id ?? null, 
        points: Number(points ?? 0),
      };
    });
  }

  private readValueByKeyPath(raw: any, key: string): any {
    if (!key.includes('.')) return raw?.[key];
    return key.split('.').reduce((acc, part) => acc?.[part], raw);
  }
}
