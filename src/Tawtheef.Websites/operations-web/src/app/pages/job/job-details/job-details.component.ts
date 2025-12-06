import { ChangeDetectorRef, Component, inject, OnInit } from '@angular/core';
import { DialogService } from 'primeng/dynamicdialog';
import { ActivatedRoute, Router } from '@angular/router';
import { JobService } from '../services/job.service';
import { NotificationService } from '../../../core/services/notification.service';
import { ConfirmApplyModalComponent } from '../modals/confirm-apply-modal/confirm-apply-modal.component';
import { GUID } from '../../../shared/types/guid.type';
import { JobLookupService } from '../services/job-lookup.service';
import { JobResponse } from '../models/job-response-model';

@Component({
  selector: 'app-job-details',
  templateUrl: './job-details.component.html',
  styleUrls: ['./job-details.component.scss'],
  standalone: false,
})
export class JobDetailsComponent implements OnInit {
  job: JobResponse | undefined;
  id!: GUID;

  activeTab: string = 'desc';
  hasApplied: boolean = false;

  private cdr = inject(ChangeDetectorRef);
  private jobService = inject(JobService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private dialogService = inject(DialogService);
  private notificationService = inject(NotificationService);
  lookupsService = inject(JobLookupService);

  ngOnInit() {
    this.id = this.route.snapshot.paramMap.get('id') as GUID;
    this.loadJobById();
    this.lookupsService.loadAll();
  }

  private loadJobById(): void {
    this.jobService.getById(this.id).subscribe({
      next: (job) => {
        this.job = job;
        this.cdr.detectChanges();
      },
      error: () => {
        this.notificationService.error("job_details.failed_to_load");
        this.router.navigate(['/jobs']);
      },
    });
  }

  setActiveTab(tab: string) {
    this.activeTab = tab;
  }

  openConfirmModal(): void {
    const ref = this.dialogService.open(ConfirmApplyModalComponent, {
      width: '600px',
      height: '350px',
      dismissableMask: true,
      closeOnEscape: true,
    });

    ref?.onClose.subscribe((confirmed: boolean) => {
      if (confirmed) {
        this.confirmApply();
        this.notificationService.success('job_details.apply_success');
      }
    });
  }

  confirmApply(): void {
    this.hasApplied = true;
  }

  getResponsibilities(): string[] {
    if (!this.job?.responsibilities?.length) return [];
    return this.job.responsibilities.map(r => r.textAr);
  }

  getJobConditions(): string[] {
    if (!this.job?.conditions?.length) return [];
    return this.job.conditions.map(c => c.textAr);
  }

  getJobSkills(): string[] {
    if (!this.job?.skills?.length) return [];
    return this.job.skills
      .filter(skill => skill.showToApplicants)
      .map(skill => {
        const skillName =
          this.lookupsService.skills().find(s => s.id === skill.skillId)?.name ||
          'job_details.unknown_skill';
        return skillName;
      });
  }

  getJobBenefits(): string[] {
    if (!this.job?.benefitsAr) return [];
    return this.job.benefitsAr
      .split(/[\n,]+/)
      .map((benefit) => benefit.trim())
      .filter(b => b);
  }

  getDegreeRequirements(): string {
    if (!this.job?.degrees?.length) return 'job_details.not_specified';

    const degreeNames = this.job.degrees
      .map(degree =>
        this.lookupsService.degrees().find(d => d.id === degree.degreeId)?.name || ''
      )
      .filter(name => name);

    return degreeNames.join(', ') || 'job_details.not_specified';
  }

  formatDate(date: Date | string | undefined): string {
    if (!date) return 'job_details.not_specified';
    return new Date(date).toLocaleDateString('ar-SA');
  }

  isJobOpen(): boolean {
    if (!this.job?.closingDate) return true;
    const closingDate = new Date(this.job.closingDate);
    const today = new Date();
    return closingDate >= today;
  }

  getVacancyStatus(): string {
    if (!this.job?.numberOfVacancies) return 'job_details.not_specified';
    return `${this.job.numberOfVacancies} ${this.job.numberOfVacancies > 1 ? 'job_details.vacancy_plural' : 'job_details.vacancy_single'}`;
  }
}
