import { Component, Input, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { JobTabReviewNoteResponse } from '../../models/job-tab-review-note-response';
import { JobService } from '../../services/job.service';
import { JobStatus } from '../../../../core/enums/lookups.enum';

@Component({
  selector: 'app-review-note',
  standalone: true,
  imports: [CommonModule, TranslateModule],
  templateUrl: './review-note.component.html',
  styleUrls: ['./review-note.component.scss']
})
export class ReviewNoteComponent {
  @Input({ required: true }) note: JobTabReviewNoteResponse | null = null;

  private jobService = inject(JobService);
  currentJob = this.jobService.getCurrentJob();
  jobStatuses = JobStatus
}
