import { FormGroup } from '@angular/forms';
import { Job } from '../../../models/job.model';
import { JobTabReviewNoteResponse } from '../../../models/job-tab-review-note-response';

export abstract class WizardStepComponent {
  abstract readonly form: FormGroup;
  abstract isValid(): boolean;
  setJobData?(job: Job, notes?: JobTabReviewNoteResponse | null): void;
}