import { Component, Input } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { TranslatePipe } from '@ngx-translate/core';
import { InputNumberModule } from 'primeng/inputnumber';
import { InputTextModule } from 'primeng/inputtext';
import { ExamBankDto } from '../../models/exam-bank.dto';
import { ExamLookupItemDto } from '../../models/exam-lookups.dto';
import { ExamCategoryComponent } from '../exam-category/exam-category.component';
import { partForm } from '../helper/exam-wizard.form';

@Component({
  selector: 'app-exam-part-one-step',
  standalone: true,
  templateUrl: './exam-part-one-step.component.html',
  styleUrl: './exam-part-one-step.component.scss',
  imports: [
    ReactiveFormsModule,
    TranslatePipe,
    InputNumberModule,
    InputTextModule,
    ExamCategoryComponent,
  ],
})
export class ExamPartOneStepComponent {
  @Input({ required: true }) form!: ReturnType<typeof partForm>;
  @Input() banks: ExamBankDto[] = [];
  @Input() categories: ExamLookupItemDto[] = [];
  @Input() specializedCategoryId = '';
  @Input() isViewMode = false;

  get specializedCategories(): ExamLookupItemDto[] {
    const specialized = this.categories.find((category) => category.id === this.specializedCategoryId);
    return specialized ? [specialized] : [];
  }
}
