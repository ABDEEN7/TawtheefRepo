import {Component, inject, OnInit} from '@angular/core';
import {DataService} from './services/data.service';
import {TranslateService} from '@ngx-translate/core';
import {LanguageService} from '../../../core/services/language.service';
import {Router} from '@angular/router';
import {AuthService} from '../../../core/auth/auth.service';
import {take} from 'rxjs';
import {FileRefDto, PrefillData, ProfileStatusDto} from '../../../core/models/auth/auth-response.model';
import {routes} from '../../../routes/routes';
import {finalize} from 'rxjs/operators';
import {ProfileLookupsService} from './services/profile-lookups.service';
import {AvatarModal} from './steps/step-personal/dialogs/avatar.modal/avatar.modal';
import {DialogService} from 'primeng/dynamicdialog';
import {PhoneMapperService} from './services/phone-mapper.service';
import {mapProfileStatusToState} from './services/profile.mapper';

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
    this.lookups.loadAll().subscribe(() => {
      const nav = this.router.currentNavigation();
      const state = nav?.extras.state as ProfileStatusDto | null;
      if (state) {
        this.avatarPreviewUrl = state.avatar ?? null;
        this.ds.prefillFromBootstrap(mapProfileStatusToState(this.phoneMapper,this.lookups,state));
        this.loading = false;
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
          this.ds.prefillFromBootstrap(mapProfileStatusToState(this.phoneMapper,this.lookups,b as ProfileStatusDto));
        });
    })

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
}
