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
  private lastSubmittedSignature: string | null = null;

  get step(){
    const stepValidity = createStepValiditySignal(this.ds.state);
    const validity = stepValidity();
    return validity['skills'];
  }
  // UI state
  skillOptions: dropdownOptionsModel[] = [];
  loadingSkills = false;
  lastQuery = '';

  selectedSkill?: dropdownOptionsModel;
  selectedLevel?: dropdownOptionsModel;

  // search stream
  private search$ = new Subject<string>();
  private sub?: Subscription;

  ngOnInit(): void {
    const state = this.ds.state();
    const signature = this.buildSignature(state.skills);
    this.lastSubmittedSignature = signature;

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
    e.value = null;
  }
  addSkill(){
    if (this.selectedSkill && this.selectedLevel) {
      const skill: Skill = {
        skillId: this.selectedSkill.id?.toString() ?? this.selectedSkill.name,
        skill: this.selectedSkill,
        levelId: this.selectedLevel.id,
        level: this.selectedLevel,
      };
      this.ds.addSkill(skill);
      this.selectedSkill = undefined;
      this.selectedLevel = undefined;
    }
  }
  removeSkill(index: number){
    var skill = this.ds.state().skills[index];
    if(skill.id){
      this.profile.deleteSkill(skill.id).subscribe({
        next: () => {
          this.ds.delSkill(index);
        },
        error: err => {
          console.error(err);
          this.messageService.add({
            severity: 'error',
            summary: this.translate.instant('wizard.errorTitle'),
            detail: this.translate.instant('wizard.skill.deleteError'),
            life: 5000,
          });
        },
      });
    } else {
      this.ds.delSkill(index);
    }
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

  private buildSignature(skills: Skill[]): string {
    return JSON.stringify(
      (skills ?? []).map(s => ({
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
