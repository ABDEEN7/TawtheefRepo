import { DatePipe } from '@angular/common';
import { Component, Input } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';
import { StatusPillComponent } from '../../../../../../../shared/components/status-pill/status-pill.component';
import { testSlotStatusTone } from '../../../test-slot-status-presentation';
import { TestSlotDetailsDto } from '../../../models/test-slot-details.dto';

@Component({
  selector: 'app-test-slot-basic-info',
  standalone: true,
  templateUrl: './test-slot-basic-info.component.html',
  styleUrl: './test-slot-basic-info.component.scss',
  imports: [DatePipe, StatusPillComponent, TranslatePipe],
})
export class TestSlotBasicInfoComponent {
  readonly testSlotStatusTone = testSlotStatusTone;
  @Input({ required: true }) details!: TestSlotDetailsDto;
}
