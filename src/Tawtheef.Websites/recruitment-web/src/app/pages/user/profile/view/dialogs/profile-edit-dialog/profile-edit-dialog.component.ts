import { CommonModule, NgSwitch, NgSwitchCase } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, effect, inject, signal, untracked } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';
import { forkJoin } from 'rxjs';
import { rxResource } from '@angular/core/rxjs-interop';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';

import { ProfileLookupsService } from '../../../wizard-profile/services/profile-lookups.service';
import { ProfileService } from '../../../wizard-profile/services/profile.service';
import { ProfileDataService } from '../../../wizard-profile/services/profile-data.service';
import { PhoneMapperService } from '../../../wizard-profile/services/phone-mapper.service';
import { mapProfileStatusToState } from '../../../wizard-profile/services/profile.mapper';
import { MyProfileReviewNoteDto, ProfileSectionEnum, UserProfileStatusEnum } from '../../models/profile-overview.model';

// Step components
import { StepPrereqComponent } from '../../../components/profile-steps/step-first-info/step-prereq.component';
import { StepPersonalComponent } from '../../../components/profile-steps/step-personal/step-personal.component';
import { StepContactComponent } from '../../../components/profile-steps/step-contact/step-contact.component';
import { StepDegreeComponent } from '../../../components/profile-steps/step-degree/step-degree.component';
import { StepExperienceComponent } from '../../../components/profile-steps/step-experience/step-experience.component';
import { StepAchievementsComponent } from '../../../components/profile-steps/step-achievements/step-achievements.component';
import { StepSkillsComponent } from '../../../components/profile-steps/step-skills/step-skills.component';
import { StepLanguagesComponent } from '../../../components/profile-steps/step-languages/step-languages.component';
import { StepAttachmentsComponent } from '../../../components/profile-steps/step-attachments/step-attachments.component';
import { PROFILE_WRITE_MODE, ProfileWriteMode } from '../../../wizard-profile/services/profile-write-mode.token';
import { UserService } from '../../../../../../core/auth/user.service';
import { I18nNamespaceDirective } from '../../../../../../shared/directives/i18n-namespace.directive';
import { ProfileCorrectionContext } from '../../../wizard-profile/models/profile-correction.model';

type EditSection =
  | 'prerequisites' | 'personal' | 'contact' | 'qualifications'
  | 'experience' | 'training' | 'achievements' | 'skills'
  | 'languages' | 'attachments';

@Component({
  selector: 'app-profile-edit-dialog',
  standalone: true,
  imports: [
    CommonModule,
    TranslatePipe,
    NgSwitchCase,
    NgSwitch,
    I18nNamespaceDirective,
    StepPrereqComponent,
    StepPersonalComponent,
    StepContactComponent,
    StepDegreeComponent,
    StepExperienceComponent,
    StepAchievementsComponent,
    StepSkillsComponent,
    StepLanguagesComponent,
    StepAttachmentsComponent
  ],
  templateUrl: './profile-edit-dialog.component.html',
  styleUrls: ['./profile-edit-dialog.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [
    ProfileDataService,
    ProfileService,
    { provide: PROFILE_WRITE_MODE, useValue: 'create' },
  ],
})
export class ProfileEditDialogComponent {
  private readonly config = inject(DynamicDialogConfig);
  private readonly ref = inject(DynamicDialogRef);
  private readonly lookups = inject(ProfileLookupsService);
  private readonly profile = inject(ProfileService);
  private readonly ds = inject(ProfileDataService);
  private readonly userService = inject(UserService);
  private readonly phoneMapperService = inject(PhoneMapperService);

  private readonly section = signal<EditSection>('personal');
  private readonly mode = signal<ProfileWriteMode>('create');
  readonly additionOnly = signal(false);

  data = rxResource({
    stream: () => forkJoin({
      lookups: this.lookups.loadAll(),
      status: this.profile.getProfileStatus()
    })
  });
  loading = computed(() => this.data.status() === 'loading');
  vmReady = computed(() => {
    const res = this.data.value();
    return !!res?.status;
  });

  constructor() {
    const dialogSection = this.config.data?.['section'] as ProfileSectionEnum | undefined;
    const dialogMode = this.config.data?.['mode'] as ProfileWriteMode | undefined;
    const dialogNotes = this.config.data?.['notes'] as MyProfileReviewNoteDto[] | undefined;
    this.additionOnly.set(this.config.data?.['additionOnly'] === true);
    this.section.set(this.mapSection(dialogSection ?? ProfileSectionEnum.Personal));
    this.mode.set(dialogMode ?? 'create');
    this.profile.setWriteMode(this.mode());
    if (dialogNotes && dialogSection) {
      const correctionContext: ProfileCorrectionContext[] = dialogNotes.map(note => ({
        section: dialogSection,
        targetType: note.targetType,
        fieldPath: note.fieldPath,
        entityId: note.entityId,
        resourceId: note.resourceId,
        note: note.note,
        status: note.status,
      }));
      this.ds.setCorrections(correctionContext);
    }

    effect(() => {
      const res = this.data.value();
      if (!res?.status) return;

      const resolvedMode = this.resolveWriteMode(res.status.status);
      this.mode.set(resolvedMode);
      this.profile.setWriteMode(resolvedMode);

      const prefill = this.userService.getPrefill?.() ?? null;
      untracked(() => {
        const state = mapProfileStatusToState(
          this.phoneMapperService,
          this.lookups,
          res.status,
          prefill
        );

        this.ds.setState(state);
      });
    });
  }

  sectionKey() {
    return this.section();
  }

  close() {
    this.ref.close(true);
  }

  private mapSection(section: ProfileSectionEnum): EditSection {
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
        return 'experience';
      case ProfileSectionEnum.TrainingCourses:
        return 'training';
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

  private resolveWriteMode(status: number | null | undefined): ProfileWriteMode {
    if (status === UserProfileStatusEnum.Approved) return 'change-request';
    if (status === UserProfileStatusEnum.RequiresUpdate) return 'review-edit';
    return 'create';
  }
}
