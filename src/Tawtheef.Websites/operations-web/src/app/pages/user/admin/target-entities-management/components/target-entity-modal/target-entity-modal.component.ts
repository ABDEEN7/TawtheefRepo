import {CommonModule} from '@angular/common';
import {
  Component,
  computed,
  EventEmitter,
  inject,
  Input,
  OnChanges,
  OnInit,
  Output,
  signal,
  SimpleChanges
} from '@angular/core';
import {FormBuilder, ReactiveFormsModule, Validators} from '@angular/forms';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import {Lang, LanguageService} from '../../../../../../core/services/language.service';
import {I18nNamespaceDirective} from '../../../../../../shared/directives/i18n-namespace.directive';
import {TargetEntityDto} from '../../models/target-entity.dto';

@Component({
  selector: 'app-target-entity-modal',
  standalone: true,
  templateUrl: './target-entity-modal.component.html',
  styleUrls: ['./target-entity-modal.component.scss'],
  imports: [CommonModule, ReactiveFormsModule, TranslatePipe, I18nNamespaceDirective]
})
export class TargetEntityModalComponent implements OnInit, OnChanges {
  private fb = inject(FormBuilder);
  private languageService = inject(LanguageService);
  private translate = inject(TranslateService);

  @Input() visible = false;
  @Input() mode: 'create' | 'edit' = 'create';
  @Input() targetEntity: TargetEntityDto | null = null;
  @Input() loading = false;

  @Output() cancel = new EventEmitter<void>();
  @Output() create = new EventEmitter<TargetEntityDto>();
  @Output() update = new EventEmitter<{ id: string; payload: TargetEntityDto }>();

  currentLang = signal<Lang>(this.languageService.get());
  isRtl = computed(() => this.currentLang() === 'ar');
  submitted = false;

  form = this.fb.nonNullable.group({
    nameAr: ['', Validators.required],
    nameEn: ['', Validators.required],
    descriptionAr: [''],
    descriptionEn: [''],
    isActive: [true]
  });

  ngOnInit(): void {
    this.languageService.current$.subscribe(lang => this.currentLang.set(lang));
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (this.visible && (changes['visible'] || changes['targetEntity'] || changes['mode'])) {
      this.patchForm();
    }
  }

  private patchForm() {
    this.submitted = false;
    this.form.reset({
      nameAr: '',
      nameEn: '',
      descriptionAr: '',
      descriptionEn: '',
      isActive: true
    });

    if (this.targetEntity) {
      this.form.patchValue({
        nameAr: this.targetEntity.nameAr,
        nameEn: this.targetEntity.nameEn,
        descriptionAr: this.targetEntity.descriptionAr || '',
        descriptionEn: this.targetEntity.descriptionEn || '',
        isActive: this.targetEntity.isActive
      });
    }
  }

  isCreateMode() {
    return this.mode === 'create';
  }

  isEditMode() {
    return this.mode === 'edit';
  }

  submit() {
    this.submitted = true;
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const {nameAr, nameEn, descriptionAr, descriptionEn, isActive} = this.form.getRawValue();

    if (this.isEditMode() && this.targetEntity && this.targetEntity.id) {
      this.update.emit({
        id: this.targetEntity.id,
        payload: {id: this.targetEntity.id, nameAr, nameEn, descriptionAr, descriptionEn, isActive}
      });
    } else {
      this.create.emit({
        id: null,
        nameAr,
        nameEn,
        descriptionAr,
        descriptionEn,
        isActive
      });
    }
  }

  close() {
    this.cancel.emit();
  }

  requiredError(controlName: 'nameAr' | 'nameEn'): string {
    if (this.form.controls[controlName].hasError('required')) {
      return this.translate.instant('TARGET_ENTITIES.FIELD_REQUIRED');
    }
    return '';
  }
}
