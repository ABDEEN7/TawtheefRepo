import {CommonModule} from '@angular/common';
import {Component, computed, inject, OnInit, signal} from '@angular/core';
import {FormBuilder, ReactiveFormsModule, Validators} from '@angular/forms';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import {DynamicDialogConfig, DynamicDialogRef} from 'primeng/dynamicdialog';
import {I18nNamespaceDirective} from '../../../../../shared/directives/i18n-namespace.directive';
import {Lang, LanguageService} from '../../../../../core/services/language.service';
import {FAQ, FaqPayload} from '../models/faq.model';

interface FaqDialogData {
  mode: 'create' | 'edit';
  faq?: FAQ;
}

@Component({
  selector: 'app-home-faq-dialog',
  standalone: true,
  templateUrl: './home-faq-dialog.component.html',
  styleUrls: ['./home-faq-dialog.component.scss'],
  imports: [CommonModule, ReactiveFormsModule, TranslatePipe, I18nNamespaceDirective]
})
export class HomeFaqDialogComponent implements OnInit {
  private fb = inject(FormBuilder);
  private dialogRef = inject(DynamicDialogRef);
  private config = inject(DynamicDialogConfig<FaqDialogData>);
  private translate = inject(TranslateService);
  private language = inject(LanguageService);

  private faq = this.config.data?.faq;
  currentLang = signal<Lang>(this.language.get());
  isRtl = computed(() => this.currentLang() === 'ar');
  submitted = false;

  form = this.fb.nonNullable.group({
    questionAr: ['', Validators.required],
    questionEn: ['', Validators.required],
    answerAr: ['', Validators.required],
    answerEn: ['', Validators.required],
    displayOrder: [0, Validators.min(0)],
    isActive: [true]
  });

  ngOnInit(): void {
    this.language.current$.subscribe(lang => this.currentLang.set(lang));
    if (this.faq) {
      this.form.patchValue({
        questionAr: this.faq.questionAr,
        questionEn: this.faq.questionEn,
        answerAr: this.faq.answerAr,
        answerEn: this.faq.answerEn,
        displayOrder: this.faq.displayOrder,
        isActive: this.faq.isActive
      });
    }
  }

  submit() {
    this.submitted = true;
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const payload: FaqPayload = this.form.getRawValue();
    this.dialogRef.close({id: this.faq?.id ?? null, payload});
  }

  cancel() {
    this.dialogRef.close(false);
  }

  requiredError() {
    return this.translate.instant('HOME_CONTENT.FIELD_REQUIRED');
  }
}
