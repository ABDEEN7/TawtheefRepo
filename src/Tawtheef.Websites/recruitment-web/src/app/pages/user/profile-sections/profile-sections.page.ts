import { CommonModule } from '@angular/common';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { I18nNamespaceDirective } from '../../../shared/directives/i18n-namespace.directive';
import { routes } from '../../../routes/routes';
import {
  ProfileOverview,
  ProfileOverviewSection,
  ProfileRequestProgress,
} from '../profile-overview/models/profile-overview.model';
import { ProfileOverviewService } from '../profile-overview/services/profile-overview.service';
import { WizardProfileModule } from '../wizard-profile/wizard-profile.module';
import { DataService } from '../wizard-profile/services/data.service';
import { AuthService } from '../../../core/auth/auth.service';
import { ProfileLookupsService } from '../wizard-profile/services/profile-lookups.service';
import { PhoneMapperService } from '../wizard-profile/services/phone-mapper.service';
import { mapProfileStatusToState } from '../wizard-profile/services/profile.mapper';
import { UserService } from '../../../core/auth/user.service';
import { NotificationService } from '../../../core/services/notification.service';
import { TranslateService } from '@ngx-translate/core';
import { take, finalize, switchMap } from 'rxjs';

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
  selector: 'app-profile-sections',
  standalone: true,
  imports: [CommonModule, TranslateModule, I18nNamespaceDirective, WizardProfileModule],
  templateUrl: './profile-sections.page.html',
  styleUrl: './profile-sections.page.scss',
})
export class ProfileSectionsPage implements OnInit {
  private api = inject(ProfileOverviewService);
  private ds = inject(DataService);
  private router = inject(Router);
  private auth = inject(AuthService);
  private lookups = inject(ProfileLookupsService);
  private phoneMapper = inject(PhoneMapperService);
  private userService = inject(UserService);
  private notify = inject(NotificationService);
  private translate = inject(TranslateService);

  overview = signal<ProfileOverview | null>(null);
  loading = signal(false);
  error = signal<string | null>(null);
  editingSection = signal<number | null>(null);
  editLoading = signal(false);

  readonly sections = computed(() => this.overview()?.sections ?? []);
  readonly requestProgress = computed<ProfileRequestProgress | null>(
    () => this.overview()?.requestProgress ?? null,
  );

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
        this.error.set('profileSections.error');
        this.loading.set(false);
      },
    });
  }

  sectionName(section: number): string {
    switch (section) {
      case 1:
        return 'profileOverview.sections.prerequisites';
      case 2:
        return 'profileOverview.sections.basicInfo';
      case 3:
        return 'profileOverview.sections.contactInfo';
      case 4:
        return 'profileOverview.sections.qualifications';
      case 5:
        return 'profileOverview.sections.experiences';
      case 6:
        return 'profileOverview.sections.training';
      case 7:
        return 'profileOverview.sections.certificates';
      case 8:
        return 'profileOverview.sections.skills';
      case 9:
        return 'profileOverview.sections.languages';
      case 10:
        return 'profileOverview.sections.attachments';
      default:
        return 'profileOverview.sections.generic';
    }
  }

  sectionStatus(section: ProfileOverviewSection): { label: string; tone: string } {
    if (!section.pendingItems?.length) {
      return { label: 'profileSections.status.clean', tone: 'success' };
    }

    const statuses = section.pendingItems.map(item => item.status);

    if (statuses.some(status => status === ReviewStatus.NeedsCorrection)) {
      return { label: 'profileOverview.status.needsCorrection', tone: 'info' };
    }

    if (statuses.some(status => status === ReviewStatus.Rejected)) {
      return { label: 'profileOverview.status.rejected', tone: 'danger' };
    }

    if (statuses.some(status => status === ReviewStatus.Pending)) {
      return { label: 'profileOverview.status.pending', tone: 'warning' };
    }

    return { label: 'profileOverview.status.notReviewed', tone: 'secondary' };
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

  requestStatusLabel(status?: number | null): string {
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

  editSection(section: number): void {
    if (this.editLoading()) return;

    this.editLoading.set(true);
    this.lookups
      .loadAll()
      .pipe(
        switchMap(() => this.auth.getAuthBootstrap$().pipe(take(1))),
        finalize(() => this.editLoading.set(false)),
      )
      .subscribe({
        next: state => {
          this.ds.prefillFromBootstrap(
            mapProfileStatusToState(this.phoneMapper, this.lookups, state, this.userService.getPrefill()),
          );
          this.editingSection.set(section);
        },
        error: () => {
          this.notify.error(
            `${this.translate.instant('wizard.errorTitle')}: ${this.translate.instant('profileSections.loadError')}`,
          );
        },
      });
  }

  openFullWizard(): void {
    this.router.navigate([routes.user.profileWizard], { state: { allowEdit: true } });
  }

  cancelEdit(): void {
    this.editingSection.set(null);
  }

  finishEdit(): void {
    this.notify.success(
      `${this.translate.instant('wizard.successTitle')}: ${this.translate.instant('profileSections.editComplete')}`,
    );
    this.editingSection.set(null);
    this.load();
  }

  private sectionStep(section: number): number {
    switch (section) {
      case 1:
        return 1;
      case 2:
        return 2;
      case 3:
        return 3;
      case 4:
        return 4;
      case 5:
        return 5;
      case 6:
      case 7:
        return 6;
      case 8:
        return 7;
      case 9:
        return 8;
      case 10:
        return 9;
      default:
        return 1;
    }
  }
}
