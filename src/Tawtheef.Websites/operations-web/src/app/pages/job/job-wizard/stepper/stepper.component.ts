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
      icon: 'fa fa-info-circle',
    },
    {
      id: 2,
      label: 'JOB_WIZARD.NAVIGATION.WIZARD_STEPS.QUALIFICATIONS',
      icon: 'fa fa-graduation-cap',
    },
    {
      id: 3,
      label: 'JOB_WIZARD.NAVIGATION.WIZARD_STEPS.RESPONSIBILITIES',
      icon: 'fa fa-clipboard-check',
    },
    {
      id: 4,
      label: 'JOB_WIZARD.NAVIGATION.WIZARD_STEPS.CONDITIONS',
      icon: 'fa fa-list-check',
    },
    {
      id: 5,
      label: 'JOB_WIZARD.NAVIGATION.WIZARD_STEPS.SKILLS',
      icon: 'fa fa-lightbulb',
    },
    {
      id: 6,
      label: 'JOB_WIZARD.NAVIGATION.WIZARD_STEPS.ATTACHMENTS',
      icon: 'fa fa-paperclip',
    },
    {
      id: 7,
      label: 'JOB_WIZARD.NAVIGATION.WIZARD_STEPS.BENEFITS',
      icon: 'fa fa-gift',
    },
  ];
}
