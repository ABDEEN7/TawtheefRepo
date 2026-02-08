import {Component, OnInit, inject} from '@angular/core';
import {AbstractControl, FormBuilder, FormGroup, ReactiveFormsModule, ValidationErrors, Validators} from '@angular/forms';
import {DynamicDialogConfig, DynamicDialogRef} from 'primeng/dynamicdialog';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import {Select} from 'primeng/select';
import {DatePicker} from 'primeng/datepicker';
import {InputText} from 'primeng/inputtext';
import {TextareaModule} from 'primeng/textarea';
import {NgClass, NgIf} from '@angular/common';
import {ProfileLookupsService} from '../../../../wizard-profile/services/profile-lookups.service';
import {FileUtilsService} from '../../../../../../../core/utils/file-utils';
import {Achievement} from '../../../../wizard-profile/models/achievement.model';
import {dateToDateOnly} from '../../../../../../../shared/types/dateOnly.type';
import {dropdownOptionsModel} from '../../../../../../../shared/models/dropdown-options.model';
import {ACHIEVEMENTS_DIALOG_LIMITS} from '../../step-experience/dialogs/dialog-config';
import {GUID} from '../../../../../../../shared/types/guid.type';
import {I18nNamespaceDirective} from '../../../../../../../shared/directives/i18n-namespace.directive';

@Component({
  selector: 'app-achievement-modal',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    TranslatePipe,
    Select,
    DatePicker,
    InputText,
    TextareaModule,
    NgIf,
    NgClass,
    I18nNamespaceDirective,
  ],
  templateUrl: './achievement.modal.html',
  styleUrl: './achievement.modal.scss',
})
export class AchievementModal implements OnInit {
  private fb = inject(FormBuilder);
  private ref = inject(DynamicDialogRef);
  private config = inject(DynamicDialogConfig);
  protected lookups = inject(ProfileLookupsService);
  private translate = inject(TranslateService);
  private fileUtils = inject(FileUtilsService);

  readonly limits = ACHIEVEMENTS_DIALOG_LIMITS;
  readonly allowedTypes = ['application/pdf', 'image/png', 'image/jpeg', 'image/webp'];
  fileError: string | null = null;
  initialAttachmentUrl: string | null = null;
  private initialId: GUID | null = null;
  private initialAttachmentId: GUID | null = null;
  today = new Date();

  form: FormGroup = this.fb.group({
    achievementType: [null, Validators.required],
    title: ['', [Validators.required, Validators.maxLength(150)]],
    issuingAuthority: ['', [Validators.required, Validators.maxLength(150)]],
    country: [null, Validators.required],
    issueDate: [null, Validators.required],
    description: ['', [Validators.maxLength(this.limits.descriptionMaxLength)]],
    fileName: [''],
    file: [null],
    relatedToSpecialization: [null],
  });

  ngOnInit(): void {
    const initial = this.config.data?.initialValue as Achievement | undefined;
    if (initial) {
      this.form.patchValue({
        achievementType: initial.achievementType,
        title: initial.title,
        issuingAuthority: initial.issuingAuthority,
        country: this.lookups.countries().find(c => c.id === initial.country?.id) ?? null,
        issueDate: initial.issueDate ? new Date(initial.issueDate) : null,
        description: initial.description,
        fileName: initial.fileName || initial.attachment?.resourceName,
        relatedToSpecialization: initial.relatedToSpecialization ?? null,
      });
      this.initialAttachmentUrl = initial.attachment?.url ?? null;
      this.initialId = this.config.data?.initialId ?? initial.id ?? null;
      this.initialAttachmentId =
        this.config.data?.attachmentId ?? initial.attachmentId ?? initial.attachment?.resourceId ?? null;

      if (!initial.file && initial.attachment) {
        this.form.get('file')?.clearValidators();
      } else {
        this.form.get('file')?.addValidators(Validators.required);
      }
    } else {
      this.form.get('file')?.addValidators(Validators.required);
    }
    this.form.updateValueAndValidity({ emitEvent: false });
    this.form.get('achievementType')?.valueChanges.subscribe(() => this.syncSpecializationRequirement());
    this.syncSpecializationRequirement();
  }

  onUpload(evt: any) {
    this.fileError = null;
    const input = evt.target as HTMLInputElement;
    const file = input.files?.[0] ?? null;
    if (!file) return;

    if (!this.allowedTypes.includes(file.type)) {
      this.fileError = this.translate.instant('validation.fileType', { types: 'PDF, PNG, JPEG, WEBP' });
      this.form.patchValue({ file: null, fileName: '' });
      return;
    }

    if (file.size > this.limits.maxFileSizeBytes) {
      this.fileError = this.translate.instant('validation.fileSize', { size: this.limits.maxFileSizeLabel });
      this.form.patchValue({ file: null, fileName: '' });
      return;
    }

    this.form.patchValue({ file, fileName: file.name });
  }

  previewFile(ev?: Event): void {
    ev?.stopPropagation();
    const file = this.form.get('file')?.value as File | null;
    if (file) {
      this.fileUtils.previewBlob(file);
      return;
    }

    if (this.initialAttachmentUrl) {
      this.fileUtils.previewUrl(this.initialAttachmentUrl, this.form.get('fileName')?.value ?? '', false);
    }
  }

  onSave() {
    if (this.form.invalid || this.fileError) {
      this.form.markAllAsTouched();
      return;
    }

    const v = this.form.getRawValue();
    const payload: Achievement = {
      id: this.initialId ?? undefined,
      achievementTypeId: v.achievementTypeId,
      achievementType: v.achievementType,
      title: v.title,
      issuingAuthority: v.issuingAuthority,
      countryId: v.countryId,
      country: v.country,
      issueDate: dateToDateOnly(v.issueDate)!,
      description: v.description,
      file: v.file,
      fileName: v.file?.name ?? v.fileName ?? null,
      attachment: this.config.data?.initialValue?.attachment,
      attachmentId: this.initialAttachmentId ?? undefined,
      relatedToSpecialization: this.shouldShowSpecializationQuestion ? !!v.relatedToSpecialization : null,
    };

    this.ref.close(payload);
  }

  onCancel() {
    this.ref.close();
  }

  get f() {
    return this.form.controls;
  }

  get shouldShowSpecializationQuestion(): boolean {
    const type = this.form.get('achievementType')?.value as dropdownOptionsModel | null;
    const backend = type?.backendName?.toLowerCase() ?? '';
    return backend.includes('certificate');
  }

  private syncSpecializationRequirement() {
    const ctrl = this.form.get('relatedToSpecialization');
    if (!ctrl) return;

    if (this.shouldShowSpecializationQuestion) {
      ctrl.setValidators([control => this.booleanRequired(control)]);
    } else {
      ctrl.clearValidators();
      ctrl.setValue(null, { emitEvent: false });
    }
    ctrl.updateValueAndValidity({ emitEvent: false });
  }

  private booleanRequired(control: AbstractControl): ValidationErrors | null {
    const v = control.value;
    return v === true || v === false ? null : { required: true };
  }
}
