import { Component, Input, OnInit } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { TranslatePipe } from '@ngx-translate/core';
import { ButtonModule } from 'primeng/button';
import { InputNumberModule } from 'primeng/inputnumber';
import { InputTextModule } from 'primeng/inputtext';
import { ExamBankDto } from '../../models/exam-bank.dto';
import { ExamLookupItemDto } from '../../models/exam-lookups.dto';
import { ExamCategoryComponent } from '../exam-category/exam-category.component';
import { categoryForm, partForm } from '../helper/exam-wizard.form';

@Component({
  selector: 'app-exam-part-two-step',
  standalone: true,
  templateUrl: './exam-part-two-step.component.html',
  styleUrl: './exam-part-two-step.component.scss',
  imports: [
    ReactiveFormsModule,
    TranslatePipe,
    ButtonModule,
    InputNumberModule,
    InputTextModule,
    ExamCategoryComponent,
  ],
})
export class ExamPartTwoStepComponent implements OnInit {
  @Input({ required: true }) form!: ReturnType<typeof partForm>;
  @Input() banks: ExamBankDto[] = [];
  @Input() categories: ExamLookupItemDto[] = [];
  @Input() specializedCategoryId = '';
  @Input() isCreateMode = false;
  @Input() isViewMode = false;
  @Input() active = false;

  private categoriesInitialized = false;

  get nonSpecializedCategories(): ExamLookupItemDto[] {
    return this.categories.filter((category) => category.id !== this.specializedCategoryId);
  }

  ngOnInit(): void {
    this.initializeCategories();
  }

  categoryIds(): string[] {
    return this.form.controls.categories.getRawValue().map((category) => category.questionBankTypeId);
  }

  addCategory(categoryId = '', markDirty = true): void {
    if (this.form.controls.categories.length >= this.nonSpecializedCategories.length) return;
    const category = categoryForm();
    category.controls.questionBankTypeId.setValue(categoryId);
    this.form.controls.categories.push(category);
    if (markDirty) this.form.markAsDirty();
  }

  removeCategory(categoryIndex: number): void {
    this.form.controls.categories.removeAt(categoryIndex);
    this.form.markAsDirty();
  }

  private initializeCategories(): void {
    if (!this.isCreateMode || this.categoriesInitialized) return;
    this.nonSpecializedCategories.forEach((category) => this.addCategory(category.id, false));
    this.categoriesInitialized = true;
  }
}
