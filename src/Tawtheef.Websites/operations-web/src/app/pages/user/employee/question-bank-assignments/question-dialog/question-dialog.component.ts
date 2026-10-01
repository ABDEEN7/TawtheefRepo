import {CommonModule} from '@angular/common';
import {Component, inject, OnDestroy} from '@angular/core';
import {FormArray, FormBuilder, ReactiveFormsModule, Validators} from '@angular/forms';
import {TranslatePipe} from '@ngx-translate/core';
import {ButtonModule} from 'primeng/button';
import {Checkbox} from 'primeng/checkbox';
import {DynamicDialogConfig, DynamicDialogRef} from 'primeng/dynamicdialog';
import {InputTextModule} from 'primeng/inputtext';
import {Select} from 'primeng/select';
import {startWith, Subscription} from 'rxjs';
import {hasMeaningfulRichContent} from '../../../../../shared/rich-content/rich-content.utils';
import {AssignmentOption, AssignmentQuestion, QuestionTypes,} from '../models/question-bank-assignment.models';
import {QuestionBankAssignmentsService} from '../services/question-bank-assignments.service';

@Component({
  selector: 'app-question-dialog',
  standalone: true,
  templateUrl: './question-dialog.component.html',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    TranslatePipe,
    ButtonModule,
    InputTextModule,
    Select,
    Checkbox,

  ],
})
export class QuestionDialogComponent implements OnDestroy {
  private readonly fb = inject(FormBuilder);
  private readonly ref = inject(DynamicDialogRef);
  private readonly service = inject(QuestionBankAssignmentsService);
  readonly config = inject(DynamicDialogConfig);

  readonly types = [
    { label: 'QUESTION_ASSIGNMENTS.MULTIPLE_CHOICE', value: QuestionTypes.multipleChoice },
    { label: 'QUESTION_ASSIGNMENTS.TRUE_FALSE', value: QuestionTypes.trueFalse },
  ];

  readonly difficulties = [
    { label: 'QUESTION_ASSIGNMENTS.EASY', value: '59e7808a-916b-44c4-92a9-41dd1c6e24e4' },
    { label: 'QUESTION_ASSIGNMENTS.MEDIUM', value: '580deb45-9304-4fcb-905e-868f5db3848b' },
    { label: 'QUESTION_ASSIGNMENTS.HARD', value: '9bbee09c-e2a3-463b-9901-4b646200074d' },
  ];

  readonly form = this.fb.group({
    questionTypeId: this.fb.control<string | null>(null, Validators.required),
    difficultyLevelId: this.fb.control<string | null>(null, Validators.required),
    questionTextAr: [''],
    questionTextEn: [''],
    explanationAr: [''],
    explanationEn: [''],
    resourceId: this.fb.control<string | null>(null),
    options: this.fb.array<AssignmentOption>([]),
  });

  imageFile?: File;
  imagePreview?: string;
  imageError = false;
  imageLoading = false;
  submitted = false;
  private imageSubscription?: Subscription;
  private readonly typeSubscription: Subscription;

  get options(): FormArray {
    return this.form.controls.options;
  }

  constructor() {
    const question = this.config.data?.question as AssignmentQuestion | undefined;
    if (question) {
      this.form.patchValue(question);
      if (question.questionTypeId === QuestionTypes.trueFalse) {
        this.initializeTrueFalseOptions(question.options);
      } else {
        this.initializeMultipleChoiceOptions(question.options);
      }
      if (question.imageUrl) this.loadExistingImage(question.imageUrl);
    } else {
      this.initializeMultipleChoiceOptions();
    }

    let previousType = this.form.controls.questionTypeId.value;
    this.typeSubscription = this.form.controls.questionTypeId.valueChanges
      .pipe(startWith(previousType))
      .subscribe((type) => {
        if (type === previousType) return;
        previousType = type;
        if (type === QuestionTypes.trueFalse) this.initializeTrueFalseOptions();
        if (type === QuestionTypes.multipleChoice) this.initializeMultipleChoiceOptions();
      });
  }

  get isTrueFalse(): boolean {
    return this.form.controls.questionTypeId.value === QuestionTypes.trueFalse;
  }

  chooseImage(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    if (!file) return;
    const allowed = ['image/jpeg', 'image/png', 'image/webp'];
    this.imageError = !allowed.includes(file.type) || file.size === 0 || file.size > 1_000_000;
    if (this.imageError) {
      input.value = '';
      return;
    }
    this.imageSubscription?.unsubscribe();
    this.imageLoading = false;
    this.revokeImagePreview();
    this.imageFile = file;
    this.imagePreview = URL.createObjectURL(file);
  }

  removeImage(): void {
    this.imageSubscription?.unsubscribe();
    this.imageLoading = false;
    this.revokeImagePreview();
    this.imagePreview = undefined;
    this.imageFile = undefined;
    this.form.controls.resourceId.setValue(null);
  }

  addOption(value: Partial<AssignmentQuestion['options'][number]> = {}): void {
    if (this.isTrueFalse) return;
    this.options.push(
      this.fb.group({
        optionTextAr: [value.optionTextAr ?? ''],
        optionTextEn: [value.optionTextEn ?? ''],
        isCorrect: [value.isCorrect ?? false],
        displayOrder: [value.displayOrder ?? this.options.length + 1],
      }),
    );
  }

  removeOption(index: number): void {
    if (this.isTrueFalse) return;
    if (this.options.length > 2) this.options.removeAt(index);
  }

  setCorrect(selectedIndex: number): void {
    this.options.controls.forEach((control, index) => {
      control.patchValue({ isCorrect: index === selectedIndex });
    });
  }

  save(): void {
    this.submitted = true;
    this.form.updateValueAndValidity();
    this.form.markAllAsTouched();

    const value = this.form.getRawValue();
    const hasAr = hasMeaningfulRichContent(value.questionTextAr);
    const hasEn = hasMeaningfulRichContent(value.questionTextEn);
    const hasQuestion =
      (hasAr && hasEn) || (!hasAr && !hasEn && !!(value.resourceId || this.imageFile));
    const hasValidOptions =
      value.options.length >= 2 &&
      value.options.every(
        (option) =>
          hasMeaningfulRichContent(option?.optionTextAr) &&
          hasMeaningfulRichContent(option?.optionTextEn),
      ) &&
      value.options.filter((option) => option?.isCorrect).length === 1;

    if (this.form.invalid || !hasQuestion || !hasValidOptions) return;
    this.ref.close({ ...value, imageFile: this.imageFile });
  }

  controlInvalid(controlName: 'questionTypeId' | 'difficultyLevelId'): boolean {
    const control = this.form.controls[controlName];
    return control.invalid && (control.touched || this.submitted);
  }

  questionContentInvalid(): boolean {
    if (!this.submitted) return false;
    const value = this.form.getRawValue();
    const hasAr = hasMeaningfulRichContent(value.questionTextAr);
    const hasEn = hasMeaningfulRichContent(value.questionTextEn);
    return !((hasAr && hasEn) || (!hasAr && !hasEn && !!(value.resourceId || this.imageFile)));
  }

  optionInvalid(index: number, language: 'optionTextAr' | 'optionTextEn'): boolean {
    const control = this.options.at(index).get(language);
    return (
      !!control && (control.touched || this.submitted) && !hasMeaningfulRichContent(control.value)
    );
  }

  correctOptionInvalid(): boolean {
    return (
      this.submitted &&
      this.options.controls.filter((option) => option.value.isCorrect).length !== 1
    );
  }

  cancel(): void {
    this.ref.close();
  }

  ngOnDestroy(): void {
    this.typeSubscription.unsubscribe();
    this.imageSubscription?.unsubscribe();
    this.revokeImagePreview();
  }

  private initializeMultipleChoiceOptions(values: AssignmentQuestion['options'] = []): void {
    this.options.clear();
    (values.length >= 2 ? values : [{}, {}]).forEach((option) => this.addOption(option));
  }

  private initializeTrueFalseOptions(values: AssignmentQuestion['options'] = []): void {
    const normalized = (value?: string | null) =>
      value
        ?.replace(/<[^>]*>/g, '')
        .trim()
        .toLowerCase();
    const trueOption = values.find(
      (option) =>
        normalized(option.optionTextAr) === 'صح' || normalized(option.optionTextEn) === 'true',
    );
    const falseOption = values.find(
      (option) =>
        normalized(option.optionTextAr) === 'خطأ' || normalized(option.optionTextEn) === 'false',
    );
    const safelyUsePositions =
      values.length === 2 &&
      !trueOption &&
      !falseOption &&
      values[0].displayOrder === 1 &&
      values[1].displayOrder === 2;
    this.options.clear();
    this.options.push(
      this.fb.group({
        optionTextAr: ['صح'],
        optionTextEn: ['True'],
        isCorrect: [trueOption?.isCorrect ?? (safelyUsePositions ? values[0].isCorrect : false)],
        displayOrder: [1],
      })
    );
    this.options.push(
       this.fb.group({
        optionTextAr: ['خطأ'],
        optionTextEn: ['False'],
        isCorrect: [falseOption?.isCorrect ?? (safelyUsePositions ? values[1].isCorrect : false)],
        displayOrder: [2],
      })
    );
  }

  private loadExistingImage(blobKey: string): void {
    this.imageLoading = true;
    this.imageSubscription = this.service.image(blobKey).subscribe({
      next: (blob) => {
        this.revokeImagePreview();
        this.imagePreview = URL.createObjectURL(blob);
        this.imageLoading = false;
      },
      error: () => {
        this.imageError = true;
        this.imageLoading = false;
      },
    });
  }

  private revokeImagePreview(): void {
    if (this.imagePreview?.startsWith('blob:')) URL.revokeObjectURL(this.imagePreview);
  }
}
