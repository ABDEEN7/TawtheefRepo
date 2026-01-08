import {ChangeDetectionStrategy, Component, computed, effect, inject, untracked} from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { forkJoin } from 'rxjs';
import { rxResource } from '@angular/core/rxjs-interop';
import {ProfileLookupsService} from '../../wizard-profile/services/profile-lookups.service';
import {ProfileService} from '../../wizard-profile/services/profile.service';
import {ProfileDataService} from '../../wizard-profile/services/profile-data.service';
import {PhoneMapperService} from '../../wizard-profile/services/phone-mapper.service';
import {mapProfileStatusToState} from '../../wizard-profile/services/profile.mapper';
import {UserService} from '../../../../../core/auth/user.service';
import {ProfileComponentsModule} from '../../components/profile-components.module';
import {NgSwitch, NgSwitchCase} from '@angular/common';
import {I18nNamespaceDirective} from '../../../../../shared/directives/i18n-namespace.directive';
import { routes } from '../../../../../routes/routes';

type EditSection =
  | 'prerequisites' | 'personal' | 'contact' | 'qualifications'
  | 'experience' | 'training' | 'achievements' | 'skills'
  | 'languages' | 'attachments';

@Component({
  selector: 'app-profile-edit-shell-page',
  templateUrl: './profile-edit-shell.page.html',
  styleUrls: ['./profile-edit-shell.page.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    ProfileComponentsModule,
    NgSwitchCase,
    NgSwitch,
    I18nNamespaceDirective
  ],
  standalone: true
})
export class ProfileEditShellPage {
  private route = inject(ActivatedRoute);
  private router = inject(Router);

  private lookups = inject(ProfileLookupsService);
  private profile = inject(ProfileService);
  private ds = inject(ProfileDataService);
  private userService = inject(UserService);
  private phoneMapperService = inject(PhoneMapperService);

  section = computed<EditSection>(() => (this.route.snapshot.data['section'] as EditSection) ?? 'personal');
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
    const mode = this.route.snapshot.queryParamMap.get('mode');
    this.profile.setWriteMode(mode === 'change-request' ? 'change-request' : 'create');
    effect(() => {
      const res = this.data.value();
      if (!res?.status) return;

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

  backToOverview() {
    this.router.navigate([routes.user.profileOverview]);
  }
}
