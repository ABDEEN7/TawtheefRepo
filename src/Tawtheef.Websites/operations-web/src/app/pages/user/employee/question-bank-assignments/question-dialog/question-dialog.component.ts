import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormArray, FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { TranslatePipe } from '@ngx-translate/core';
import { ButtonModule } from 'primeng/button';
import { Checkbox } from 'primeng/checkbox';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { InputTextModule } from 'primeng/inputtext';
import { Select } from 'primeng/select';
import {
  AssignmentQuestion,
  AssignmentOption,
  QuestionTypes,
} from '../models/question-bank-assignment.models';

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
export class QuestionDialogComponent {
  private readonly fb = inject(FormBuilder);
  private readonly ref = inject(DynamicDialogRef);
  readonly config = inject(DynamicDialogConfig);

  readonly types = [
    {
      label: 'QUESTION_ASSIGNMENTS.MULTIPLE_CHOICE',
      value: QuestionTypes.multipleChoice,
    },
    {
      label: 'QUESTION_ASSIGNMENTS.TRUE_FALSE',
      value: QuestionTypes.trueFalse,
    },
  ];

  readonly difficulties = [
    {
      label: 'QUESTION_ASSIGNMENTS.EASY',
      value: '59e7808a-916b-44c4-92a9-41dd1c6e24e4',
    },
    {
      label: 'QUESTION_ASSIGNMENTS.MEDIUM',
      value: '580deb45-9304-4fcb-905e-868f5db3848b',
    },
    {
      label: 'QUESTION_ASSIGNMENTS.HARD',
      value: '9bbee09c-e2a3-463b-9901-4b646200074d',
    },
  ];

  readonly form = this.fb.group({
    questionTypeId: ['', Validators.required],
    difficultyLevelId: ['', Validators.required],
    questionTextAr: [''],
    questionTextEn: [''],
    explanationAr: [''],
    explanationEn: [''],
    options: this.fb.array<AssignmentOption>([]),
  });

  get options(): FormArray {
    return this.form.controls.options;
  }

  constructor() {
    const question = this.config.data?.question as AssignmentQuestion | undefined;

    if (question) {
      this.form.patchValue(question);
      question.options.forEach((option) => this.addOption(option));
      return;
    }

    this.addOption();
    this.addOption();
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
    if (this.options.length > 2) {
      this.options.removeAt(index);
    }
  }

  setCorrect(selectedIndex: number): void {
    this.options.controls.forEach((control, index) => {
      control.patchValue({ isCorrect: index === selectedIndex });
    });
  }

  save(): void {
    const value = this.form.getRawValue();
    const hasQuestion = !!(value.questionTextAr?.trim() || value.questionTextEn?.trim());
    const hasValidOptions =
      value.options.every(
        (option) => !!(option?.optionTextAr?.trim() || option?.optionTextEn?.trim()),
      ) && value.options.filter((option) => option?.isCorrect).length === 1;

    if (this.form.valid && hasQuestion && hasValidOptions) {
      this.ref.close(value);
      return;
    }

    this.form.markAllAsTouched();
  }

  cancel(): void {
    this.ref.close();
  }
}
