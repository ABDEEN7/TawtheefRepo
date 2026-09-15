import { Component, Input } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { TranslatePipe } from '@ngx-translate/core';
import { DatePickerModule } from 'primeng/datepicker';
import { InputTextModule } from 'primeng/inputtext';
import { SelectModule } from 'primeng/select';
import { dropdownOptionsModel } from '../../../../../../../shared/models/dropdown-options.model';
import { testSlotCreateForm } from '../../helper/test-slot-create.form';

@Component({
  selector: 'app-slot-details',
  standalone: true,
  templateUrl: './slot-details.component.html',
  styleUrl: './slot-details.component.scss',
  imports: [ReactiveFormsModule, TranslatePipe, DatePickerModule, InputTextModule, SelectModule],
})
export class SlotDetailsComponent {
  @Input({ required: true }) form!: ReturnType<typeof testSlotCreateForm>;
  @Input() rooms: dropdownOptionsModel[] = [];
}
