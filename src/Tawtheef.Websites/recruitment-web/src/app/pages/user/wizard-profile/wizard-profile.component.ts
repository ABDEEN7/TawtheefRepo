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

@Component({
  selector: 'app-wizard-profile',
  templateUrl: './wizard-profile.component.html',
  styleUrls: ['./wizard-profile.component.scss'],
  standalone: false,
})
export class WizardProfileComponent implements OnInit {
  ds = inject(DataService);
  language = inject(LanguageService);
  private router = inject(Router);
  private auth = inject(AuthService);
  lookups = inject(ProfileLookupsService);

  step = 1;
  total = 8;

  loading = true;

  progress = this.ds.progress;
  progressText = computed(() => this.progress() + '%');
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
      this.ds.prefillFromBootstrap({
        fullName: state.prefill.fullName ?? null,
        fullNameEn: state.prefill.fullName ?? null,
        qid: state.prefill.qid ?? null,
        // gender: state.prefill.gender ?? null,
        dob: state.prefill.dob ?? null,
        phone: state.prefill.phone ?? null,
        email: state.prefill.email ?? null,
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
          this.ds.prefillFromBootstrap({
            fullName: b.prefill.fullName ?? null,
            fullNameEn: b.prefill.fullName ?? null,
            qid: b.prefill.qid ?? null,
            // gender: b.prefill.gender ?? null,
            dob: b.prefill.dob ?? null,
            phone: b.prefill.phone ?? null,
            email: b.prefill.email ?? null,
            // nationality: b.prefill.nationality ?? null,
            avatarUrl: b.prefill.avatar ?? null,
          } as Partial<ProfileState>);
        }
      });

  }
  canGoTo(targetStep: number): boolean {
    const v = this.ds.stepValidity();
    const orderedSteps: (keyof typeof v)[] = [
      'personal',
      'contact',
      'degrees',
      'experience',
      'skills',
      'attachments'
    ];

    if (targetStep === 1) return true;
    for (let i = 0; i < targetStep - 1 && i < orderedSteps.length; i++) {
      if (!v[orderedSteps[i]]) {
        return false;
      }
    }

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
}

