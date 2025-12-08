import { FormGroup } from '@angular/forms';
import { Job } from '../../../models/job.model';

export abstract class WizardStepComponent {
  abstract readonly form: FormGroup;
  abstract isValid(): boolean;
  abstract setJobData(job: Job): void;
}