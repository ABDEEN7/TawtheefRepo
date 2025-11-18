import { ChangeDetectorRef, Component, inject, OnInit } from '@angular/core';
import { DialogService } from 'primeng/dynamicdialog';
import { ActivatedRoute } from '@angular/router';
import {Job} from '../models/job.model';
import {JobService} from '../services/job.service';
import {NotificationService} from '../../../core/services/notification.service';
import {ConfirmApplyModalComponent} from '../modals/confirm-apply-modal/confirm-apply-modal.component';
import { GUID } from '../../../shared/types/guid.type';

@Component({
  selector: 'app-job-details',
  templateUrl: './job-details.component.html',
  styleUrls: ['./job-details.component.scss'],
  standalone: false,
})
export class JobDetailsComponent implements OnInit {
  job: Job | undefined;
  id!: GUID;

  activeTab: string = 'desc';
  hasApplied: boolean = false;

  private cdr = inject(ChangeDetectorRef);
  private jobService = inject(JobService);
  private route = inject(ActivatedRoute);
  private dialogService = inject(DialogService);
  private notificationService = inject(NotificationService);
  ngOnInit() {
    this.id = this.route.snapshot.paramMap.get('id') as GUID;
    this.loadJobById();
  }

  private loadJobById(): void {
    this.jobService.loadJob(this.id).subscribe({
      next: (job) => {
        this.job = job;
        this.cdr.detectChanges();
      },
      error: (error) => {
        this.notificationService.error(error);
        this.cdr.detectChanges();
      },
    });
  }

  setActiveTab(tab: string) {
    this.activeTab = tab;
    this.cdr.detectChanges();
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
        this.notificationService.success('applied_successfully');
      }
    });
  }

  confirmApply(): void {
    this.hasApplied = true;
  }

  getResponsibilities(): string[] {
    return [
      'إعداد خطط دروس تراعي الفروق الفردية',
      'استخدام أدوات قياس متنوعة لقياس نواتج التعلم',
      'التعاون مع الأسرة والاختصاصيين التربويين',
      'الالتزام بمعايير الجودة والسلامة',
    ];
  }

  getJobBenefits(): string[] {
    if (!this.job?.benefits) return [];
    return this.job.benefits.split(',').map((benefit) => benefit.trim());
  }
}
