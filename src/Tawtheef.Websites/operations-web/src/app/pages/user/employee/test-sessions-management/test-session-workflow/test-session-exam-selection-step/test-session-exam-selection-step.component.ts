import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TranslatePipe } from '@ngx-translate/core';
import { SelectModule } from 'primeng/select';
import { dropdownOptionsModel } from '../../../../../../shared/models/dropdown-options.model';
import { TestSessionExamDetailsDto } from '../../models/test-session-exam-details.dto';

@Component({
  selector: 'app-test-session-exam-selection-step',
  standalone: true,
  templateUrl: './test-session-exam-selection-step.component.html',
  styleUrl: './test-session-exam-selection-step.component.scss',
  imports: [FormsModule, TranslatePipe, SelectModule],
})
export class TestSessionExamSelectionStepComponent {
  @Input() exams: dropdownOptionsModel[] = [];
  @Input() selectedExamId?: string;
  @Input() examDetails?: TestSessionExamDetailsDto;
  @Input() loading = false;
  @Input() showValidation = false;
  @Output() examSelected = new EventEmitter<string | undefined>();
}
