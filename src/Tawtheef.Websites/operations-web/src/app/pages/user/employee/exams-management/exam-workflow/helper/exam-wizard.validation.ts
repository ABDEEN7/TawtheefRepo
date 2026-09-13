import { ExamBankDto } from '../../models/exam-bank.dto';
import { ExamConfigurationDto } from '../../models/exam-configuration.dto';
import { ExamPartDto } from '../../models/exam-part.dto';

const count = (value: number) => Number.isInteger(value) && value >= 0 && value <= 2147483647;
const percent = (value: number) => Number.isFinite(value) && value >= 0 && value <= 100;

export function validatePart(part: ExamPartDto, banks: ExamBankDto[], final: boolean): string[] {
  const errors = new Set<string>();
  if (
    !count(part.partNo) ||
    part.partNo === 0 ||
    !count(part.durationMinutes) ||
    (part.qualificationScore !== null && !percent(part.qualificationScore))
  ) {
    errors.add('NUMBERS');
  }
  if ((part.titleAr?.length ?? 0) > 200 || (part.titleEn?.length ?? 0) > 200) errors.add('LENGTH');
  if (final && (!part.titleAr?.trim() || part.durationMinutes <= 0 || !part.categories.length)) {
    errors.add('PART_COMPLETE');
  }
  if (new Set(part.categories.map((c) => c.questionBankTypeId)).size !== part.categories.length) {
    errors.add('DUPLICATE_CATEGORY');
  }
  for (const category of part.categories) {
    const bank = banks.find(
      (b) => b.id === category.questionBankVersionId && b.questionBankTypeId === category.questionBankTypeId,
    );
    if (!bank) errors.add('BANK');
    if (
      ![
        category.questionCount,
        category.easyQuestionCount,
        category.mediumQuestionCount,
        category.hardQuestionCount,
      ].every(count) ||
      !percent(category.weightPercent)
    )
      errors.add('NUMBERS');
    if (final) {
      if (
        category.questionCount <= 0 ||
        category.easyQuestionCount + category.mediumQuestionCount + category.hardQuestionCount !==
          category.questionCount
      )
        errors.add('DISTRIBUTION');
      if (
        bank &&
        (category.easyQuestionCount > bank.easy ||
          category.mediumQuestionCount > bank.medium ||
          category.hardQuestionCount > bank.hard)
      )
        errors.add('AVAILABILITY');
    }
  }
  return [...errors];
}

export function validateExam(
  exam: ExamConfigurationDto,
  banks: ExamBankDto[],
  final: boolean,
  specializedCategoryId: string,
  categoryCount: number,
): string[] {
  const errors = new Set<string>();
  if (!exam.jobId || !exam.interruptionPolicyId || (final && !exam.titleAr.trim()))
    errors.add('INFO');
  if (
    exam.titleAr.length > 200 ||
    (exam.titleEn?.length ?? 0) > 200 ||
    (exam.notes?.length ?? 0) > 2000
  ) {
    errors.add('LENGTH');
  }
  if (
    exam.parts.length !== 2 ||
    exam.parts[0]?.partNo !== 1 ||
    exam.parts[1]?.partNo !== 2 ||
    exam.parts[0]?.categories.length !== 1 ||
    exam.parts[0]?.categories[0]?.questionBankTypeId !== specializedCategoryId ||
    exam.parts[1]?.categories.some((c) => c.questionBankTypeId === specializedCategoryId) ||
    new Set(exam.parts[1]?.categories.map((c) => c.questionBankTypeId)).size !==
      exam.parts[1]?.categories.length ||
    exam.parts[1]?.categories.length > Math.max(0, categoryCount - 1)
  )
    errors.add('STRUCTURE');
  exam.parts.forEach((p) => validatePart(p, banks, final).forEach((e) => errors.add(e)));
  const categories = exam.parts.flatMap((p) => p.categories);
  if (
    final &&
    categories.reduce((sum, c) => sum + Math.round(c.weightPercent * 100), 0) !== 10000
  ) {
    errors.add('WEIGHT');
  }
  if (
    !count(exam.totalQuestions) ||
    exam.totalQuestions !== categories.reduce((sum, c) => sum + c.questionCount, 0)
  ) {
    errors.add('TOTAL');
  }
  return [...errors];
}
