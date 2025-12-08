import { Component, Input, Output, EventEmitter } from '@angular/core';
import { icon } from '@primeuix/themes/aura/avatar';
import { label } from '@primeuix/themes/aura/metergroup';

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
      label: 'JOB_WIZARD.NAVIGATION.WIZARD_STEPS.BASICS',
      icon: 'fa fa-info-circle'
    },
    {
      id: 2,
      label: 'JOB_WIZARD.NAVIGATION.WIZARD_STEPS.OVERVIEW',
      icon: 'fa fa-info-circle',
    },
    {
      id: 3,
      label: 'JOB_WIZARD.NAVIGATION.WIZARD_STEPS.QUALIFICATIONS',
      icon: 'fa fa-graduation-cap',
    },
    {
      id: 4,
      label: 'JOB_WIZARD.NAVIGATION.WIZARD_STEPS.RESPONSIBILITIES',
      icon: 'fa fa-clipboard-check',
    },
    {
      id: 5,
      label: 'JOB_WIZARD.NAVIGATION.WIZARD_STEPS.CONDITIONS',
      icon: 'fa fa-list-check',
    },
    {
      id: 6,
      label: 'JOB_WIZARD.NAVIGATION.WIZARD_STEPS.SKILLS',
      icon: 'fa fa-lightbulb',
    },
    {
      id: 7,
      label: 'JOB_WIZARD.NAVIGATION.WIZARD_STEPS.ATTACHMENTS',
      icon: 'fa fa-paperclip',
    },
    {
      id: 8,
      label: 'JOB_WIZARD.NAVIGATION.WIZARD_STEPS.BENEFITS',
      icon: 'fa fa-gift',
    },
  ];
}
