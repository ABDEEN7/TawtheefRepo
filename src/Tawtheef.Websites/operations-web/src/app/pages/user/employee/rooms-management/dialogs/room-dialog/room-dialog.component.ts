import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { InputNumberModule } from 'primeng/inputnumber';
import { InputTextModule } from 'primeng/inputtext';
import { Select } from 'primeng/select';
import { TextareaModule } from 'primeng/textarea';
import { finalize } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { NotificationService } from '../../../../../../core/services/notification.service';
import { dropdownOptionsModel } from '../../../../../../shared/models/dropdown-options.model';
import { CreateRoomRequest } from '../../models/create-room-request.dto';
import { RoomListItemDto } from '../../models/room-list-item.dto';
import { LocationDto } from '../../../locations-management/models/location.dto';
import { RoomsService } from '../../services/rooms.service';



@Component({
  selector: 'app-create-room-dialog',
  standalone: true,
  templateUrl: './room-dialog.component.html',
  styleUrls: ['./room-dialog.component.scss'],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    TranslatePipe,
    InputTextModule,
    InputNumberModule,
    Select,
    TextareaModule
  ]
})
export class RoomDialogComponent {
  private readonly formBuilder = inject(FormBuilder);
  private readonly roomsService = inject(RoomsService);
  private readonly notification = inject(NotificationService);
  private readonly translate = inject(TranslateService);
  private readonly dialogRef = inject(DynamicDialogRef);
  private readonly dialogConfig = inject(DynamicDialogConfig);

  private readonly room = this.dialogConfig.data?.room as RoomListItemDto | undefined;
  readonly isEditMode = !!this.room;

  readonly saving = signal(false);
  readonly submitted = signal(false);
  readonly roomTypeOptions = this.dialogConfig.data?.roomTypes as dropdownOptionsModel[] ?? [];
  readonly statusOptions = this.dialogConfig.data?.statuses as dropdownOptionsModel[] ?? [];
  readonly locationOptions = (this.dialogConfig.data?.locations as LocationDto[]) ?? [];

  readonly form = this.formBuilder.group({
    nameAr: [this.room?.nameAr ?? '', [Validators.required, Validators.maxLength(200)]],
    nameEn: [this.room?.nameEn ?? '', [Validators.required, Validators.maxLength(200)]],
    locationId: [this.room?.locationId ?? null as string | null, Validators.required],
    roomTypeId: [this.room?.roomTypeId ?? null as string | null, Validators.required],
    capacity: [this.room?.capacity ?? null as number | null, [Validators.required, Validators.min(1)]],
    statusId: [this.room?.statusId ?? null as string | null, Validators.required],
    notes: [this.room?.notes ?? '', Validators.maxLength(2000)]
  });

  save(): void {
    this.submitted.set(true);
    if (this.form.invalid || this.saving()) {
      this.form.markAllAsTouched();
      return;
    }

    const value = this.form.getRawValue();
    const request: CreateRoomRequest = {
      nameAr: value.nameAr!.trim(),
      nameEn: value.nameEn!.trim(),
      locationId: value.locationId!,
      roomTypeId: value.roomTypeId!,
      capacity: value.capacity!,
      statusId: value.statusId!,
      notes: value.notes?.trim() || null
    };

    this.saving.set(true);
    const saveRequest: Observable<string | void> = this.room
      ? this.roomsService.update(this.room.id, request)
      : this.roomsService.create(request);

    saveRequest
      .pipe(finalize(() => this.saving.set(false)))
      .subscribe({
        next: () => {
          const messageKey = this.isEditMode ? 'ROOMS.UPDATE_SUCCESS' : 'ROOMS.SAVE_SUCCESS';
          this.notification.success(this.translate.instant(messageKey));
          this.dialogRef.close(this.room?.id ?? true);
        }
      });
  }

  locationName(location: LocationDto): string {
    return this.translate.currentLang === 'ar'
      ? location.nameAr
      : (location.nameEn || location.nameAr);
  }

  cancel(): void {
    if (!this.saving()) this.dialogRef.close();
  }

  showError(controlName: keyof typeof this.form.controls): boolean {
    const control = this.form.controls[controlName];
    return control.invalid && (control.touched || this.submitted());
  }
}
