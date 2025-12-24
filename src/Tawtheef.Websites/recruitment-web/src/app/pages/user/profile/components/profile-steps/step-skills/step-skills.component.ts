import {
  Component,
  EventEmitter,
  Output,
  OnDestroy,
  OnInit,
  inject,
} from '@angular/core';
import {
  AutoComplete,
  AutoCompleteCompleteEvent,
  AutoCompleteSelectEvent,
} from 'primeng/autocomplete';
import { Subject, Subscription, of } from 'rxjs';
import {
  debounceTime,
  filter,
  switchMap,
  tap,
  catchError,
  map,
  finalize,
} from 'rxjs/operators';
import {ProfileDataService} from '../../../wizard-profile/services/profile-data.service';
import {ProfileLookupsService} from '../../../wizard-profile/services/profile-lookups.service';
import {MessageService} from 'primeng/api';
import {TranslateService} from '@ngx-translate/core';
import {ProfileService} from '../../../wizard-profile/services/profile.service';
import {createStepValiditySignal} from '../../../wizard-profile/state/profile-step-validity.signal';
import {dropdownOptionsModel} from '../../../../../../shared/models/dropdown-options.model';
import {Skill} from '../../../wizard-profile/models/skill.model';
import {NotificationService} from '../../../../../../core/services/notification.service';

@Component({
  selector: 'app-step-skills',
  templateUrl: './step-skills.component.html',
  styleUrl: './step-skills.component.scss',
  standalone: false
})
export class StepSkillsComponent implements OnInit, OnDestroy {
  @Output() back = new EventEmitter<void>();
  @Output() next = new EventEmitter<void>();

  protected readonly ds = inject(ProfileDataService);
  protected readonly lookups = inject(ProfileLookupsService);
  private readonly notificationService = inject(NotificationService);
  private readonly messageService = inject(MessageService);
  private readonly translate = inject(TranslateService);
  private readonly profile = inject(ProfileService);

  saving = false;
  private lastSubmittedSignature: string | null = null;

  private readonly stepValidity = createStepValiditySignal(this.ds.state);
  get step() {
    const validity = this.stepValidity();
    return validity['skills'];
  }

  skillOptions: dropdownOptionsModel[] = [];
  loadingSkills = false;
  lastQuery = '';

  skillSearchModel: dropdownOptionsModel | null = null;

  selectedSkill: dropdownOptionsModel | null = null;
  selectedLevel: dropdownOptionsModel | null = null;

  // search stream
  private readonly search$ = new Subject<string>();
  private sub?: Subscription;

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
    this.selectedSkill = e.value as dropdownOptionsModel;
  }

  addSkill(): void {
    if (this.selectedSkill && this.selectedLevel) {
      if(this.ds.state().skills.some(s=> s.skill?.backendName == this.selectedSkill?.backendName)){
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
        },
        error: (err: any) => {
          console.error(err);
        },
      });
    } else {
      this.ds.delSkill(index);
    }
  }

  onNext(): void {
    if (!this.step.valid) {
      this.notificationService.error(this.step.errors
          .map(e => `* ${this.translate.instant(e.i18nKey)}`)
          .join('\n'));
      return;
    }

    const state = this.ds.state();
    const skills = state.skills || [];
    const signature = this.buildSignature(skills);

    if (signature && signature === this.lastSubmittedSignature) {
      this.next.emit();
      return;
    }

    this.saving = true;
    this.profile.saveSkillsSection(skills).subscribe({
      next: () => {
        this.saving = false;
        this.lastSubmittedSignature = signature;
        this.next.emit();
      },
      error: (err: any) => {
        console.error(err);
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
