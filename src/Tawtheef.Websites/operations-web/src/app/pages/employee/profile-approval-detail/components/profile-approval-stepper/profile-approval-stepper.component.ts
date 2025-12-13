import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { ReviewStatus } from '../../profile-approval/models/profile-approval.models';

export type StepUiStatus = 'done' | 'bad' | 'progress' | 'idle';

export interface ProfileApprovalStepperSection {
  section: number;
  uiStatus: StepUiStatus;
  icon: string;
  labelKey: string;
}

@Component({
  selector: 'app-profile-approval-stepper',
  standalone: true,
  imports: [CommonModule, TranslateModule],
  templateUrl: './profile-approval-stepper.component.html',
  styleUrl: './profile-approval-stepper.component.scss',
})
export class ProfileApprovalStepperComponent {
  @Input({ required: true })
  sections: ProfileApprovalStepperSection[] = [];

  @Input()
  activeSection: number | null = null;

  @Output()
  sectionChange = new EventEmitter<number>();

  selectSection(section: number): void {
    this.sectionChange.emit(section);
  }

  statusClass(status: ReviewStatus | null | undefined): StepUiStatus {
    if (status === ReviewStatus.Approved) return 'done';
    if (status === ReviewStatus.Rejected) return 'bad';
    return 'idle';
  }
}
