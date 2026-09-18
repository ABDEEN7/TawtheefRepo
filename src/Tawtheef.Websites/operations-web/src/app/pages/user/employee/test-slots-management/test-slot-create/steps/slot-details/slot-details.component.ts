import { Component, inject, Input } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { TranslatePipe } from '@ngx-translate/core';
import { DatePickerModule } from 'primeng/datepicker';
import { InputTextModule } from 'primeng/inputtext';
import { SelectModule } from 'primeng/select';
import { LanguageService } from '../../../../../../../core/services/language.service';
import { RoomListItemDto } from '../../../../rooms-management/models/room-list-item.dto';
import { testSlotCreateForm } from '../../helper/test-slot-create.form';

@Component({
  selector: 'app-slot-details',
  standalone: true,
  templateUrl: './slot-details.component.html',
  styleUrl: './slot-details.component.scss',
  imports: [ReactiveFormsModule, TranslatePipe, DatePickerModule, InputTextModule, SelectModule],
})
export class SlotDetailsComponent {
  readonly language = inject(LanguageService);
  private readonly today = new Date(
    new Date().getFullYear(),
    new Date().getMonth(),
    new Date().getDate(),
  );
  @Input({ required: true }) form!: ReturnType<typeof testSlotCreateForm>;
  @Input() rooms: RoomListItemDto[] = [];
  @Input() isViewMode = false;
  @Input() validationErrors: string[] = [];

  get minSlotDate(): Date | undefined {
    const slotDate = this.form.controls.slotDate.value;
    return slotDate && this.isBeforeToday(slotDate) ? undefined : this.today;
  }

  get roomLabel(): 'nameAr' | 'nameEn' {
    return this.language.isRtl ? 'nameAr' : 'nameEn';
  }

  private isBeforeToday(value: Date): boolean {
    const date = new Date(value);
    date.setHours(0, 0, 0, 0);
    return date < this.today;
  }

  showError(controlName: keyof ReturnType<typeof testSlotCreateForm>['controls']): boolean {
    const control = this.form.controls[controlName];
    return control.invalid && (control.touched || control.dirty);
  }

  hasValidationError(error: string): boolean {
    return this.validationErrors.includes(error);
  }

  titleArIsEmpty(): boolean {
    const control = this.form.controls.titleAr;
    return (control.touched || control.dirty) && !control.value.trim();
  }
}
