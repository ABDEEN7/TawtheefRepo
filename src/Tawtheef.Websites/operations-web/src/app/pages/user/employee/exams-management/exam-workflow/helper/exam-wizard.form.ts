import { FormArray, FormControl, FormGroup } from '@angular/forms';
import { ExamPartDto } from '../../models/exam-part.dto';
import { ExamCategoryDto } from '../../models/exam-category.dto';

const control = <T>(value: T) => new FormControl(value, { nonNullable: true });

export function examForm() {
  return new FormGroup({
    jobId: control(''),
    titleAr: control(''),
    titleEn: control<string | null>(null),
    allowPreviousQuestion: control(true),
    interruptionPolicyId: control(''),
    notes: control<string | null>(null),
    parts: new FormArray<ReturnType<typeof partForm>>([
      partForm(1),
      partForm(2),
    ]),
  });
}

export function partForm(partNo: 1 | 2, value?: ExamPartDto) {
  return new FormGroup({
    partNo: control(value?.partNo ?? partNo),
    titleAr: control(value?.titleAr ?? ''),
    titleEn: control<string | null>(value?.titleEn ?? null),
    durationMinutes: control(value?.durationMinutes ?? 0),
    qualificationScore: control<number | null>(value?.qualificationScore ?? null),
    categories: new FormArray((value?.categories ?? []).map(categoryForm)),
  });
}

export function categoryForm(value?: ExamCategoryDto) {
  return new FormGroup({
    categoryId: control(value?.categoryId ?? ''),
    questionBankVersionId: control(value?.questionBankVersionId ?? ''),
    questionCount: control(value?.questionCount ?? 0),
    weightPercent: control(value?.weightPercent ?? 0),
    easyQuestionCount: control(value?.easyQuestionCount ?? 0),
    mediumQuestionCount: control(value?.mediumQuestionCount ?? 0),
    hardQuestionCount: control(value?.hardQuestionCount ?? 0),
  });
}

export type ExamCategoryForm = ReturnType<typeof categoryForm>;
