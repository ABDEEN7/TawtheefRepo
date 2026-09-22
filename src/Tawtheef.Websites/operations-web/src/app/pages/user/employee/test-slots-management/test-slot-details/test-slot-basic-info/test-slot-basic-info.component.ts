import { DatePipe } from '@angular/common';
import { Component, Input } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';
import { TagModule } from 'primeng/tag';
import { TestSlotDetailsDto } from '../../models/test-slot-details.dto';

@Component({
  selector: 'app-test-slot-basic-info',
  standalone: true,
  templateUrl: './test-slot-basic-info.component.html',
  styleUrl: './test-slot-basic-info.component.scss',
  imports: [DatePipe, TagModule, TranslatePipe],
})
export class TestSlotBasicInfoComponent {
  @Input({ required: true }) details!: TestSlotDetailsDto;
}
