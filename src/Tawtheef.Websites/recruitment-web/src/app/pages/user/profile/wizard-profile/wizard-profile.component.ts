import {Component, inject, OnInit} from '@angular/core';
import {ProfileDataService} from './services/profile-data.service';
import {TranslateService} from '@ngx-translate/core';
import {ActivatedRoute, Router} from '@angular/router';
import {take} from 'rxjs';
import {finalize} from 'rxjs/operators';
import {ProfileLookupsService} from './services/profile-lookups.service';
import {DialogService} from 'primeng/dynamicdialog';
import {PhoneMapperService} from './services/phone-mapper.service';
import {mapProfileStatusToState} from './services/profile.mapper';
import {ProfileStatusDto} from '../../../../core/models/auth/auth-response.model';
import {AuthService} from '../../../../core/auth/auth.service';
import {LanguageService} from '../../../../core/services/language.service';
import {UserService} from '../../../../core/auth/user.service';
import {routes} from '../../../../routes/routes';
import {AvatarModal} from '../components/profile-steps/step-personal/dialogs/avatar.modal/avatar.modal';
import {ProfileService} from './services/profile.service';

@Component({
  selector: 'app-wizard-profile',
  templateUrl: './wizard-profile.component.html',
  styleUrls: ['./wizard-profile.component.scss'],
  standalone: false,
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

  avatarPreviewUrl: string | null = null;

  step = 1;
  total = 10;

  loading = true;
  private forcedStep: number | null = null;

  stepLabels: {label: string, icon: string}[] = [
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
          this.ds.up('available',available);
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
      const nav = this.router.currentNavigation();
      const state = nav?.extras.state as ProfileStatusDto | null;
      if (state) {
        this.avatarPreviewUrl = state.avatar ?? null;
        state.provider = provider;
        this.ds.prefillFromBootstrap(mapProfileStatusToState(this.phoneMapper,this.lookups,state, this.userService.getPrefill()));
        this.loading = false;
        this.moveToFirstInvalidStep();
        this.applyForcedStep();
        this.markTouched(this.step);
        return;
      }
      this.auth.getAuthBootstrap$()
        .pipe(take(1))
        .pipe(finalize(() => {
          this.loading = false;
        }))
        .subscribe((b: Partial<ProfileStatusDto>) => {
          if (b.isComplete) {
            this.router.navigate([routes.user.dashboard]);
            return;
          }
          this.avatarPreviewUrl = b.avatar ?? null;
          b.provider = provider;
          this.ds.prefillFromBootstrap(mapProfileStatusToState(this.phoneMapper,this.lookups,b as ProfileStatusDto, this.userService.getPrefill()));
          this.moveToFirstInvalidStep();
          this.applyForcedStep();
          this.markTouched(this.step);
        });
    })
  }

  moveToFirstInvalidStep() {
    //get the first step not valid by ds.stepValidity
    const firstInvalidStep =
      Array.from({length: this.total}, (_, i) => i + 1)
      .find(i => !this.isStepValid(i));

    //make all step until firstInvalidStep touched
    for (let i = 1; i < firstInvalidStep!; i++) {
      this.markTouched(i);
    }

    if (firstInvalidStep) {
      this.step = firstInvalidStep;
    }else{
      //move to the last step
      this.step = this.total;
    }
  }

  canGoTo(targetStep: number): boolean {
    if (targetStep === 1) return true;
    const validity = this.ds.stepValidity();
    for (let i = 0; i < targetStep - 1 && i < this.orderedValidationSteps.length; i++) {
      if (!validity[this.orderedValidationSteps[i]]) {
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
      contentStyle: {'max-height': '80vh', 'overflow': 'scroll'},
      baseZIndex: 10000,
      closable: true,
    })?.onClose.subscribe((croppedImage: string | null) => {
      if (croppedImage) {
        this.ds.up('avatarUrl', croppedImage);
        this.avatarPreviewUrl = croppedImage;
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
