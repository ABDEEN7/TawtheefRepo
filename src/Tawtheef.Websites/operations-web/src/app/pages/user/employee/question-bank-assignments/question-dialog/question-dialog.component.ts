import { CommonModule } from '@angular/common';
import { Component, inject, OnDestroy } from '@angular/core';
import { FormArray, FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { TranslatePipe } from '@ngx-translate/core';
import { ButtonModule } from 'primeng/button';
import { Checkbox } from 'primeng/checkbox';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { InputTextModule } from 'primeng/inputtext';
import { Select } from 'primeng/select';
import { Subscription } from 'rxjs';
import { RichContentEditorComponent } from '../../../../../shared/rich-content/rich-content-editor.component';
import { RichContentInputComponent } from '../../../../../shared/rich-content/rich-content-input.component';
import { hasMeaningfulRichContent } from '../../../../../shared/rich-content/rich-content.utils';
import {
  AssignmentOption,
  AssignmentQuestion,
  QuestionTypes,
} from '../models/question-bank-assignment.models';
import { QuestionBankAssignmentsService } from '../services/question-bank-assignments.service';

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
    RichContentEditorComponent,
    RichContentInputComponent,
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

  get options(): FormArray {
    return this.form.controls.options;
  }

  constructor() {
    const question = this.config.data?.question as AssignmentQuestion | undefined;
    if (question) {
      this.form.patchValue(question);
      question.options.forEach((option) => this.addOption(option));
      if (question.imageUrl) this.loadExistingImage(question.imageUrl);
      return;
    }

    this.addOption();
    this.addOption();
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
    return !!control && (control.touched || this.submitted) && !hasMeaningfulRichContent(control.value);
  }

  correctOptionInvalid(): boolean {
    return this.submitted && this.options.controls.filter((option) => option.value.isCorrect).length !== 1;
  }

  cancel(): void {
    this.ref.close();
  }

  ngOnDestroy(): void {
    this.imageSubscription?.unsubscribe();
    this.revokeImagePreview();
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
