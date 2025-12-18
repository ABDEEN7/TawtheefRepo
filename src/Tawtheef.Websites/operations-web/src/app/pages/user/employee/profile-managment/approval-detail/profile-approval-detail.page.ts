import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { finalize, Subscription, combineLatest } from 'rxjs';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import {
  ProfileApprovalStepperComponent,
  ProfileApprovalStepperSection,
  StepUiStatus,
} from './components/profile-approval-stepper/profile-approval-stepper.component';
import { BasicInfoSectionComponent } from './components/sections/basic-info-section/basic-info-section.component';
import { QualificationsSectionComponent } from './components/sections/qualifications-section/qualifications-section.component';
import { ExperiencesSectionComponent } from './components/sections/experiences-section/experiences-section.component';
import { TrainingSectionComponent } from './components/sections/training-section/training-section.component';
import { AttachmentsSectionComponent } from './components/sections/attachments-section/attachments-section.component';
import {Select} from 'primeng/select';
import {Textarea} from 'primeng/textarea';
import { CertificatesSectionComponent } from './components/sections/certificates-section/certificates-section.component';
import {
  ProfileApprovalDetail,
  ProfileApprovalItem, ProfileApprovalSection,
  ReviewStatus
} from '../approval-list/models/profile-approval.models';
import {routes} from '../../../../../routes/routes';
import {ProfileApprovalService} from '../approval-list/services/profile-approval.service';
import {I18nNamespaceDirective} from '../../../../../shared/directives/i18n-namespace.directive';
import {ProgressSpinnerModule} from 'primeng/progressspinner';
import {CardModule} from 'primeng/card';
import {ButtonModule} from 'primeng/button';
import {AvatarModule} from 'primeng/avatar';
import {DialogService} from 'primeng/dynamicdialog';
import {MessageService} from 'primeng/api';
import {FileUtilsService} from '../../../../../core/utils/file-utils';
import {ContactInfoSectionComponent} from './components/sections/contact-info-section/contact-info-section.component';
import {FirstInfoSectionComponent} from './components/sections/first-info-section/first-info-section.component';
import {FinalReviewSection} from './components/sections/final-review-section/final-review-section';
import {SkillsSectionComponent} from './components/sections/skills-section/skills-section.component';
import {LanguagesSectionComponent} from './components/sections/languages-section/languages-section.component';
import {LanguageService} from '../../../../../core/services/language.service';
import {NotificationService} from '../../../../../core/services/notification.service';
import {FaDirArrowDirective} from '../../../../../shared/directives/dir-arrow.directive';

@Component({
  selector: 'app-profile-approval-detail-page',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    RouterModule,
    TranslateModule,
    I18nNamespaceDirective,
    ProgressSpinnerModule,
    CardModule,
    ButtonModule,
    AvatarModule,
    ProfileApprovalStepperComponent,
    BasicInfoSectionComponent,
    QualificationsSectionComponent,
    ExperiencesSectionComponent,
    TrainingSectionComponent,
    CertificatesSectionComponent,
    SkillsSectionComponent,
    LanguagesSectionComponent,
    AttachmentsSectionComponent,
    Select,
    Textarea,
    ContactInfoSectionComponent,
    FirstInfoSectionComponent,
    FinalReviewSection,
    FaDirArrowDirective,
  ],
  templateUrl: './profile-approval-detail.page.html',
  styleUrl: './profile-approval-detail.page.scss',
  providers: [DialogService, MessageService],
})
export class ProfileApprovalDetailPage implements OnInit, OnDestroy {
  private fileUtils = inject(FileUtilsService);
  private api = inject(ProfileApprovalService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private translate = inject(TranslateService);
  private notifications = inject(NotificationService);
  private language = inject(LanguageService);

  private subscriptions: Subscription[] = [];
  private lastLoadedKey: string | null = null;
  private readonly flowSections = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11];

  loadingDetail = signal(false);
  selectedProfileId = signal<string | null>(null);
  detail = signal<ProfileApprovalDetail | null>(null);
  error = signal<string | null>(null);
  partialMode = signal(false);
  activeSection = signal<number | null>(null);
  currentLang = signal(this.language.get());
  isRtl = computed(() => this.currentLang() === 'ar');
  isChangesMode = computed(() => this.partialMode());
  reviewStatusOptions = [
    { labelKey: 'profileApproval.status.approved', value: ReviewStatus.Approved },
    { labelKey: 'profileApproval.status.changes', value: ReviewStatus.ChangesRequested },
    { labelKey: 'profileApproval.status.needsCorrection', value: ReviewStatus.NeedsCorrection },
    { labelKey: 'profileApproval.status.rejected', value: ReviewStatus.Rejected },
  ];
  ngOnInit(): void {
    const langSub = this.language.current$.subscribe(lang => this.currentLang.set(lang));
    this.subscriptions.push(langSub);

    const sub = combineLatest([this.route.paramMap, this.route.queryParamMap]).subscribe(([params, query]) => {
      const profileId = params.get('profileId');
      if (!profileId) {
        this.backToList();
        return;
      }

      this.selectedProfileId.set(profileId);
      const loadKey = `${profileId}`;
      if (loadKey !== this.lastLoadedKey) {
        this.lastLoadedKey = loadKey;
        this.loadDetail();
      } else if (!this.detail()) {
        this.loadDetail();
      }
    });

    this.subscriptions.push(sub);
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(s => s.unsubscribe());
  }

  toggleChangesMode(): void {
    const profileId = this.selectedProfileId();
    if (!profileId) return;

    const next = !this.partialMode();
    this.router.navigate(['./'], {
      relativeTo: this.route,
      queryParams: { changes: next ? 1 : null },
      queryParamsHandling: 'merge',
    });
  }
  backToList(): void {
    this.partialMode.set(false);
    this.selectedProfileId.set(null);
    this.detail.set(null);
    this.router.navigate([routes.employee.approvalProfile]);
  }

  loadDetail(): void {
    const profileId = this.selectedProfileId();

    if (!profileId) return;
    this.loadingDetail.set(true);
    this.error.set(null);
    const loader = this.partialMode()
      ? this.api.getProfileChanges(profileId)
      : this.api.getProfile(profileId);

    loader
      .pipe(
        finalize(() => {
          this.loadingDetail.set(false);
        })
      )
      .subscribe({
        next: detail => {
          if(!detail.approvedProfile)
            this.partialMode.set(false);
          const ordered = { ...detail, sections: this.sortSections(detail.sections) };
          const merged = this.mergeDetail(this.detail(), ordered);
          this.detail.set(merged);
          const firstSection = this.flowSections[0] ?? null;
          if (firstSection !== null) this.activeSection.set(firstSection);
        },
        error: () => this.error.set(this.translate.instant('profileApproval.errors.loadDetail')),
      });
  }


  previewFile(resourceUrl: string): void {
    this.fileUtils.previewUrl(resourceUrl, '', false).then(() => {});
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
        return 'profileOverview.sections.finalReview';
    }
  }

  sectionItems(section: number): ProfileApprovalItem[] {
    const detail = this.detail();
    if (!detail?.sections?.length) return [];

    return detail.sections.find(s => s.section === section)?.items ?? [];
  }

  private parsePartialFlag(value: string | null): boolean {
    if (!value) return false;
    return value === '1' || value.toLowerCase() === 'true';
  }

  private patchReviewedItem(reviewItemId: string, status: ReviewStatus, note?: string): void {
    const current = this.detail();
    if (!current?.sections?.length) return;

    const nextSections = current.sections.map(sec => {
      const nextItems = (sec.items ?? []).map(it => {
        if (it.reviewItemId !== reviewItemId) return it;

        return {
          ...it,
          status,
          note: note ?? it.note ?? null,
        };
      });

      return { ...sec, items: nextItems };
    });

    // Update detail immutably so Angular change detection + signals pick it up reliably.
    this.detail.set({
      ...current,
      sections: nextSections,
    });
  }

  updateItem(event: {reviewItemId: string, status: ReviewStatus, note?: string}): void {
    if (!this.selectedProfileId()) return;
    this.loadingDetail.set(true);
    this.api
      .reviewItem(event.reviewItemId, event.status, event.note)
      .pipe(
        finalize(() => {
          this.loadingDetail.set(false);
        })
      )
      .subscribe({
        next: () => {
          this.patchReviewedItem(event.reviewItemId, event.status, event.note);
          this.notifications.success(this.translate.instant('profileApproval.detail.itemSaved'));
        },
        error: err => {
          const message = err?.error?.[0]?.message ?? this.translate.instant('profileApproval.errors.reviewItem');
          this.notifications.error(message);
          this.error.set(message);
        },
      });
  }


  private normalizeSections(incoming: ProfileApprovalDetail): ProfileApprovalDetail {
    const map = new Map<number, ProfileApprovalSection>();
    (incoming.sections ?? []).forEach(s => map.set(s.section, s));

    const normalized: ProfileApprovalSection[] = this.flowSections.map(section => {
      const existing = map.get(section);
      if (existing) return existing;

      // Create empty section placeholder
      return {
        section,
        sectionReview: null,
        items: [],
        hasAttachments: false
      } as any;
    });

    return { ...incoming, sections: normalized };
  }

  private mergeDetail(current: ProfileApprovalDetail | null, incoming: ProfileApprovalDetail): ProfileApprovalDetail {
    const normalizedIncoming = this.normalizeSections(incoming);

    if (!current) return normalizedIncoming;

    return {
      ...current,
      ...normalizedIncoming,
      profile: normalizedIncoming.profile ?? current.profile,
      approvedProfile: normalizedIncoming.approvedProfile ?? current.approvedProfile,
      sections: normalizedIncoming.sections?.length ? normalizedIncoming.sections : current.sections ?? [],
    };
  }


  stepperSections(info: ProfileApprovalDetail): ProfileApprovalStepperSection[] {
    const current = this.activeSection();

    const sections = this.sortSections(info.sections);

    const sectionSteps = sections.map(s => {
      const st = s.sectionReview?.status;

      let uiStatus: StepUiStatus = 'idle';
      if (st === ReviewStatus.Approved) uiStatus = 'done';
      else if (st === ReviewStatus.Rejected) uiStatus = 'bad';
      else if (s.section === current) uiStatus = 'progress';
      else uiStatus = 'idle';

      return {
        section: s.section,
        uiStatus,
        icon: this.sectionIcon(s.section),
        labelKey: this.sectionName(s.section),
      };
    });
    return sectionSteps;
  }

  orderedSections(info: ProfileApprovalDetail): ProfileApprovalSection[] {
    return this.sortSections(info.sections);
  }

  private sortSections(sections: ProfileApprovalSection[] | null | undefined): ProfileApprovalSection[] {
    if (!sections?.length) return [];
    return [...sections].sort((a, b) => a.section - b.section);
  }

  sectionIcon(section: number): string {
    switch (section) {
      case 1:  return 'pi pi-verified';        // Basic identity
      case 2:  return 'pi pi-id-card';        // Identity (duplicate case)
      case 3:  return 'pi pi-address-book';   // Address / contact
      case 4:  return 'pi pi-graduation-cap'; // Education
      case 5:  return 'pi pi-briefcase';      // Experience / work
      case 6:  return 'pi pi-folder-open';    // Documents / files
      case 7:  return 'pi pi-list';           // Lists / records
      case 8:  return 'pi pi-star';           // Skills (best available semantic match)
      case 9:  return 'pi pi-language';       // Languages
      case 10: return 'pi pi-paperclip';      // Attachments
      default: return 'pi pi-clipboard';             // Review / approval
    }
  }

  onSectionStatusChange(sec: ProfileApprovalSection, status: ReviewStatus | null) {
    if (!sec.sectionReview) return;
    sec.sectionReview.status = status ?? ReviewStatus.Pending;
  }

  onSectionNoteChange(sec: ProfileApprovalSection, note: string) {
    if (!sec.sectionReview) return;
    sec.sectionReview.note = note;
  }

  goPrev(info: ProfileApprovalDetail) {
    const idx = info.sections.findIndex(s => s.section === this.activeSection());
    if (idx <= 0) return;
    this.activeSection.set(info.sections[idx - 1].section);
  }

  goNext(info: ProfileApprovalDetail) {
    const idx = info.sections.findIndex(s => s.section === this.activeSection());
    if (idx < 0 || idx >= info.sections.length - 1) return;
    this.activeSection.set(info.sections[idx + 1].section);
  }

  progressStats(info: ProfileApprovalDetail): {
    pendingSections: number;
    flaggedSections: number;
    approvedSections: number;
    pendingItems: number;
  } {
    const stats = { pendingSections: 0, flaggedSections: 0, approvedSections: 0, pendingItems: 0 };
    const sections = info.sections ?? [];

    sections.forEach(sec => {
      const status = sec.sectionReview?.status ?? ReviewStatus.Pending;

      if (status === ReviewStatus.Approved) stats.approvedSections += 1;
      else if (status === ReviewStatus.Rejected || status === ReviewStatus.ChangesRequested || status === ReviewStatus.NeedsCorrection)
        stats.flaggedSections += 1;
      else stats.pendingSections += 1;

      (sec.items ?? []).forEach(item => {
        if (
          item.status === ReviewStatus.Pending ||
          item.status === ReviewStatus.NeedsCorrection ||
          item.status === ReviewStatus.ChangesRequested
        ) {
          stats.pendingItems += 1;
        }
      });
    });

    return stats;
  }

  firstAttentionSection(info: ProfileApprovalDetail): number | null {
    const sections = this.sortSections(info.sections);
    const target = sections.find(sec => {
      const hasPendingItem = (sec.items ?? []).some(
        i =>
          i.status === ReviewStatus.Pending ||
          i.status === ReviewStatus.ChangesRequested ||
          i.status === ReviewStatus.NeedsCorrection ||
          i.status === ReviewStatus.Rejected
      );
      const status = sec.sectionReview?.status ?? ReviewStatus.Pending;
      const needsAttention =
        hasPendingItem ||
        status === ReviewStatus.Pending ||
        status === ReviewStatus.ChangesRequested ||
        status === ReviewStatus.NeedsCorrection ||
        status === ReviewStatus.Rejected;

      return needsAttention;
    });

    return target?.section ?? null;
  }

  jumpToAttention(info: ProfileApprovalDetail): void {
    const target = this.firstAttentionSection(info);
    if (target !== null) this.activeSection.set(target);
  }
}
