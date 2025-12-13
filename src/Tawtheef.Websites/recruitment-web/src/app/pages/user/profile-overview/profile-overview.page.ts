import { CommonModule } from '@angular/common';
import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { ProfileOverviewService } from './services/profile-overview.service';
import { ProfileOverview, ProfileRequestProgress } from './models/profile-overview.model';
import { I18nNamespaceDirective } from '../../../shared/directives/i18n-namespace.directive';

enum ReviewStatus {
  NotReviewed = 0,
  Pending = 1,
  Approved = 2,
  Rejected = 3,
  NeedsCorrection = 4,
  ChangesRequested = 4,
}

enum ProfileStatus {
  InCreation = 0,
  Submitted = 1,
  UnderReview = 2,
  RequiresUpdate = 3,
  Approved = 4,
  Rejected = 5,
  Cancelled = 6,
  AdminCancelled = 7,
}

@Component({
  selector: 'app-profile-overview',
  standalone: true,
  imports: [CommonModule, TranslateModule, I18nNamespaceDirective],
  templateUrl: './profile-overview.page.html',
  styleUrl: './profile-overview.page.scss',
})
export class ProfileOverviewPage implements OnInit {
  private api = inject(ProfileOverviewService);

  overview = signal<ProfileOverview | null>(null);
  loading = signal(false);
  error = signal<string | null>(null);

  readonly hasPending = computed(() => this.overview()?.hasPendingChanges ?? false);
  readonly pendingItems = computed(
    () => this.overview()?.sections?.flatMap(section => section.pendingItems ?? []) ?? [],
  );

  readonly requestProgress = computed<ProfileRequestProgress | null>(() => this.overview()?.requestProgress ?? null);

  readonly pendingCounts = computed(() => {
    const items = this.pendingItems();

    return {
      total: items.length,
      pending: items.filter(item => item.status === ReviewStatus.Pending).length,
      needsCorrection: items.filter(item => item.status === ReviewStatus.NeedsCorrection).length,
      rejected: items.filter(item => item.status === ReviewStatus.Rejected).length,
    };
  });

  readonly overviewStatus = computed(() => this.profileStatusConfig(this.overview()?.status));

  readonly nextStep = computed(() => this.nextStepKey(this.overview()?.status, this.requestProgress()));

  readonly summaryTone = computed<'success' | 'warning' | 'danger' | 'info'>(() => {
    const progress = this.requestProgress();

    if (!progress) return 'info';

    if (progress.requiresUserAction) return 'danger';
    if (this.hasPending()) return 'warning';
    return 'success';
  });

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading.set(true);
    this.error.set(null);
    this.api.getOverview().subscribe({
      next: data => {
        this.overview.set(data);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('profileOverview.error');
        this.loading.set(false);
      },
    });
  }

  profileStatusLabel(status?: number): string {
    switch (status) {
      case ProfileStatus.Submitted:
        return 'profileOverview.profileStatus.submitted';
      case ProfileStatus.UnderReview:
        return 'profileOverview.profileStatus.underReview';
      case ProfileStatus.RequiresUpdate:
        return 'profileOverview.profileStatus.requiresUpdate';
      case ProfileStatus.Approved:
        return 'profileOverview.profileStatus.approved';
      case ProfileStatus.Rejected:
        return 'profileOverview.profileStatus.rejected';
      case ProfileStatus.Cancelled:
        return 'profileOverview.profileStatus.cancelled';
      case ProfileStatus.AdminCancelled:
        return 'profileOverview.profileStatus.adminCancelled';
      default:
        return 'profileOverview.profileStatus.inCreation';
    }
  }

  profileStatusTone(status?: number): 'success' | 'warning' | 'info' | 'danger' {
    switch (status) {
      case ProfileStatus.Approved:
        return 'success';
      case ProfileStatus.RequiresUpdate:
      case ProfileStatus.Submitted:
      case ProfileStatus.UnderReview:
        return 'warning';
      case ProfileStatus.Rejected:
      case ProfileStatus.Cancelled:
      case ProfileStatus.AdminCancelled:
        return 'danger';
      default:
        return 'info';
    }
  }

  profileStatusConfig(status?: number): { label: string; description: string; tone: string } {
    return {
      label: this.profileStatusLabel(status),
      description: this.profileStatusDescription(status),
      tone: this.profileStatusTone(status),
    };
  }

  profileStatusDescription(status?: number): string {
    switch (status) {
      case ProfileStatus.Submitted:
        return 'profileOverview.profileStatusDescriptions.submitted';
      case ProfileStatus.UnderReview:
        return 'profileOverview.profileStatusDescriptions.underReview';
      case ProfileStatus.RequiresUpdate:
        return 'profileOverview.profileStatusDescriptions.requiresUpdate';
      case ProfileStatus.Approved:
        return 'profileOverview.profileStatusDescriptions.approved';
      case ProfileStatus.Rejected:
        return 'profileOverview.profileStatusDescriptions.rejected';
      case ProfileStatus.Cancelled:
        return 'profileOverview.profileStatusDescriptions.cancelled';
      case ProfileStatus.AdminCancelled:
        return 'profileOverview.profileStatusDescriptions.adminCancelled';
      default:
        return 'profileOverview.profileStatusDescriptions.inCreation';
    }
  }

  reviewStatusLabel(status?: number): string {
    switch (status) {
      case ReviewStatus.Pending:
        return 'profileOverview.status.pending';
      case ReviewStatus.Approved:
        return 'profileOverview.status.approved';
      case ReviewStatus.Rejected:
        return 'profileOverview.status.rejected';
      case ReviewStatus.NeedsCorrection:
        return 'profileOverview.status.needsCorrection';
      default:
        return 'profileOverview.status.notReviewed';
    }
  }

  reviewStatusTone(status?: number): 'success' | 'warning' | 'info' | 'danger' {
    switch (status) {
      case ReviewStatus.Approved:
        return 'success';
      case ReviewStatus.Pending:
        return 'warning';
      case ReviewStatus.Rejected:
        return 'danger';
      case ReviewStatus.NeedsCorrection:
        return 'info';
      default:
        return 'info';
    }
  }

  reviewStatusDescription(status?: number): string {
    switch (status) {
      case ReviewStatus.Pending:
        return 'profileOverview.statusDescriptions.pending';
      case ReviewStatus.Approved:
        return 'profileOverview.statusDescriptions.approved';
      case ReviewStatus.Rejected:
        return 'profileOverview.statusDescriptions.rejected';
      case ReviewStatus.NeedsCorrection:
        return 'profileOverview.statusDescriptions.needsCorrection';
      default:
        return 'profileOverview.statusDescriptions.notReviewed';
    }
  }

  sectionName(section: number): string {
    switch (section) {
      case 1:
        return 'profileOverview.sections.basicInfo';
      case 2:
        return 'profileOverview.sections.contactInfo';
      case 3:
        return 'profileOverview.sections.qualifications';
      case 4:
        return 'profileOverview.sections.experiences';
      case 5:
        return 'profileOverview.sections.training';
      case 6:
        return 'profileOverview.sections.certificates';
      case 7:
        return 'profileOverview.sections.skills';
      case 8:
        return 'profileOverview.sections.languages';
      case 9:
        return 'profileOverview.sections.attachments';
      default:
        return 'profileOverview.sections.generic';
    }
  }

  nextStepKey(status?: number, progress?: ProfileRequestProgress | null): string {
    if (progress?.requiresUserAction) {
      return 'profileOverview.actionCenter.respond';
    }

    switch (status) {
      case ProfileStatus.Submitted:
      case ProfileStatus.UnderReview:
        return 'profileOverview.actionCenter.wait';
      case ProfileStatus.RequiresUpdate:
        return 'profileOverview.actionCenter.update';
      case ProfileStatus.Approved:
        return 'profileOverview.actionCenter.done';
      case ProfileStatus.Rejected:
        return 'profileOverview.actionCenter.rejected';
      case ProfileStatus.Cancelled:
      case ProfileStatus.AdminCancelled:
        return 'profileOverview.actionCenter.cancelled';
      default:
        return 'profileOverview.actionCenter.start';
    }
  }
}
