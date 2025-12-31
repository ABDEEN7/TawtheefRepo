import { ChangeDetectorRef, Component, inject, OnInit } from '@angular/core';
import { DialogService } from 'primeng/dynamicdialog';
import { ActivatedRoute, Router } from '@angular/router';
import { JobService } from '../services/job.service';
import { NotificationService } from '../../../core/services/notification.service';
import { ConfirmApplyModalComponent } from '../modals/confirm-apply-modal/confirm-apply-modal.component';
import { GUID } from '../../../shared/types/guid.type';
import { JobLookupService } from '../services/job-lookup.service';
import { JobResponse } from '../models/job-response-model';
import { JobStatus } from '../../../core/enums/lookups.enum';
import { routes } from '../../../routes/routes';
import { JobTabType } from '../enums/job-tab-type';

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
  
  private tabsContent: { id: string, title: string, icon: string}[] = [
    { id: 'Overview', title: 'JOB_DETAILS.OVERVIEW', icon: 'fa-file-alt' },
    { id: 'Skills', title: 'JOB_DETAILS.SKILLS', icon: 'fa-tools' },
    { id: 'Conditions', title: 'JOB_DETAILS.CONDITIONS', icon: 'fa-graduation-cap' },
    { id: 'Benefits', title: 'JOB_DETAILS.BENEFITS', icon: 'fa-gift' },
    { id: 'Responsibilities', title: 'JOB_DETAILS.RESPONSIBILITIES', icon: 'fa-info-circle' },
    { id: 'RequiredAttachments', title: 'JOB_DETAILS.REQUIRED_ATTACHMENTS', icon: 'fa-info-circle' }
  ];

  private cdr = inject(ChangeDetectorRef);
  private jobService = inject(JobService);
  private route = inject(ActivatedRoute);
  lookupsService = inject(JobLookupService);

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

  getTabContent(): { id: string, title: string, icon: string} {
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
    
    switch(this.job.jobStatus.backendName) {
      case JobStatus.Approved:
        return this.isJobOpen() ? this.jobStatus.Approved : this.jobStatus.Closed;
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

  getJobSkills(): string[] {
    if (!this.job?.skills?.length) return [];
    return this.job.skills
      .filter(skill => skill.showToApplicants)
      .map(skill => skill.skill.name); 
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

    const degreeNames = this.job.degrees.map(degree => degree.degree.name)
      

    return degreeNames.join(',') || '';
  }

  isJobOpen(): boolean {
    if (!this.job?.closingDate) return true;
    const closingDate = new Date(this.job.closingDate);
    const today = new Date();
    return closingDate >= today;
  }
}