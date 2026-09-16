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
  readonly minSlotDate = new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate());
  @Input({ required: true }) form!: ReturnType<typeof testSlotCreateForm>;
  @Input() rooms: RoomListItemDto[] = [];

  get roomLabel(): 'nameAr' | 'nameEn' {
    return this.language.isRtl ? 'nameAr' : 'nameEn';
  }
}
