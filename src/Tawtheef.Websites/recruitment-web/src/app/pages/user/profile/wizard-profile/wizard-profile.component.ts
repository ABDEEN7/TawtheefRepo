import { Component, inject, OnInit, signal, computed, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { take } from 'rxjs';
import { finalize } from 'rxjs/operators';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { DialogService } from 'primeng/dynamicdialog';
import { ToggleSwitchModule } from 'primeng/toggleswitch';
import { ProfileDataService } from './services/profile-data.service';
import { ProfileLookupsService } from './services/profile-lookups.service';
import { PhoneMapperService } from './services/phone-mapper.service';
import { mapProfileStatusToState } from './services/profile.mapper';
import { ProfileStatusDto } from '../../../../core/models/auth/auth-response.model';
import { AuthService } from '../../../../core/auth/auth.service';
import { LanguageService } from '../../../../core/services/language.service';
import { UserService } from '../../../../core/auth/user.service';
import { routes } from '../../../../routes/routes';
import { ProfileService } from './services/profile.service';
import { AvatarUtils } from '../../../../core/utils/avatar-utils';
import { BOOTSTRAP_KEY } from '../../../../core/guards/profile-complete.guard';
import { PROFILE_WRITE_MODE } from './services/profile-write-mode.token';
import { I18nNamespaceDirective } from '../../../../shared/directives/i18n-namespace.directive';

// Step components
import { StepPrereqComponent } from '../components/profile-steps/step-first-info/step-prereq.component';
import { StepPersonalComponent } from '../components/profile-steps/step-personal/step-personal.component';
import { StepContactComponent } from '../components/profile-steps/step-contact/step-contact.component';
import { StepDegreeComponent } from '../components/profile-steps/step-degree/step-degree.component';
import { StepExperienceComponent } from '../components/profile-steps/step-experience/step-experience.component';
import { StepAchievementsComponent } from '../components/profile-steps/step-achievements/step-achievements.component';
import { StepSkillsComponent } from '../components/profile-steps/step-skills/step-skills.component';
import { StepLanguagesComponent } from '../components/profile-steps/step-languages/step-languages.component';
import { StepAttachmentsComponent } from '../components/profile-steps/step-attachments/step-attachments.component';
import { StepReviewComponent } from './steps/step-review/step-review.component';
import { AvatarModal } from '../components/profile-steps/step-personal/dialogs/avatar.modal/avatar.modal';

@Component({
  selector: 'app-wizard-profile',
  templateUrl: './wizard-profile.component.html',
  styleUrls: ['./wizard-profile.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [
    DialogService,
    ProfileDataService,
    ProfileService,
    { provide: PROFILE_WRITE_MODE, useValue: 'create' },
  ],
  imports: [
    CommonModule,
    FormsModule,
    TranslatePipe,
    ToggleSwitchModule,
    I18nNamespaceDirective,
    StepPrereqComponent,
    StepPersonalComponent,
    StepContactComponent,
    StepDegreeComponent,
    StepExperienceComponent,
    StepAchievementsComponent,
    StepSkillsComponent,
    StepLanguagesComponent,
    StepAttachmentsComponent,
    StepReviewComponent
  ]
})
export class WizardProfileComponent implements OnInit {
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private auth = inject(AuthService);
  private dialog = inject(DialogService);
  private translate = inject(TranslateService);
  ds = inject(ProfileDataService);
  lookups = inject(ProfileLookupsService);
  language = inject(LanguageService);
  phoneMapper = inject(PhoneMapperService);
  userService = inject(UserService);
  profileService = inject(ProfileService);

  avatarPreviewUrl: string | null = null
  defaultAvatar = AvatarUtils.default;

  step = 1;
  total = 10;

  loading = true;
  private forcedStep: number | null = null;

  stepLabels: { label: string, icon: string }[] = [
    { label: 'wizard.steps.firstInfo', icon: '' },
    { label: 'wizard.steps.personal', icon: 'hgi-user' },
    { label: 'wizard.steps.contact', icon: 'hgi-house-02' },
    { label: 'wizard.steps.degrees', icon: 'hgi-school' },
    { label: 'wizard.steps.experience', icon: 'hgi-briefcase-05' },
    { label: 'wizard.steps.achievements', icon: '' },
    { label: 'wizard.steps.skills', icon: 'hgi-ai-user' },
    { label: 'wizard.steps.languages', icon: 'hgi-checkmark-circle-02' },
    { label: 'wizard.steps.attachments', icon: 'hgi-file-upload' },
    { label: 'wizard.steps.review', icon: '' },
  ];

  private stepKeyMap: Record<number,
    keyof ReturnType<typeof this.ds.stepValidity>> = {
      1: 'basic',
      2: 'personal',
      3: 'contact',
      4: 'degrees',
      5: 'experience',
      6: 'achievements',
      7: 'skills',
      8: 'languages',
      9: 'attachments',
    };

  private orderedValidationSteps: (keyof ReturnType<typeof this.ds.stepValidity>)[] = [
    'basic',
    'personal',
    'contact',
    'degrees',
    'experience',
    'achievements',
    'skills',
    'languages',
    'attachments',
  ];
  private touchedSteps = new Set<number>();

  private markTouched(step: number) {
    this.touchedSteps.add(step);
  }
  isStepTouched(step: number): boolean {
    return this.touchedSteps.has(step);
  }
  // Complete = valid AND touched
  isStepComplete(step: number): boolean {
    return this.isStepTouched(step) && this.isStepValid(step);
  }
  isCurrentStepValid(): boolean {
    const validity = this.ds.stepValidity();
    const key = this.stepKeyMap[this.step];
    if (!key) return true;
    return validity[key];
  }

  isStepValid(step: number): boolean {
    const validity = this.ds.stepValidity();
    const key = this.stepKeyMap[step];
    if (!key) return this.canGoTo(step);
    return validity[key];
  }

  completedSteps(): number {
    const validity = this.ds.stepValidity();
    const completeCount = this.orderedValidationSteps.reduce((count, key) =>
      count + (validity[key] ? 1 : 0), 0);
    const reviewUnlocked = this.canGoTo(this.total) ? 1 : 0;
    return Math.min(this.total, completeCount + reviewUnlocked);
  }
  onAvailabilityChange(available: boolean) {
    this.profileService
      .saveRecruitmentAvailability(available)
      .subscribe({
        next: () => {
          this.ds.up('available', available);
        }
      });
  }

  ngOnInit(): void {
    const provider = this.userService.getCurrentUser()?.provider ?? 'Google';
    this.forcedStep = this.parseStep(this.route.snapshot.queryParamMap.get('step'));
    this.route.queryParamMap.subscribe(params => {
      const stepParam = this.parseStep(params.get('step'));
      if (stepParam) {
        this.forcedStep = stepParam;
        this.applyForcedStep();
      }
    });
    this.lookups.loadAll().subscribe(() => {

      const provider = this.userService.getCurrentUser()?.provider ?? 'Google';
      this.auth.getAuthBootstrap$()
        .pipe(take(1), finalize(() => (this.loading = false)))
        .subscribe((b: Partial<ProfileStatusDto>) => {
          if (b.isComplete) {
            this.router.navigate([routes.user.dashboard]);
            return;
          }

          const prefill = this.userService.getPrefill();
          this.avatarPreviewUrl = prefill?.avatar ?? b.avatar ?? this.userService.getCurrentUser()?.profilePictureUrl ?? null;
          (b as any).provider = provider;

          this.ds.prefillFromBootstrap(
            mapProfileStatusToState(this.phoneMapper, this.lookups, b as ProfileStatusDto, prefill)
          );

          this.moveToFirstInvalidStep();
          this.applyForcedStep();
          this.markTouched(this.step);
        });
    });
  }
  private readBootstrapFromStorage(): Partial<ProfileStatusDto> | null {
    try {
      const raw = sessionStorage.getItem(BOOTSTRAP_KEY);
      if (!raw) return null;
      return JSON.parse(raw) as Partial<ProfileStatusDto>;
    } catch {
      return null;
    }
  }
  moveToFirstInvalidStep() {
    //get the first step not valid by ds.stepValidity
    const firstInvalidStep =
      Array.from({ length: this.total }, (_, i) => i + 1)
        .find(i => !this.isStepValid(i));

    //make all step until firstInvalidStep touched
    for (let i = 1; i < firstInvalidStep!; i++) {
      this.markTouched(i);
    }

    if (firstInvalidStep) {
      this.step = firstInvalidStep;
    } else {
      //move to the last step
      this.step = this.total;
    }
  }

  canGoTo(targetStep: number): boolean {
    if (targetStep === 1) return true;
    const validity = this.ds.stepValidity();
    for (let i = 0; i < targetStep - 1 && i < this.orderedValidationSteps.length; i++) {
      const stepKey = this.orderedValidationSteps[i];
      // Each preceding step must be both valid AND successfully submitted to the server
      if (!validity[stepKey] || !this.ds.isStepSubmitted(stepKey)) {
        return false;
      }
    }

    return true;
  }

  go(step: number) {
    if (!this.canGoTo(step)) return;
    this.step = step;
    this.markTouched(this.step);
  }

  next() {
    this.markTouched(this.step);
    if (!this.isCurrentStepValid()) {
      return;
    }

    if (this.step < this.total) {
      this.step++;
      this.markTouched(this.step);
    }
  }

  prev() {
    this.markTouched(this.step);
    if (this.step > 1) {
      this.step--;
      this.markTouched(this.step);
    }
  }

  openAvatarDialog() {
    this.dialog.open(AvatarModal, {
      header: this.translate.instant('wizard.personal.avatar.title'),
      width: '80%',
      contentStyle: { 'max-height': '80vh', 'overflow': 'scroll' },
      baseZIndex: 10000,
      closable: true,
      draggable: false,
    })?.onClose.subscribe((croppedImage: string | null) => {
      if (croppedImage) {
        this.ds.up('avatarUrl', croppedImage);
        this.avatarPreviewUrl = croppedImage;
        this.userService.updateProfilePicture(croppedImage);
      }
    });
  }

  private parseStep(value: string | null): number | null {
    if (!value) return null;
    const parsed = Number(value);
    if (!Number.isFinite(parsed)) return null;
    if (parsed < 1 || parsed > this.total) return null;
    return parsed;
  }

  private applyForcedStep() {
    if (!this.forcedStep) return;
    if (this.canGoTo(this.forcedStep)) {
      this.step = this.forcedStep;
    }
  }
}
