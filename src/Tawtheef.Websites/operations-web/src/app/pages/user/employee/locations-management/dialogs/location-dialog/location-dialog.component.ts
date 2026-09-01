import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { InputTextModule } from 'primeng/inputtext';
import { TextareaModule } from 'primeng/textarea';
import { Observable } from 'rxjs';
import { finalize } from 'rxjs/operators';
import { NotificationService } from '../../../../../../core/services/notification.service';
import { LocationDto, SaveLocationRequest } from '../../models/location.dto';
import { LocationsService } from '../../services/locations.service';

@Component({
  selector: 'app-location-dialog',
  standalone: true,
  templateUrl: './location-dialog.component.html',
  styleUrls: ['./location-dialog.component.scss'],
  imports: [CommonModule, ReactiveFormsModule, TranslatePipe, InputTextModule, TextareaModule]
})
export class LocationDialogComponent {
  private readonly formBuilder = inject(FormBuilder);
  private readonly locationsService = inject(LocationsService);
  private readonly notification = inject(NotificationService);
  private readonly translate = inject(TranslateService);
  private readonly dialogRef = inject(DynamicDialogRef);
  private readonly dialogConfig = inject(DynamicDialogConfig);

  private readonly location = this.dialogConfig.data?.location as LocationDto | undefined;
  readonly isEditMode = !!this.location;
  readonly saving = signal(false);
  readonly submitted = signal(false);

  readonly form = this.formBuilder.group({
    nameAr: [this.location?.nameAr ?? '', [Validators.required, Validators.maxLength(200)]],
    nameEn: [this.location?.nameEn ?? '', Validators.maxLength(200)],
    locationLink: [
      this.location?.locationLink ?? '',
      [Validators.required, Validators.maxLength(500)]
    ],
    notes: [this.location?.notes ?? '', Validators.maxLength(2000)]
  });

  save(): void {
    this.submitted.set(true);
    if (this.form.invalid || this.saving()) {
      this.form.markAllAsTouched();
      return;
    }

    const value = this.form.getRawValue();
    const request: SaveLocationRequest = {
      nameAr: value.nameAr!.trim(),
      nameEn: value.nameEn?.trim() || null,
      locationLink: value.locationLink!.trim(),
      notes: value.notes?.trim() || null
    };

    this.saving.set(true);
    const saveRequest: Observable<string | void> = this.location
      ? this.locationsService.update(this.location.id, request)
      : this.locationsService.create(request);

    saveRequest
      .pipe(finalize(() => this.saving.set(false)))
      .subscribe({
        next: () => {
          const messageKey = this.isEditMode
            ? 'LOCATIONS.UPDATE_SUCCESS'
            : 'LOCATIONS.SAVE_SUCCESS';
          this.notification.success(this.translate.instant(messageKey));
          this.dialogRef.close(this.location?.id ?? true);
        }
      });
  }

  cancel(): void {
    if (!this.saving()) this.dialogRef.close();
  }

  showError(controlName: keyof typeof this.form.controls): boolean {
    const control = this.form.controls[controlName];
    return control.invalid && (control.touched || this.submitted());
  }
}
