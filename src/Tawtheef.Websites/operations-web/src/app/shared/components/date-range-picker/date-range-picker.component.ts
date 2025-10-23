import {Component, EventEmitter, Output} from '@angular/core';
import {FormsModule} from "@angular/forms";
import {BsDaterangepickerDirective} from "ngx-bootstrap/datepicker";

@Component({
  selector: 'app-date-range-picker',
  templateUrl: './date-range-picker.component.html',
  styleUrl: './date-range-picker.component.scss',
  standalone: true,
  imports: [
    FormsModule,
    BsDaterangepickerDirective
  ]
})
export class DateRangePickerComponent {
  @Output() rangeChange = new EventEmitter<{from?: Date, to?: Date}>();
  bsValue = this.isoDaysAgo(365);
  bsRangeValue: Date[];
  maxDate = new Date();

  constructor() {
    this.maxDate.setDate(this.maxDate.getDate() + 7);
    this.bsRangeValue = [this.bsValue, this.maxDate];
  }

  onRangeChanged(event: {from?: Date, to?: Date}) {
    this.rangeChange.emit(event);
  }
  isoDaysAgo(days: number): Date {
    const d = new Date();
    d.setDate(d.getDate() - days);
    return d;
  }
}
