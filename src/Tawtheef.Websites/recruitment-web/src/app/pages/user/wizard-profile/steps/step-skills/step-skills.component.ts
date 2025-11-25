import { Component, EventEmitter, Output, inject, OnDestroy, OnInit } from '@angular/core';
import {AutoCompleteCompleteEvent, AutoCompleteSelectEvent, AutoCompleteUnselectEvent} from 'primeng/autocomplete';
import {Subject, Subscription, of, delay} from 'rxjs';
import {debounceTime, distinctUntilChanged, filter, switchMap, tap, catchError, map} from 'rxjs/operators';
import { DataService } from '../../services/data.service';
import {SkillDto} from '../../models/skill-dto.model';
import {ProfileLookupsService} from '../../services/profile-lookups.service';
import {dropdownOptionsModel} from '../../../../../shared/models/dropdown-options.model';

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

  // UI state
  skillOptions: SkillDto[] = [];
  loadingSkills = false;
  lastQuery = '';

  // language form bits
  newLanguage?: dropdownOptionsModel;
  newLevel?: dropdownOptionsModel;

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
  addSkill(e: AutoCompleteSelectEvent){
    this.ds.addSkill(e.value);
  }
  removeSkill(e: AutoCompleteUnselectEvent){
    this.ds.delSkill(e.value);
  }
  addLang(): void {
    if (this.newLanguage && this.newLevel) {
      this.ds.addLang({
        langId: this.newLanguage.id,
        langName: this.newLanguage.name,
        levelId: this.newLevel.id,
        levelName: this.newLevel.name
      });
      this.newLanguage = this.newLevel = undefined;
    }
  }
  removeLang(index: number){
    this.ds.delLang(index)
  }

  ngOnDestroy(): void {
    this.sub?.unsubscribe();
  }
}
