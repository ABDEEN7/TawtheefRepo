import { Component, Input, Output, EventEmitter, ViewEncapsulation } from '@angular/core';

@Component({
  selector: 'app-stepper',
  standalone: false,
  templateUrl: './stepper.component.html',
  styleUrls: ['./stepper.component.scss'],
})
export class StepperComponent {
  @Input() current = 1;
  @Output() stepChange = new EventEmitter<number>();

  steps = [
    {
      id: 1,
      label: 'JOB_WIZARD.NAVIGATION.WIZARD_STEPS.OVERVIEW',
      icon: 'hgi hgi-stroke hgi-briefcase-03 me-1',
    },
    {
      id: 2,
      label: 'JOB_WIZARD.NAVIGATION.WIZARD_STEPS.QUALIFICATIONS',
      icon: 'hgi hgi-stroke hgi-school me-1 fw-normal',
    },
    {
      id: 3,
      label: 'JOB_WIZARD.NAVIGATION.WIZARD_STEPS.RESPONSIBILITIES',
      icon: 'fa fa-clipboard-check',
    },
    {
      id: 4,
      label: 'JOB_WIZARD.NAVIGATION.WIZARD_STEPS.CONDITIONS',
      icon: 'hgi hgi-stroke hgi-briefcase-05',
    },
    {
      id: 5,
      label: 'JOB_WIZARD.NAVIGATION.WIZARD_STEPS.SKILLS',
      icon: 'hgi hgi-stroke hgi-ai-user me-1 fw-normal',
    },
    {
      id: 6,
      label: 'JOB_WIZARD.NAVIGATION.WIZARD_STEPS.ATTACHMENTS',
      icon: 'hgi hgi-stroke hgi-file-upload me-1 fw-normal',
    },
    {
      id: 7,
      label: 'JOB_WIZARD.NAVIGATION.WIZARD_STEPS.BENEFITS',
      icon: 'hgi hgi-stroke hgi-checkmark-badge-01',
    },
  ];
}
