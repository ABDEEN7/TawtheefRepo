import { Component, inject } from '@angular/core';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { TranslatePipe } from '@ngx-translate/core';
import { CommonModule } from '@angular/common';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { JobReviewAttachment } from '../../models/job-review-attachment';

@Component({
  selector: 'app-job-review-attachments-modal',
  imports: [
    FormsModule,
    ReactiveFormsModule,
    TranslatePipe,
    CommonModule
  ],
  templateUrl: './job-review-attachments-modal.component.html',
  styleUrl: './job-review-attachments-modal.component.scss',
})
export class JobReviewAttachmentsModalComponent {
  public ref = inject(DynamicDialogRef);
  public config = inject(DynamicDialogConfig);
  private fb = inject(FormBuilder);

  attachments: JobReviewAttachment[] = [];
  attachmentForm: FormGroup;

  constructor() {
    this.attachmentForm = this.fb.group({
      title: ['', Validators.required],
      file: [null, Validators.required]
    });

    if (this.config.data?.existingAttachments) {
      this.attachments = [...this.config.data.existingAttachments];
    }
  }

  onFileChange(event: Event): void {
    const input = event.target as HTMLInputElement;
    const files = input.files;
    if (files && files.length > 0) {
      const file = files[0];
      this.attachmentForm.patchValue({
        file: file,
        title: file.name
      });
    }
  }

  addAttachment(): void {
    if (this.attachmentForm.invalid) {
      this.markFormGroupTouched(this.attachmentForm);
      return;
    }

    const formValue = this.attachmentForm.value;
    const newAttachment: JobReviewAttachment = {
      fileName: formValue.title,
      file: formValue.file
    };

    this.attachments.push(newAttachment);
    this.attachmentForm.reset();
    
    const fileInput = document.querySelector('input[type="file"]') as HTMLInputElement;
    if (fileInput) {
      fileInput.value = '';
    }
  }

  removeAttachment(index: number): void {
    this.attachments.splice(index, 1);
  }

  previewAttachment(attachment: JobReviewAttachment): void {
    if (!attachment.file) return;

    const fileURL = URL.createObjectURL(attachment.file);
    const fileType = attachment.file.type;

    if (fileType === 'application/pdf') {
      window.open(fileURL, '_blank');
    } else if (fileType.startsWith('image/')) {
      window.open(fileURL, '_blank');
    } else {
      const link = document.createElement('a');
      link.href = fileURL;
      link.download = attachment.fileName;
      link.click();
    }
  }

  save(): void {
    this.ref.close(this.attachments);
  }

  cancel(): void {
    this.ref.close();
  }

  private markFormGroupTouched(formGroup: FormGroup): void {
    Object.values(formGroup.controls).forEach(control => {
      control.markAsTouched();
      if (control instanceof FormGroup) {
        this.markFormGroupTouched(control);
      }
    });
  }

  get fileName(): string {
    const file = this.attachmentForm.get('file')?.value;
    return file ? file.name : '';
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.attachmentForm.get(fieldName);
    return !!(field && field.invalid && field.touched);
  }
}