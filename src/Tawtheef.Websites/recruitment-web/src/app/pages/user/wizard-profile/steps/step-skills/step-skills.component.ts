import { Component, EventEmitter, Output, inject, OnDestroy, OnInit } from '@angular/core';
import {AutoCompleteCompleteEvent, AutoCompleteSelectEvent} from 'primeng/autocomplete';
import {Subject, Subscription, of} from 'rxjs';
import {debounceTime, distinctUntilChanged, filter, switchMap, tap, catchError, map} from 'rxjs/operators';
import { DataService } from '../../services/data.service';
import {SkillDto} from '../../models/skill-dto.model';
import {ProfileLookupsService} from '../../services/profile-lookups.service';
import {dropdownOptionsModel} from '../../../../../shared/models/dropdown-options.model';
import {createStepValiditySignal} from '../../state/profile-step-validity.signal';
import {MessageService} from 'primeng/api';
import {TranslateService} from '@ngx-translate/core';
import {ProfileService} from '../../services/profile.service';
import {Skill} from '../../models/skill.model';

@Component({
  selector: 'app-step-skills',
  templateUrl: './step-skills.component.html',
  styleUrls: ['./step-skills.component.scss'],
  standalone: false,
})
export class StepSkillsComponent implements OnInit, OnDestroy {
  @Output() back = new EventEmitter<void>();
  @Output() next = new EventEmitter<void>();

  ds = inject(DataService);
  lookups = inject(ProfileLookupsService);
  messageService = inject(MessageService);
  translate = inject(TranslateService);
  profile = inject(ProfileService);

  saving = false;
  
  get step(){
    const stepValidity = createStepValiditySignal(this.ds.state);
    const validity = stepValidity();
    return validity['skills'];
  }
  // UI state
  skillOptions: SkillDto[] = [];
  loadingSkills = false;
  lastQuery = '';

  selectedSkill?: SkillDto;
  selectedLevel?: dropdownOptionsModel;

  // search stream
  private search$ = new Subject<string>();
  private sub?: Subscription;

  ngOnInit(): void {
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
        distinctUntilChanged(),
        tap(() => {
          this.loadingSkills = true;
          this.skillOptions = [];
        }),
        switchMap(q =>
          this.lookups.searchSkills(q).pipe(
            tap(() => (this.loadingSkills = false)),
            catchError(err => {
              console.error(err);
              this.loadingSkills = false;
              this.skillOptions = [];
              return of([]);
            })
          )
        )
      )
      .subscribe(res => {
        this.skillOptions = res;
      });
  }

  // PrimeNG completeMethod hook
  onSkillSearch(e: AutoCompleteCompleteEvent): void {
    const q = (e?.query ?? '').trim();
    if (q.length < 3) {
      this.skillOptions = [];
      this.loadingSkills = false;
      return;
    }
    this.search$.next(q);
  }
  onSkillSelect(e: AutoCompleteSelectEvent){
    this.selectedSkill = e.value;
  }
  addSkill(){
    if (this.selectedSkill && this.selectedLevel) {
      const skill: Skill = {
        id: this.selectedSkill.id?.toString(),
        skillId: this.selectedSkill.id?.toString() ?? this.selectedSkill.name,
        skillName: this.selectedSkill.name,
        levelId: this.selectedLevel.id,
        levelName: this.selectedLevel.name,
      };
      this.ds.addSkill(skill);
      this.selectedSkill = undefined;
      this.selectedLevel = undefined;
    }
  }
  removeSkill(skill: Skill){
    this.ds.delSkill(skill.skillId);
  }

  onNext() {
    if (!this.step.valid) {
      this.messageService.add({
        severity: 'error',
        summary: this.translate.instant('wizard.validationErrorTitle'),
        detail: this.step.errors.map(e => `* ${this.translate.instant(e.i18nKey)}`).join('\n'),
        life: 5000,
      });
      return;
    }
    const state = this.ds.state();
    const skills = state.skills || [];
    const languages = state.languages || [];

    this.saving = true;
    this.profile.saveSkillsSection(skills, languages).subscribe({
      next: () => {
        this.saving = false;
        this.next.emit();
      },
      error: err => {
        console.error(err);
        this.saving = false;
        this.messageService.add({
          severity: 'error',
          summary: this.translate.instant('wizard.errorTitle'),
          detail: this.translate.instant('wizard.skills.saveError'),
          life: 5000,
        });
      },
    });
  }

  ngOnDestroy(): void {
    this.sub?.unsubscribe();
  }
}
