import { Component, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-stepper',
  standalone: false,
  templateUrl: './stepper.component.html',
  styleUrls: ['./stepper.component.scss']
})
export class StepperComponent {
  @Input() current = 1;
  @Output() stepChange = new EventEmitter<number>();

  steps = [
    { id: 1, icon: 'fa fa-clipboard-list', label: 'job_wizard.navigation.wizard_steps.basics' },
    { id: 2, icon: 'fa fa-flag', label: 'job_wizard.navigation.wizard_steps.quotas' },
    { id: 3, icon: 'fa fa-list-check', label: 'job_wizard.navigation.wizard_steps.conditions' },
    { id: 4, icon: 'fa fa-lightbulb', label: 'job_wizard.navigation.wizard_steps.skills' },
    { id: 5, icon: 'fa fa-file-lines', label: 'job_wizard.navigation.wizard_steps.description_benefits' },
    { id: 6, icon: 'fa fa-eye', label: 'job_wizard.navigation.wizard_steps.review' }
  ];
}
