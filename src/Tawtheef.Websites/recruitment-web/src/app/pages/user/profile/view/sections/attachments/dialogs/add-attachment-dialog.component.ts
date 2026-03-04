import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { ButtonDirective } from 'primeng/button';
import { InputText } from 'primeng/inputtext';
import { DynamicDialogRef } from 'primeng/dynamicdialog';

import { Attachment } from '../../../../wizard-profile/models/attachment.model';
import { I18nNamespaceDirective } from '../../../../../../../shared/directives/i18n-namespace.directive';

@Component({
  selector: 'app-add-attachment-dialog',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, TranslatePipe, ButtonDirective, InputText, I18nNamespaceDirective],
  template: `
    <section class="attachment-dialog" i18nNamespace="pages/user/profile-overview">
      <div class="mb-3">
        <label class="form-label fw-semibold" for="attachmentTitle">
          {{ 'profileView.attachments.titleLabel' | translate }}
        </label>
        <input
          pInputText
          id="attachmentTitle"
          type="text"
          class="w-100"
          [formControl]="form.controls.title"
        />
        <div class="text-danger small mt-1" *ngIf="form.controls.title.touched && form.controls.title.invalid">
          {{ 'validation.required' | translate }}
        </div>
      </div>

      <div class="mb-3">
        <label class="form-label fw-semibold" for="attachmentFile">
          {{ 'profileView.attachments.fileLabel' | translate }}
        </label>
        <input
          id="attachmentFile"
          type="file"
          class="form-control"
          [accept]="allowedTypes.join(',')"
          (change)="onFileChange($event)"
        />
        <div class="text-muted small mt-1" *ngIf="fileName">
          {{ fileName }}
        </div>
        <div class="text-danger small mt-1" *ngIf="fileError">
          {{ fileError }}
        </div>
      </div>

      <div class="d-flex justify-content-end gap-2">
        <button pButton type="button" class="p-button-text" (click)="close()">
          {{ 'common.cancel' | translate }}
        </button>
        <button pButton type="button" (click)="save()">
          {{ 'common.save' | translate }}
        </button>
      </div>
    </section>
  `,
})
export class AddAttachmentDialogComponent {
  private readonly ref = inject(DynamicDialogRef);
  private readonly fb = inject(FormBuilder);
  private readonly translate = inject(TranslateService);

  protected readonly allowedTypes = ['application/pdf', 'image/png', 'image/jpeg'];
  protected readonly maxFileSize = 1_000_000;
  protected fileError: string | null = null;
  protected fileName: string | null = null;
  private file: File | null = null;

  protected readonly form = this.fb.group({
    title: ['', Validators.required],
  });

  onFileChange(event: Event): void {
    this.fileError = null;
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0] ?? null;
    if (!file) {
      this.fileName = null;
      this.file = null;
      return;
    }

    if (!this.allowedTypes.includes(file.type)) {
      this.fileError = this.translate.instant('validation.fileType', { types: 'PDF, PNG, JPEG, WEBP' });
      this.fileName = null;
      this.file = null;
      return;
    }

    if (file.size > this.maxFileSize) {
      this.fileError = this.translate.instant('validation.fileSize', { size: '1MB' });
      this.fileName = null;
      this.file = null;
      return;
    }

    this.fileName = file.name;
    this.file = file;
  }

  save(): void {
    if (this.form.invalid || !this.file || this.fileError) {
      this.form.markAllAsTouched();
      if (!this.file) {
        this.fileError = this.translate.instant('validation.required');
      }
      return;
    }

    const attachment: Attachment = {
      title: this.form.value.title ?? '',
      fileName: this.file.name,
      file: this.file,
    };

    this.ref.close(attachment);
  }

  close(): void {
    this.ref.close();
  }
}
