import {CommonModule} from '@angular/common';
import {Component, computed, inject, OnInit, signal} from '@angular/core';
import {FormBuilder, ReactiveFormsModule, Validators} from '@angular/forms';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import {DynamicDialogConfig, DynamicDialogRef} from 'primeng/dynamicdialog';
import {I18nNamespaceDirective} from '../../../../../shared/directives/i18n-namespace.directive';
import {Lang, LanguageService} from '../../../../../core/services/language.service';
import {HomeSuccessStory, HomeSuccessStoryPayload} from '../models/home-success-story.model';

interface SuccessStoryDialogData {
  mode: 'create' | 'edit';
  story?: HomeSuccessStory;
}

@Component({
  selector: 'app-home-success-story-dialog',
  standalone: true,
  templateUrl: './home-success-story-dialog.component.html',
  styleUrls: ['./home-success-story-dialog.component.scss'],
  imports: [CommonModule, ReactiveFormsModule, TranslatePipe, I18nNamespaceDirective]
})
export class HomeSuccessStoryDialogComponent implements OnInit {
  private fb = inject(FormBuilder);
  private dialogRef = inject(DynamicDialogRef);
  private config = inject(DynamicDialogConfig<SuccessStoryDialogData>);
  private translate = inject(TranslateService);
  private language = inject(LanguageService);

  private story = this.config.data?.story;
  currentLang = signal<Lang>(this.language.get());
  isRtl = computed(() => this.currentLang() === 'ar');
  submitted = false;

  form = this.fb.nonNullable.group({
    nameAr: ['', Validators.required],
    nameEn: ['', Validators.required],
    roleAr: ['', Validators.required],
    roleEn: ['', Validators.required],
    metricTitleAr: ['', Validators.required],
    metricTitleEn: ['', Validators.required],
    metricDescriptionAr: ['', Validators.required],
    metricDescriptionEn: ['', Validators.required],
    imageUrl: ['', Validators.required],
    displayOrder: [0, Validators.min(0)],
    isActive: [true]
  });

  ngOnInit(): void {
    this.language.current$.subscribe(lang => this.currentLang.set(lang));
    if (this.story) {
      this.form.patchValue({
        nameAr: this.story.nameAr,
        nameEn: this.story.nameEn,
        roleAr: this.story.roleAr,
        roleEn: this.story.roleEn,
        metricTitleAr: this.story.metricTitleAr,
        metricTitleEn: this.story.metricTitleEn,
        metricDescriptionAr: this.story.metricDescriptionAr,
        metricDescriptionEn: this.story.metricDescriptionEn,
        imageUrl: this.story.imageUrl,
        displayOrder: this.story.displayOrder,
        isActive: this.story.isActive
      });
    }
  }

  submit() {
    this.submitted = true;
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const payload: HomeSuccessStoryPayload = this.form.getRawValue();
    this.dialogRef.close({id: this.story?.id ?? null, payload});
  }

  cancel() {
    this.dialogRef.close(false);
  }

  requiredError() {
    return this.translate.instant('HOME_CONTENT.FIELD_REQUIRED');
  }
}
