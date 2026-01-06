import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { rxResource } from '@angular/core/rxjs-interop';
import { ButtonDirective } from 'primeng/button';
import { Tag } from 'primeng/tag';
import { Skeleton } from 'primeng/skeleton';
import { TooltipModule } from 'primeng/tooltip';
import { DialogService } from 'primeng/dynamicdialog';

import { routes } from '../../../../routes/routes';
import {
  ProfileSectionEnum,
  ProfileChangeRequestDto,
  ProfileChangeRequestStatusEnum,
  ReviewStatusEnum,
  MyProfileReviewSummaryDto
} from '../overview/models/profile-overview.model';
import { FileUtilsService } from '../../../../core/utils/file-utils';
import { ProfileOverviewService } from '../overview/services/profile-overview.service';
import { I18nNamespaceDirective } from '../../../../shared/directives/i18n-namespace.directive';
import { ReviewStepsDialogComponent } from '../overview/dialogs/review-steps-dialog/review-steps-dialog.component';
import { ReviewItemEditDialogComponent } from '../overview/dialogs/review-item-edit-dialog/review-item-edit-dialog.component';
import { ProfileStatusDto } from '../../../../core/models/auth/auth-response.model';
import { ProfilePrerequisitesSectionComponent } from './sections/prerequisites/prerequisites-section.component';
import { ProfilePersonalSectionComponent } from './sections/personal/personal-section.component';
import { ProfileContactSectionComponent } from './sections/contact/contact-section.component';
import { ProfileQualificationsSectionComponent } from './sections/qualifications/qualifications-section.component';
import { ProfileExperienceSectionComponent } from './sections/experience/experience-section.component';
import { ProfileTrainingSectionComponent } from './sections/training/training-section.component';
import { ProfileAchievementsSectionComponent } from './sections/achievements/achievements-section.component';
import { ProfileSkillsSectionComponent } from './sections/skills/skills-section.component';
import { ProfileLanguagesSectionComponent } from './sections/languages/languages-section.component';
import { ProfileAttachmentsSectionComponent } from './sections/attachments/attachments-section.component';
import { ProfileViewCqrs } from './profile-view.cqrs';
import {FaDirArrowDirective} from '../../../../shared/directives/dir-arrow.directive';

interface SectionCard {
  section: ProfileSectionEnum;
  icon: string;
  labelKey: string;
}
// Base (non-generic) rxResource instance type
type AnyRxRes = ReturnType<typeof rxResource>;
// Generic wrapper: same shape, but value() is strongly typed
type RxRes<T> = Omit<AnyRxRes, 'value'> & { value: () => T | undefined };
@Component({
  selector: 'app-profile-view-page',
  standalone: true,
  imports: [
    CommonModule,
    TranslatePipe,
    Skeleton,
    TooltipModule,
    I18nNamespaceDirective,
    ProfilePrerequisitesSectionComponent,
    ProfilePersonalSectionComponent,
    ProfileContactSectionComponent,
    ProfileQualificationsSectionComponent,
    ProfileExperienceSectionComponent,
    ProfileTrainingSectionComponent,
    ProfileAchievementsSectionComponent,
    ProfileSkillsSectionComponent,
    ProfileLanguagesSectionComponent,
    ProfileAttachmentsSectionComponent,
    FaDirArrowDirective
  ],
  templateUrl: './profile-view.page.html',
  styleUrls: ['./profile-view.page.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [DialogService]
})
export class ProfileViewPage {
  private readonly fileUtils = inject(FileUtilsService);
  private readonly router = inject(Router);
  private readonly i18n = inject(TranslateService);
  private readonly overviewService = inject(ProfileOverviewService);
  private readonly profileCqrs = inject(ProfileViewCqrs);
  private readonly dialogService = inject(DialogService);
  protected readonly ProfileSectionEnum = ProfileSectionEnum;

  protected readonly cards: SectionCard[] = [
    { section: ProfileSectionEnum.Prerequisites, icon: 'pi pi-file', labelKey: 'profileOverview.sections.prerequisites' },
    { section: ProfileSectionEnum.Personal, icon: 'pi pi-id-card', labelKey: 'profileOverview.sections.personal' },
    { section: ProfileSectionEnum.Contact, icon: 'pi pi-map-marker', labelKey: 'profileOverview.sections.contact' },
    { section: ProfileSectionEnum.Qualifications, icon: 'pi pi-graduation-cap', labelKey: 'profileOverview.sections.qualifications' },
    { section: ProfileSectionEnum.Experience, icon: 'pi pi-briefcase', labelKey: 'profileOverview.sections.experiences' },
    { section: ProfileSectionEnum.TrainingCourses, icon: 'pi pi-book', labelKey: 'profileOverview.sections.trainingCourses' },
    { section: ProfileSectionEnum.CertificatesAndAwards, icon: 'pi pi-star', labelKey: 'profileOverview.sections.certificatesAndAwards' },
    { section: ProfileSectionEnum.Skills, icon: 'pi pi-bolt', labelKey: 'profileOverview.sections.skills' },
    { section: ProfileSectionEnum.Languages, icon: 'pi pi-language', labelKey: 'profileOverview.sections.languages' },
    { section: ProfileSectionEnum.Attachments, icon: 'pi pi-paperclip', labelKey: 'profileOverview.sections.attachments' }
  ];
  get keyLabel(){
    return this.cards.find(c => c.section === this.expanded())?.labelKey;
  }

  private readonly basics = rxResource({
    params: () => true,
    stream: () => this.profileCqrs.basics()
  });

  private readonly review = rxResource({
    params: () => true,
    stream: () => this.overviewService.getMyProfileReviewSummary(),
  });

  private readonly changeRequests = rxResource({
    params: () => true,
    stream: () => this.overviewService.getMyChangeRequests(),
  });

  // FIX: correct rxResource typing inside Map.
  // If loadSection returns a different DTO per section, make this RxRes<unknown> instead.
  private readonly sections = new Map<ProfileSectionEnum, RxRes<ProfileStatusDto>>();

  protected readonly expanded = signal<ProfileSectionEnum>(ProfileSectionEnum.Personal);

  constructor() {
    this.cards.forEach(card => {
      this.sections.set(
        card.section,
        rxResource<ProfileStatusDto, unknown>({
          stream: () => this.profileCqrs.section(card.section),
        })
      );
    });
  }

  readonly header = computed(() => this.basics.value());
  readonly loadingBasics = computed(() => this.basics.status() === 'loading');

  readonly reviewNotes = computed(() => {
    const review = this.review.value() as MyProfileReviewSummaryDto | undefined;
    if (!review) return undefined;

    const sectionIndex = (review.sections ?? []).reduce((acc, section) => {
      acc[section.section] = section.notesCount ?? 0;
      return acc;
    }, {} as Record<number, number>);

    return { ...review, sectionIndex } as MyProfileReviewSummaryDto & { sectionIndex: Record<number, number> };
  });

  openCard(section: ProfileSectionEnum) {
    const res = this.sections.get(section);
    this.expanded.set(section);
    res?.reload();
  }

  sectionStatus(section: ProfileSectionEnum) {
    return this.sections.get(section)?.status() ?? 'idle';
  }

  sectionValue(section: ProfileSectionEnum) {
    return this.sections.get(section)?.value() ?? null;
  }

  readonly changeRequestsVm = computed(() => {
    const items = (this.changeRequests.value() as ProfileChangeRequestDto[] | undefined) ?? [];
    return items.map(item => ({
      ...item,
      statusLabelKey: this.changeStatusLabelKey(item.status),
      statusSeverity: this.changeStatusSeverity(item.status),
      sectionLabelKey: this.sectionLabelKey(item.section)
    }));
  });

  openReviewStep() {
    const review = this.review.value() as MyProfileReviewSummaryDto | undefined;

    this.dialogService
      .open(ReviewStepsDialogComponent, {
        header: this.i18n.instant('profileOverview.reviewSteps.dialogTitle'),
        data: { sections: review?.sections ?? [] },
        styleClass: 'w-100 w-md-75'
      })
      ?.onClose.subscribe(result => {
      if (result?.section) {
        this.navigateToEditSection(result.section as ProfileSectionEnum);
      }
    });
  }

  openReviewItem(note: any, section: ProfileSectionEnum) {
    this.dialogService.open(ReviewItemEditDialogComponent, {
      header: this.i18n.instant('profileOverview.reviewItemDialog.title'),
      data: { note, section, sectionLabel: note?.title ?? '', canEdit: true, fileUrl: null },
      styleClass: 'w-100 w-md-50'
    });
  }

  navigateToEditSection(section: ProfileSectionEnum) {
    this.router.navigate([routes.user.profileEditSection(this.sectionSegment(section))]);
  }

  private sectionSegment(section: ProfileSectionEnum): string {
    switch (section) {
      case ProfileSectionEnum.Prerequisites:
        return 'prerequisites';
      case ProfileSectionEnum.Personal:
        return 'personal';
      case ProfileSectionEnum.Contact:
        return 'contact';
      case ProfileSectionEnum.Qualifications:
        return 'qualifications';
      case ProfileSectionEnum.Experience:
      case ProfileSectionEnum.TrainingCourses:
        return 'experience';
      case ProfileSectionEnum.CertificatesAndAwards:
        return 'achievements';
      case ProfileSectionEnum.Skills:
        return 'skills';
      case ProfileSectionEnum.Languages:
        return 'languages';
      case ProfileSectionEnum.Attachments:
      default:
        return 'attachments';
    }
  }

  openFile(url: string | null | undefined) {
    if (!url) return;
    this.fileUtils.previewUrl(url);
  }

  sectionLabelKey(section: number): string {
    switch (section) {
      case ProfileSectionEnum.Prerequisites:
        return 'profileOverview.sections.prerequisites';
      case ProfileSectionEnum.Personal:
        return 'profileOverview.sections.personal';
      case ProfileSectionEnum.Contact:
        return 'profileOverview.sections.contact';
      case ProfileSectionEnum.Qualifications:
        return 'profileOverview.sections.qualifications';
      case ProfileSectionEnum.Experience:
        return 'profileOverview.sections.experiences';
      case ProfileSectionEnum.TrainingCourses:
        return 'profileOverview.sections.trainingCourses';
      case ProfileSectionEnum.CertificatesAndAwards:
        return 'profileOverview.sections.certificatesAndAwards';
      case ProfileSectionEnum.Skills:
        return 'profileOverview.sections.skills';
      case ProfileSectionEnum.Languages:
        return 'profileOverview.sections.languages';
      case ProfileSectionEnum.Attachments:
      default:
        return 'profileOverview.sections.attachments';
    }
  }

  noteSeverity(status: number): 'warn' | 'danger' | 'secondary' {
    if (status === ReviewStatusEnum.NeedsCorrection) return 'warn';
    if (status === ReviewStatusEnum.Rejected) return 'danger';
    return 'secondary';
  }

  noteStatusLabelKey(status: number): string {
    if (status === ReviewStatusEnum.NeedsCorrection) return 'profileOverview.reviewStatus.needsCorrection';
    if (status === ReviewStatusEnum.Rejected) return 'profileOverview.reviewStatus.rejected';
    return 'profileOverview.reviewStatus.other';
  }

  changeStatusLabelKey(status: number): string {
    switch (status) {
      case ProfileChangeRequestStatusEnum.Pending:
        return 'profileOverview.changeRequests.status.pending';
      case ProfileChangeRequestStatusEnum.UnderReview:
        return 'profileOverview.changeRequests.status.underReview';
      case ProfileChangeRequestStatusEnum.Approved:
        return 'profileOverview.changeRequests.status.approved';
      case ProfileChangeRequestStatusEnum.Rejected:
        return 'profileOverview.changeRequests.status.rejected';
      case ProfileChangeRequestStatusEnum.Canceled:
        return 'profileOverview.changeRequests.status.canceled';
      default:
        return 'profileOverview.changeRequests.status.pending';
    }
  }

  changeStatusSeverity(status: number): 'success' | 'warn' | 'danger' | 'secondary' {
    switch (status) {
      case ProfileChangeRequestStatusEnum.Approved:
        return 'success';
      case ProfileChangeRequestStatusEnum.Pending:
      case ProfileChangeRequestStatusEnum.UnderReview:
        return 'warn';
      case ProfileChangeRequestStatusEnum.Rejected:
        return 'danger';
      default:
        return 'secondary';
    }
  }
}
