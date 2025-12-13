import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { finalize, Subscription, combineLatest } from 'rxjs';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { ProfileApprovalService } from '../profile-approval/services/profile-approval.service';
import {
  FinalApprovalAction,
  ProfileApprovalDetail,
  ProfileApprovalItem,
  ProfileApprovalSection,
  ReviewStatus,
  ReviewTargetType,
} from '../profile-approval/models/profile-approval.models';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { AvatarModule } from 'primeng/avatar';
import { DialogService } from 'primeng/dynamicdialog';
import { MessageService } from 'primeng/api';
import { ItemDialogResult, ItemReviewDialogComponent } from '../profile-approval/dialogs/item-review-dialog/item-review-dialog';
import { SectionDialogResult, SectionReviewDialogComponent } from '../profile-approval/dialogs/section-review-dialog/section-review-dialog';
import { FinalActionConfirmDialogComponent } from '../profile-approval/dialogs/final-action-confirm-dialog/final-action-confirm-dialog';
import { FileUtilsService } from '../../../core/utils/file-utils';
import { I18nNamespaceDirective } from '../../../shared/directives/i18n-namespace.directive';
import { routes } from '../../../routes/routes';
import {
  ProfileApprovalStepperComponent,
  ProfileApprovalStepperSection,
  StepUiStatus,
} from './components/profile-approval-stepper/profile-approval-stepper.component';
import { BasicInfoSectionComponent } from './components/sections/basic-info-section/basic-info-section.component';
import { QualificationsSectionComponent } from './components/sections/qualifications-section/qualifications-section.component';
import { ExperiencesSectionComponent } from './components/sections/experiences-section/experiences-section.component';
import { TrainingSectionComponent } from './components/sections/training-section/training-section.component';
import { SkillsLanguagesSectionComponent } from './components/sections/skills-languages-section/skills-languages-section.component';
import { AttachmentsSectionComponent } from './components/sections/attachments-section/attachments-section.component';
import { PhotoSectionComponent } from './components/sections/photo-section/photo-section.component';
import {Select} from 'primeng/select';
import {Textarea} from 'primeng/textarea';

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
    SkillsLanguagesSectionComponent,
    AttachmentsSectionComponent,
    PhotoSectionComponent,
    Select,
    Textarea,
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
  private dialogService = inject(DialogService);
  private messages = inject(MessageService);

  private subscriptions: Subscription[] = [];
  private lastLoadedKey: string | null = null;
  private readonly flowSections = [1, 2, 3, 4, 5, 6, 7, 9];

  loadingDetail = signal(false);
  loadingFinalAction = signal(false);
  selectedProfileId = signal<string | null>(null);
  detail = signal<ProfileApprovalDetail | null>(null);
  error = signal<string | null>(null);
  partialMode = signal(false);
  activeSection = signal<number | null>(null);
  activeFinalAction = signal<FinalApprovalAction | null>(null);
  finalSummary = signal('');
  finalActionNote = signal('');
  finalAttachment: File | null = null;
  reviewStatusOptions = [
    { labelKey: 'profileApproval.status.approved', value: ReviewStatus.Approved },
    { labelKey: 'profileApproval.status.changes', value: ReviewStatus.ChangesRequested },
    { labelKey: 'profileApproval.status.rejected', value: ReviewStatus.Rejected },
  ];

  protected readonly ReviewStatus = ReviewStatus;
  protected readonly ReviewTargetType = ReviewTargetType;

  ngOnInit(): void {
    const sub = combineLatest([this.route.paramMap, this.route.queryParamMap]).subscribe(([params, query]) => {
      const profileId = params.get('profileId');
      const partial = this.parsePartialFlag(query.get('changes'));
      this.partialMode.set(partial);

      if (!profileId) {
        this.backToList();
        return;
      }

      this.selectedProfileId.set(profileId);
      const loadKey = `${profileId}-${partial}`;
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
          const ordered = { ...detail, sections: this.sortSections(detail.sections) };
          const merged = this.mergeDetail(this.detail(), ordered);
          this.detail.set(merged);
          const firstSection = this.flowSections[0] ?? null;
          if (firstSection !== null) this.activeSection.set(firstSection);
        },
        error: () => this.error.set(this.translate.instant('profileApproval.errors.loadDetail')),
      });
  }

  openItemDialog(item: ProfileApprovalItem, action: 'approve' | 'reject' | 'changes'): void {
    this.dialogService.open(ItemReviewDialogComponent, {
      header: this.translate.instant('profileApproval.dialog.title'),
      width: '420px',
      data: { item, action },
    })?.onClose.subscribe((result?: ItemDialogResult) => {
      if (!result) return;

      if (result.action === 'approve') {
        this.updateItem(item.reviewItemId, ReviewStatus.Approved);
      } else if (result.action === 'reject') {
        this.updateItem(item.reviewItemId, ReviewStatus.Rejected, result.note);
      } else {
        this.updateItem(item.reviewItemId, ReviewStatus.ChangesRequested, result.note);
      }
    });
  }

  openSectionDialog(section: ProfileApprovalSection, action: 'section-approve' | 'section-changes'): void {
    this.dialogService.open(SectionReviewDialogComponent, {
      header: this.translate.instant('profileApproval.section.dialogTitle'),
      width: '480px',
      data: {
        section,
        action,
        sectionLabelKey: this.sectionName(section.section),
        initialNote: section.sectionReview?.note ?? '',
      },
    })?.onClose.subscribe((result?: SectionDialogResult) => {
      if (!result || !section.sectionReview) return;

      if (result.action === 'section-approve') {
        this.updateItem(section.sectionReview.reviewItemId, ReviewStatus.Approved, result.note);
      } else {
        this.updateItem(section.sectionReview.reviewItemId, ReviewStatus.ChangesRequested, result.note);
      }
    });
  }


  previewFile(resourceUrl: string): void {
    this.fileUtils.previewUrl(resourceUrl, '', false).then(() => {});
  }

  tabHasPending(section: ProfileApprovalSection): boolean {
    const pendingItem = section.items.some(i => i.status === ReviewStatus.Pending || i.status === ReviewStatus.ChangesRequested || i.status === ReviewStatus.Rejected);
    return pendingItem || (section.sectionReview?.status === ReviewStatus.ChangesRequested || section.sectionReview?.status === ReviewStatus.Rejected || section.sectionReview?.status === ReviewStatus.Pending);
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
        return ''
    }
  }

  private collectNeedsCorrectionTargets(): string[] {
    const detail = this.detail();
    if (!detail) return [];

    const targets = new Set<string>();
    detail.sections.forEach(section => {
      if (section.sectionReview && section.sectionReview.status === ReviewStatus.ChangesRequested)
        targets.add(section.sectionReview.reviewItemId);

      section.items
        .filter(item => item.status === ReviewStatus.ChangesRequested)
        .forEach(item => targets.add(item.reviewItemId));
    });

    return Array.from(targets);
  }

  private parsePartialFlag(value: string | null): boolean {
    if (!value) return false;
    return value === '1' || value.toLowerCase() === 'true';
  }

  private updateItem(reviewItemId: string, status: ReviewStatus, note?: string): void {
    if (!this.selectedProfileId()) return;
    this.loadingDetail.set(true);
    this.api
      .reviewItem(reviewItemId, status, note)
      .pipe(finalize(() => this.loadingDetail.set(false)))
      .subscribe({
        next: () => {
          this.loadDetail();
        },
        error: err => {
          const message = err?.error?.[0]?.message ?? this.translate.instant('profileApproval.errors.reviewItem');
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

      // قسم غير موجود من الـ API -> أنشئ قالب فارغ
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

    return sections.map(s => {
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
      case 1: return 'fa-regular fa-id-card';
      case 2: return 'fa-regular fa-address-book';
      case 3: return 'fa-solid fa-graduation-cap';
      case 4: return 'fa-solid fa-briefcase';
      case 5: return 'fa-solid fa-language';
      case 6: return 'fa-regular fa-folder-open';
      case 7: return 'fa-regular fa-rectangle-list';
      case 9: return 'fa-regular fa-image';
      default: return 'fa-regular fa-circle';
    }
  }

  /** تخزين مبدئي محلي (UI) ثم إرسال مراجعة القسم عند الضغط التالي/السابق أو حسب رغبتك */
  onSectionStatusChange(sec: ProfileApprovalSection, status: ReviewStatus | null) {
    if (!sec.sectionReview) return;

    // تحديث UI محلياً
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
}
