import { Component, inject, Input } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { InputNumberModule } from 'primeng/inputnumber';
import { SelectModule } from 'primeng/select';
import { TableModule } from 'primeng/table';
import { LanguageService } from '../../../../../../core/services/language.service';
import { ExamCategoryForm } from '../helper/exam-wizard.form';
import { ExamBankDto } from '../../models/exam-bank.dto';
import { ExamLookupItemDto } from '../../models/exam-lookups.dto';

type DifficultyRow = {
  key: 'EASY' | 'MEDIUM' | 'HARD';
  control: 'easyQuestionCount' | 'mediumQuestionCount' | 'hardQuestionCount';
  available: 'easy' | 'medium' | 'hard';
};

type TotalRow = { key: 'TOTAL' };

@Component({
  selector: 'app-exam-category',
  standalone: true,
  templateUrl: './exam-category.component.html',
  styleUrl: './exam-category.component.scss',
  imports: [ReactiveFormsModule, TranslatePipe, InputNumberModule, SelectModule, TableModule],
})
export class ExamCategoryComponent {
  @Input({ required: true }) form!: ExamCategoryForm;
  @Input({ required: true }) prefix!: string;
  @Input() banks: ExamBankDto[] = [];
  @Input() categories: ExamLookupItemDto[] = [];
  @Input() usedCategories: string[] = [];
  @Input() hideCategory = false;
  @Input() isViewMode = false;

  readonly language = inject(LanguageService);
  private readonly translate = inject(TranslateService);
  readonly difficulties: readonly DifficultyRow[] = [
    { key: 'EASY', control: 'easyQuestionCount', available: 'easy' },
    { key: 'MEDIUM', control: 'mediumQuestionCount', available: 'medium' },
    { key: 'HARD', control: 'hardQuestionCount', available: 'hard' },
  ];
  readonly distributionRows: (DifficultyRow | TotalRow)[] = [
    ...this.difficulties,
    { key: 'TOTAL' },
  ];

  get bank() {
    return this.banks.find((b) => b.id === this.form.controls.questionBankVersionId.value);
  }

  get total() {
    const c = this.form.getRawValue();
    return (c.easyQuestionCount || 0) + (c.mediumQuestionCount || 0) + (c.hardQuestionCount || 0);
  }

  get hasDistributionAvailabilityError() {
    return this.difficulties.some((difficulty) => this.hasDifficultyAvailabilityError(difficulty));
  }

  get isDistributionAvailable() {
    return !!this.bank && !this.hasDistributionAvailabilityError;
  }

  hasDifficultyAvailabilityError(difficulty: DifficultyRow) {
    const bank = this.bank;
    return !!bank && this.form.controls[difficulty.control].value > bank[difficulty.available];
  }

  categoryOptions() {
    return this.categories.filter(
      (c) => c.id === this.form.controls.categoryId.value || !this.usedCategories.includes(c.id),
    );
  }

  bankOptions() {
    return this.banks
      .filter((b) => b.categoryId === this.form.controls.categoryId.value)
      .map((b) => ({
        ...b,
        label: this.translate.instant('EXAM_WIZARD.BANK_LABEL', {
          name: this.language.isRtl ? b.nameAr : b.nameEn,
          version: b.versionNo,
          id: b.id,
        }),
      }));
  }
}
