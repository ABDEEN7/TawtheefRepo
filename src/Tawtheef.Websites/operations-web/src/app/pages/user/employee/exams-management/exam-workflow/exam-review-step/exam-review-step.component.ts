import { Component, EventEmitter, inject, Input, Output } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { LanguageService } from '../../../../../../core/services/language.service';
import { ExamLookupItemDto } from '../../models/exam-lookups.dto';
import { ExamPartDto } from '../../models/exam-part.dto';

@Component({
  selector: 'app-exam-review-step',
  standalone: true,
  templateUrl: './exam-review-step.component.html',
  styleUrl: './exam-review-step.component.scss',
  imports: [TranslatePipe, ButtonModule, TableModule],
})
export class ExamReviewStepComponent {
  @Input() parts: ExamPartDto[] = [];
  @Input() categories: ExamLookupItemDto[] = [];
  @Input() isViewMode = false;
  @Input() hasReviewErrors = false;
  @Output() editRequested = new EventEmitter<number>();

  private readonly language = inject(LanguageService);

  get totalQuestions(): number {
    return this.parts
      .flatMap((part) => part.categories)
      .reduce((sum, category) => sum + (category.questionCount || 0), 0);
  }

  get totalWeight(): number {
    return (
      this.parts
        .flatMap((part) => part.categories)
        .reduce((sum, category) => sum + Math.round((category.weightPercent || 0) * 100), 0) / 100
    );
  }

  lookupName(id: string): string {
    const category = this.categories.find((item) => item.id === id);
    return category ? (this.language.isRtl ? category.nameAr : category.nameEn) : '';
  }
}
