import {Component, computed, inject, OnInit} from '@angular/core';
import {DataService} from './services/data.service';
import {TranslateService} from '@ngx-translate/core';
import {LanguageService} from '../../../core/services/language.service';
import {ProfileState} from './models/profile-state.model';
import {Router} from '@angular/router';
import {AuthService} from '../../../core/auth/auth.service';
import {take} from 'rxjs';
import {AuthBootstrap, PrefillData} from '../../../core/models/auth/auth-response.model';
import {routes} from '../../../routes/routes';
import {finalize} from 'rxjs/operators';
import {ProfileLookupsService} from './services/profile-lookups.service';
import {AvatarModal} from './steps/step-personal/dialogs/avatar.modal/avatar.modal';
import {DialogService} from 'primeng/dynamicdialog';
import {PhoneMapperService} from './services/phone-mapper.service';

@Component({
  selector: 'app-wizard-profile',
  templateUrl: './wizard-profile.component.html',
  styleUrls: ['./wizard-profile.component.scss'],
  standalone: false,
})
export class WizardProfileComponent implements OnInit {
  private router = inject(Router);
  private auth = inject(AuthService);
  private dialog = inject(DialogService);
  private translate = inject(TranslateService);
  ds = inject(DataService);
  lookups = inject(ProfileLookupsService);
  language = inject(LanguageService);
  phoneMapper = inject(PhoneMapperService);

  avatarPreviewUrl: string | null = null;

  step = 1;
  total = 8;

  loading = true;

  private stepKeyMap: Record<number,
    keyof ReturnType<typeof this.ds.stepValidity>> = {
    1: 'basic',
    2: 'personal',
    3: 'contact',
    4: 'degrees',
    5: 'experience',
    6: 'skills',
    7: 'attachments',
  };

  isCurrentStepValid(): boolean {
    const validity = this.ds.stepValidity();
    const key = this.stepKeyMap[this.step];
    if (!key) return true;
    return validity[key];
  }
  ngOnInit(): void {
    this.lookups.loadAll();

    const nav = this.router.currentNavigation();
    const state = nav?.extras.state as {
      prefill?: PrefillData | null;
      missing?: string[];
    } | undefined;
    if (state?.prefill) {
      this.avatarPreviewUrl = state.prefill.avatar ?? null;
      this.ds.prefillFromBootstrap({
        fullName: state.prefill.fullName ?? null,
        fullNameEn: state.prefill.fullName ?? null,
        qid: state.prefill.qid ?? null,
        dob: state.prefill.dob ?? null,
        phone: this.phoneMapper.toPhoneObject(state.prefill.phone) ?? false,
        phoneVerified: state.prefill.phoneVerified ?? false,
        email: state.prefill.email ?? null,
        emailVerified: state.prefill.emailVerified ?? false,
        // gender: state.prefill.gender ?? null,
        // nationality: state.prefill.nationality ?? null,
        avatarUrl: state.prefill.avatar ?? null,
      } as Partial<ProfileState>);
      this.loading = false;
      return;
    }
    this.auth.getAuthBootstrap$()
      .pipe(take(1))
      .pipe(finalize(() => {this.loading = false;}))
      .subscribe((b: AuthBootstrap) => {
        if (!b.requiresProfileCompletion) {
          this.router.navigate([routes.user.dashboard]);
          return;
        }

        if (b.prefill) {
          this.avatarPreviewUrl =  b.prefill.avatar ?? null;
          this.ds.prefillFromBootstrap({
            fullName: b.prefill.fullName ?? null,
            fullNameEn: b.prefill.fullName ?? null,
            qid: b.prefill.qid ?? null,
            dob: b.prefill.dob ?? null,
            phone: this.phoneMapper.toPhoneObject(b.prefill.phone) ?? null,
            phoneVerified: b.prefill.phoneVerified ?? false,
            email: b.prefill.email ?? null,
            emailVerified: b.prefill.emailVerified ?? false,
            // nationality: b.prefill.nationality ?? null,
            // gender: b.prefill.gender ?? null,
            avatarUrl: b.prefill.avatar ?? null,
          } as Partial<ProfileState>);
        }
      });

  }
  canGoTo(targetStep: number): boolean {
    // const v = this.ds.stepValidity();
    // const orderedSteps: (keyof typeof v)[] = [
    //   'personal',
    //   'contact',
    //   'degrees',
    //   'experience',
    //   'skills',
    //   'attachments'
    // ];

    // if (targetStep === 1) return true;
    // for (let i = 0; i < targetStep - 1 && i < orderedSteps.length; i++) {
    //   if (!v[orderedSteps[i]]) {
    //     return false;
    //   }
    // }

    return true;
  }
  go(step: number) {
    if (!this.canGoTo(step)) return;
    this.step = step;
  }
  next() {
    if (!this.isCurrentStepValid()) {
      return;
    }

    if (this.step < this.total) {
      this.step++;
    }
  }
  prev() {
    if (this.step > 1) {
      this.step--;
    }
  }
  toggleLang(){
    this.language.toggle();
  }
  openAvatarDialog() {
    this.dialog.open(AvatarModal, {
      header: this.translate.instant('wizard.personal.avatar.title'),
      width: '80%',
      contentStyle: { 'max-height': '80vh', 'overflow': 'visible' },
      baseZIndex: 10000,
      closable: true,
    })?.onClose.subscribe((croppedImage: string | null) => {
      if (croppedImage) {
        this.ds.up('avatarUrl', croppedImage);
        this.avatarPreviewUrl = croppedImage;
      }
    });
  }
}

