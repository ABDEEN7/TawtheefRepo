import { ChangeDetectorRef, Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { JobService } from '../services/job.service';
import { GUID } from '../../../../../shared/types/guid.type';
import { JobLookupService } from '../services/job-lookup.service';
import { JobResponse } from '../models/job-response-model';
import { JobStatus } from '../../../../../core/enums/lookups.enum';
import { JobTabType } from '../enums/job-tab-type';
import { JobBasicModalComponent } from '../modals/basics-step-modal/job-basic-modal.component';
import { DialogService } from 'primeng/dynamicdialog';
import { TranslateService } from '@ngx-translate/core';
import { JobSkillResponse } from '../models/job-skill-response.model';
import { JobSpecializationResponse } from '../models/job-specialization-response.model';
import { routes } from '../../../../../routes/routes';

@Component({
  selector: 'app-job-details',
  templateUrl: './job-details.component.html',
  styleUrls: ['./job-details.component.scss'],
  standalone: false,
})
export class JobDetailsComponent implements OnInit {
  job!: JobResponse;
  id!: GUID;
  jobStatus = JobStatus;

  activeTab: string = JobTabType.Overview;
  hasApplied: boolean = false;
  isFavorite: boolean = false;
  tabType = JobTabType;

  private tabsContent = [
    { id: JobTabType.Overview, title: 'JOB_DETAILS.OVERVIEW', icon: 'fa-file-alt' },
    { id: JobTabType.Responsibilities, title: 'JOB_DETAILS.RESPONSIBILITIES', icon: 'fa-tasks' },
    { id: JobTabType.Qualifications, title: 'JOB_DETAILS.QUALIFICATIONS', icon: 'fa-graduation-cap' },
    { id: JobTabType.Conditions, title: 'JOB_DETAILS.CONDITIONS', icon: 'fa-clipboard-list' },
    { id: JobTabType.Skills, title: 'JOB_DETAILS.SKILLS', icon: 'fa-tools' },
    { id: JobTabType.Benefits, title: 'JOB_DETAILS.BENEFITS', icon: 'fa-gift' },
    { id: JobTabType.Attachments, title: 'JOB_DETAILS.REQUIRED_ATTACHMENTS', icon: 'fa-paperclip' }
  ];

  private cdr = inject(ChangeDetectorRef);
  private jobService = inject(JobService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  lookupsService = inject(JobLookupService);
  private dialogService = inject(DialogService);
  private translateService = inject(TranslateService);

  ngOnInit() {
    this.id = this.route.snapshot.paramMap.get('id') as GUID;
    this.loadJobById();
  }

  private loadJobById(): void {
    this.jobService.getById(this.id).subscribe({
      next: (job) => {
        this.job = job;
        this.cdr.detectChanges();
      }
    });
  }

  setActiveTab(tab: string) {
    this.activeTab = tab;
  }

  navigateToList(): void {
    this.router.navigate([routes.portal.JobList]);
  }

  getTabContent(): { id: string, title: string, icon: string } {
    return this.tabsContent.find(tab => tab.id === this.activeTab) || this.tabsContent[0];
  }

  confirmApply(): void {
    this.hasApplied = true;
  }

  toggleFavorite(): void {
    this.isFavorite = !this.isFavorite;
  }

  getStatusClass(): string {
    if (!this.job?.jobStatus?.backendName) return this.jobStatus.Closed;

    switch (this.job.jobStatus.backendName) {
      case JobStatus.PendingPointConfiguration:
      case JobStatus.NeedPointUpdate:
        return this.isJobOpen() ? this.jobStatus.PendingPointConfiguration : this.jobStatus.Closed;
      case JobStatus.PendingApproval:
        return this.jobStatus.PendingApproval;
      case JobStatus.Draft:
        return this.jobStatus.Draft;
      default:
        return this.jobStatus.Closed;
    }
  }

  getResponsibilities(): { textAr: string; textEn: string }[] {
    if (!this.job?.responsibilities?.length) return [];
    return this.job.responsibilities.map((c) => ({
      textAr: c.textAr,
      textEn: c.textEn,
    }));
  }

  getJobConditions(): { textAr: string; textEn: string }[] {
    if (!this.job?.conditions?.length) return [];
    return this.job.conditions.map(c => ({
      textAr: c.textAr,
      textEn: c.textEn
    }));
  }

  getJobSkills(): JobSkillResponse[] {
    if (!this.job?.skills?.length) return [];
    return this.job.skills.filter(skill => skill.showToApplicants);
  }

  getJobBenefits(): string[] {
    if (!this.job?.benefitsAr) return [];
    return this.job.benefitsAr
      .split(/[\n,]+/)
      .map((benefit) => benefit.trim())
      .filter(b => b);
  }

  getDegreeRequirements(): string {
    if (!this.job?.degrees?.length) return '';
    const degreeNames = this.job.degrees.map(degree => degree.degree.name);
    return degreeNames.join(',') || '';
  }

  getSpecializationRequirements(): JobSpecializationResponse[] {
    return this.job?.jobSpecializations || [];
  }

  isJobOpen(): boolean {
    if (!this.job?.closingDate) return true;
    const closingDate = new Date(this.job.closingDate);
    const today = new Date();
    return closingDate >= today;
  }

  openViewBasicData() {
    if (!this.job?.id) return;

    this.dialogService.open(JobBasicModalComponent, {
      width: 'min(920px, 96vw)',
      modal: true,
      closable: true,
      closeOnEscape: true,
      dismissableMask: true,
      header: this.translateService.instant('JOB_BASIC_MODAL.TITLE'),
      styleClass: 'custom-bootstrap-dialog',
      draggable: false,
      data: {
        isViewMode: true,
        jobId: this.job.id,
        jobData: this.job
      },
    })?.onClose.subscribe((result) => {
    });
  }
}
