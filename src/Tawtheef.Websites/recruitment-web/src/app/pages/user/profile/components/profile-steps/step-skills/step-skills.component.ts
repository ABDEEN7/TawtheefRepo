import {
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  EventEmitter,
  inject,
  Input,
  isDevMode,
  OnDestroy,
  OnInit,
  Output,
  computed,
  input,
  output
} from '@angular/core';
import { CommonModule, NgClass } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { AutoCompleteCompleteEvent, AutoCompleteSelectEvent, AutoCompleteModule } from 'primeng/autocomplete';
import { SelectModule } from 'primeng/select';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { of, Subject, Subscription } from 'rxjs';
import { catchError, debounceTime, filter, finalize, map, switchMap, tap } from 'rxjs/operators';
import { TooltipModule } from 'primeng/tooltip';
import { FaDirArrowDirective } from '../../../../../../shared/directives/dir-arrow.directive';
import { ProfileDataService } from '../../../wizard-profile/services/profile-data.service';
import { ProfileLookupsService } from '../../../wizard-profile/services/profile-lookups.service';
import { ProfileService } from '../../../wizard-profile/services/profile.service';
import { dropdownOptionsModel, DropdownOptionVM } from '../../../../../../shared/models/dropdown-options.model';
import { Skill } from '../../../wizard-profile/models/skill.model';
import { NotificationService } from '../../../../../../core/services/notification.service';
import { EndpointsService } from '../../../../../../core/http/endpoints.service';
import { RemoteSelectComponent } from '../../../../../../shared/components/remote-select/remote-select';

@Component({
  selector: 'app-step-skills',
  templateUrl: './step-skills.component.html',
  styleUrl: './step-skills.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    CommonModule,
    FormsModule,
    TranslatePipe,
    AutoCompleteModule,
    SelectModule,
    ButtonModule,
    TableModule,
    FaDirArrowDirective,
    RemoteSelectComponent,
    TooltipModule
  ]
})
export class StepSkillsComponent implements OnInit, OnDestroy {
  back = output<void>();
  next = output<void>();
  submitLabelKey = input<string>('wizard.buttons.next');
  showBack = input<boolean>(true);
  requireChanges = input<boolean>(false);

  protected readonly ds = inject(ProfileDataService);
  protected readonly lookups = inject(ProfileLookupsService);
  protected readonly endpoints = inject(EndpointsService);
  private readonly notificationService = inject(NotificationService);
  private readonly translate = inject(TranslateService);
  private readonly profile = inject(ProfileService);
  private readonly cdr = inject(ChangeDetectorRef);

  saving = false;
  private lastSubmittedSignature: string | null = null;

  step = computed(() => this.ds.stepValidationDetailed().skills);

  skillOptions: DropdownOptionVM[] = [];
  loadingSkills = false;
  lastQuery = '';

  skillSearchModel: DropdownOptionVM | null = null;

  selectedSkill: DropdownOptionVM | null = null;
  selectedLevel: DropdownOptionVM | null = null;

  // search stream
  private readonly search$ = new Subject<string>();
  private sub?: Subscription;
  buildUserExtraParams = () => ({
    majors: this.ds.state().degrees
      .filter((degree) => degree?.major)
      .map((degree) => degree.major!.id)
      .concat(
        this.ds.state().degrees
          .filter((degree) => degree?.subMajor)
          .map((degree) => degree.subMajor!.id)
      )
  });
  ngOnInit(): void {
    const state = this.ds.state();
    this.lastSubmittedSignature = null;

    this.sub = this.search$
      .pipe(
        map(q => (q ?? '').trim()),
        tap(q => {
          this.lastQuery = q;
          if (q.length < 3) {
            this.skillOptions = [];
            this.loadingSkills = false;
          }
        }),
        filter(q => q.length >= 3),
        debounceTime(300),
        switchMap(q => {
          this.loadingSkills = true;
          this.skillOptions = [];

          return this.lookups.searchSkills(q).pipe(
            catchError(err => {
              console.error(err);
              this.skillOptions = [];
              return of([]);
            }),
            finalize(() => {
              this.loadingSkills = false;
            })
          );
        })
      )
      .subscribe(res => {
        this.skillOptions = res;
        this.cdr.detectChanges();
      });
  }

  onSkillSearch(e: AutoCompleteCompleteEvent): void {
    const q = (e?.query ?? '').trim();

    if (q.length < 3) {
      this.skillOptions = [];
      this.loadingSkills = false;
      return;
    }

    this.search$.next(q);
  }

  onSkillSelect(e: AutoCompleteSelectEvent): void {
    this.selectedSkill = e.value as DropdownOptionVM;
  }

  addSkill(): void {
    if (this.selectedSkill && this.selectedLevel) {
      if (this.ds.state().skills.some(s => s.skill?.backendName == this.selectedSkill?.backendName)) {
        this.notificationService.error(this.translate.instant('wizard.profile.skills.duplicateMessage'));
        return;
      }
      const skill: Skill = {
        skillId: this.selectedSkill.id?.toString() ?? this.selectedSkill.name,
        skill: this.selectedSkill,
        levelId: this.selectedLevel.id,
        level: this.selectedLevel,
      };

      this.ds.addSkill(skill);
      this.selectedSkill = null;
      this.selectedLevel = null;
      this.skillSearchModel = null;
      this.skillOptions = [];
      this.lastQuery = '';
    }
  }

  removeSkill(index: number): void {
    const skill = this.ds.state().skills[index];

    if (skill.id) {
      this.profile.deleteSkill(skill.id).subscribe({
        next: () => {
          this.ds.delSkill(index);
        }
      });
    } else {
      this.ds.delSkill(index);
    }
  }

  onNext(): void {
    if (!this.step().valid) {
      this.notificationService.error(this.step().errors
        .map(e => `* ${this.translate.instant(e.i18nKey)}`)
        .join('\n'));
      return;
    }

    const state = this.ds.state();
    const skills = state.skills || [];
    const signature = this.buildSignature(skills);

    if (signature && signature === this.lastSubmittedSignature) {
      if (this.requireChanges()) {
        this.notificationService.error(this.translate.instant('profileView.notifications.noChanges'));
        return;
      }
      this.notificationService.info(this.translate.instant('profileView.notifications.noChanges'));
      this.next.emit();
      return;
    }

    this.saving = true;
    this.profile.saveSkillsSection(skills).subscribe({
      next: () => {
        this.saving = false;
        this.lastSubmittedSignature = signature;
        this.ds.markStepSubmitted('skills');
        if (this.profile.isChangeRequestMode()) {
          this.notificationService.success(this.translate.instant('profileView.notifications.changeRequestSent'));
        }
        this.next.emit();
      },
      error: (err: any) => {
        this.saving = false;
      },
    });
  }

  private buildSignature(skills: Skill[] | null | undefined): string {
    const safe = skills ?? [];
    return JSON.stringify(
      safe.map(s => ({
        id: s.id ?? null,
        skillId: s.skillId ?? s.id ?? null,
        levelId: s.levelId ?? s.level?.id ?? null,
      }))
    );
  }

  ngOnDestroy(): void {
    this.sub?.unsubscribe();
  }
}
