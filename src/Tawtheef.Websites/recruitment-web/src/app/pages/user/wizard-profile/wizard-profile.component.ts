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

  step = 1;
  total = 7;

  progress = this.ds.progress;
  progressText = computed(() => this.progress() + '%');

  ngOnInit(): void {
    const nav = this.router.currentNavigation();
    const state = nav?.extras.state as {
      prefill?: PrefillData | null;
      missing?: string[];
    } | undefined;

    if (state?.prefill) {
      // case 1: arrived here from guard -> we already have data
      this.ds.prefillFromBootstrap({
        email: state.prefill.email ?? null,
        givenNameEn: state.prefill.givenNameEn ?? null,
        familyNameEn: state.prefill.familyNameEn ?? null,
        avatar: state.prefill.avatar ?? null,
        phone: state.prefill.phone ?? null,
      } as Partial<ProfileState>);
      return;
    }

    // case 2: direct URL or page refresh -> no navigation state
    this.auth.getAuthBootstrap$()
      .pipe(take(1))
      .subscribe((b: AuthBootstrap) => {
        if (!b.requiresProfileCompletion) {
          this.router.navigate([routes.user.dashboard]);
          return;
        }

        if(b.prefill)
          this.ds.prefillFromBootstrap({
            email: b.prefill.email ?? null,
            givenNameEn: b.prefill.givenNameEn ?? null,
            familyNameEn: b.prefill.familyNameEn ?? null,
            avatar: b.prefill.avatar ?? null,
            phone: b.prefill.phone ?? null,
          } as Partial<ProfileState>);
      });
  }
  go(n: number){ if(n>=1 && n<=this.total) this.step = n; }
  next(){ if(this.step < this.total) this.step++; }
  prev(){ if(this.step > 1) this.step--; }

  toggleLang(){
    this.language.toggle();
  }
}
