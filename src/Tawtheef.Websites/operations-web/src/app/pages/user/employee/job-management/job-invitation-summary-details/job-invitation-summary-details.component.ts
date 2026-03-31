import { CommonModule } from '@angular/common';
import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { Select } from 'primeng/select';
import { DrawerModule } from 'primeng/drawer';
import { TextareaModule } from 'primeng/textarea';
import { TooltipModule } from 'primeng/tooltip';

import { I18nNamespaceDirective } from '../../../../../shared/directives/i18n-namespace.directive';
import { PaginationComponent } from '../../../../../shared/components/pagination/pagination.component';
import {
  InviteRowVM,
  JobInfoVM,
  JobInvitesStatsVM,
  LookupOption,
  InvitationAttachmentVM
} from '../models/job-invitation-summary-details.model';
import { JobInvitationSummaryDetailsService } from '../services/job-invitation-summary-details.service';
import { GUID } from '../../../../../shared/types/guid.type';
import { GuidUtils } from '../../../../../core/utils/guid-utils';
import { routes } from '../../../../../routes/routes';
import { TableModule } from 'primeng/table';
import { PaginationMetadata } from '../../../../../core/models/pagination-metadata.model';
import { FaDirArrowDirective } from '../../../../../shared/directives/dir-arrow.directive';
import { LanguageService } from '../../../../../core/services/language.service';


@Component({
  selector: 'app-job-invitation-summary-details',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    RouterModule,
    TranslatePipe,
    I18nNamespaceDirective,
    Select,
    PaginationComponent,
    TableModule,
    FaDirArrowDirective,
    DrawerModule,
    TextareaModule,
    TooltipModule
  ],
  templateUrl: './job-invitation-summary-details.component.html',
  styleUrl: './job-invitation-summary-details.component.scss',
})
export class JobInvitationSummaryDetailsComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  detailsService = inject(JobInvitationSummaryDetailsService);
  lang = inject(LanguageService);

  jobId = signal<GUID>(GuidUtils.emptyGuid);

  selectedStatus = signal<string | null>(null);
  searchText = signal<string>('');
  batchNumber = signal<string>('');
  currentPage = signal(1);
  itemsPerPage = signal(7);

  jobInfo = signal<JobInfoVM | null>(null);
  stats = signal<JobInvitesStatsVM | null>(null);
  rows = signal<InviteRowVM[]>([]);
  paginationMetadata = signal<PaginationMetadata | null>(null);

  totalItems = computed(() => this.paginationMetadata()?.totalCount ?? 0);

  statusOptions = signal<LookupOption[]>([]);

  // Attachment State
  showAttachments = signal(false);
  selectedInvitationId = signal<GUID>(GuidUtils.emptyGuid);
  attachments = signal<InvitationAttachmentVM[]>([]);
  isLoadingAttachments = signal(false);

  showPreview = signal(false);
  previewUrl = signal<SafeResourceUrl | null>(null);
  previewTitle = signal('');
  private currentBlobUrl: string | null = null;

  private sanitizer = inject(DomSanitizer);

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('jobId') ?? '';
    this.jobId.set(id as GUID);

    this.detailsService.loadLookups().subscribe({
      next: (response) => {
        this.statusOptions.set(response);
      }
    });
    this.loadAll();
  }

  loadAll(): void {
    this.detailsService.getJobInvites(this.jobId()).subscribe((res) => this.jobInfo.set(res));

    this.detailsService.getStats({
      jobId: this.jobId(),
    }).subscribe((res) => this.stats.set(res));


    this.detailsService.getRows({
      jobId: this.jobId(),
      statusId: this.selectedStatus() || null,
      search: this.searchText() || '',
      batchNumber: this.getValidBatchNumber(),
      pageNumber: this.currentPage(),
      pageSize: this.itemsPerPage(),
      sortBy: 'createdDate',
      sortDirection: 'desc',
    }).subscribe((res) => {
      this.rows.set(res.items ?? []);
      this.paginationMetadata.set(res.metadata);
    });
  }

  viewAttachments(invitationId: string): void {
    this.selectedInvitationId.set(invitationId as GUID);
    this.showAttachments.set(true);
    this.loadAttachments();
  }

  previewAttachment(attachment: InvitationAttachmentVM): void {
    if (!attachment.resourceUrl) return;
    this.onPreviewHide();
    this.showPreview.set(true);
    this.previewTitle.set(attachment.resourceName);
    this.previewUrl.set(null);
    this.detailsService.getAttachmentBlob(attachment.resourceUrl).subscribe({
      next: (blob) => {
        this.currentBlobUrl = URL.createObjectURL(blob);
        this.previewUrl.set(this.sanitizer.bypassSecurityTrustResourceUrl(this.currentBlobUrl));
      },
      error: (err) => {
        console.error('File preview failed', err);
        this.showPreview.set(false);
      }
    });
  }

  onPreviewHide(): void {
    if (this.currentBlobUrl) {
      URL.revokeObjectURL(this.currentBlobUrl);
      this.currentBlobUrl = null;
    }
    this.previewUrl.set(null);
  }

  loadAttachments(): void {
    this.isLoadingAttachments.set(true);
    this.detailsService.getInvitationAttachments(this.selectedInvitationId()).subscribe({
      next: (res) => {
        this.attachments.set(res);
        this.isLoadingAttachments.set(false);
      },
      error: () => this.isLoadingAttachments.set(false)
    });
  }

  approveAttachment(attachmentId: string): void {
    this.detailsService.reviewAttachment({
      invitationId: this.selectedInvitationId(),
      attachmentId: attachmentId as GUID,
      isApproved: true
    }).subscribe(() => {
      this.loadAttachments();
      this.loadAll();
    });
  }

  returnAttachment(attachmentId: string, note: string): void {
    if (!note) return;
    this.detailsService.reviewAttachment({
      invitationId: this.selectedInvitationId(),
      attachmentId: attachmentId as GUID,
      isApproved: false,
      reviewNote: note
    }).subscribe(() => {
      this.loadAttachments();
      this.loadAll();
    });
  }

  onFilterChange(): void {
    this.currentPage.set(1);
    this.loadAll();
  }

  clearFilters(): void {
    this.selectedStatus.set(null);
    this.searchText.set('');
    this.batchNumber.set('');
    this.currentPage.set(1);
    this.loadAll();
  }

  onPageChange(page: number): void {
    this.currentPage.set(page);
    this.loadAll();
  }

  onPageSizeChange(size: number): void {
    this.itemsPerPage.set(size);
    this.currentPage.set(1);
    this.loadAll();
  }

  // status pills classes (adjust backend names to your API)
  getStatusPillClass(backendName: string): string {
    const map: Record<string, string> = {
      Applied: 'pill success',
      Submitted: 'pill success',
      NewInvitation: 'pill info',
      PendingAttachmentApproval: 'pill warning',
      ReturnedAttachment: 'pill danger',
      Cancelled: 'pill danger',
      Declined: 'pill warning',
      Refused: 'pill warning',
    };

    return map[backendName] ?? 'pill neutral';
  }

  navigateTo() {
    this.router.navigate([routes.portal.jobInvitationSummary]);
  }

  private getValidBatchNumber(): string | null {
    const value = this.batchNumber().trim();
    return value && GuidUtils.isValid(value) ? value : null;
  }
}
