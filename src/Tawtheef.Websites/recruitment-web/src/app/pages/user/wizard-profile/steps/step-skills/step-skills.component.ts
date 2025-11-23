import { Component, EventEmitter, Output, inject, OnDestroy, OnInit } from '@angular/core';
import {AutoCompleteCompleteEvent, AutoCompleteSelectEvent, AutoCompleteUnselectEvent} from 'primeng/autocomplete';
import {Subject, Subscription, of, delay} from 'rxjs';
import { debounceTime, distinctUntilChanged, filter, switchMap, tap, catchError } from 'rxjs/operators';
import { DataService } from '../../services/data.service';
import {SkillDto} from '../../models/skill-dto.model';

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

  // UI state
  skillOptions: SkillDto[] = [];
  loadingSkills = false;
  lastQuery = '';

  // language form bits
  newName?: string;
  newLevel?: string;

  // dropdown options (يمكنك ربطها بالترجمة)
  languageOptions = [
    { label: 'العربية', value: 'العربية' },
    { label: 'الإنجليزية', value: 'الإنجليزية' },
    { label: 'الفرنسية', value: 'الفرنسية' },
    { label: 'الألمانية', value: 'الألمانية' },
    { label: 'الإسبانية', value: 'الإسبانية' },
  ];

  levelOptions = [
    { label: 'مبتدئ', value: 'مبتدئ' },
    { label: 'متوسط', value: 'متوسط' },
    { label: 'متقدم', value: 'متقدم' },
    { label: 'ممتاز', value: 'ممتاز' },
  ];

  // search stream
  private search$ = new Subject<string>();
  private sub?: Subscription;

  ngOnInit(): void {
    const data: SkillDto[] = [ { id: 1, name: 'JavaScript' }, { id: 2, name: 'TypeScript' }, { id: 3, name: 'Angular' }, { id: 4, name: 'React' }, { id: 5, name: 'Vue.js' }, { id: 6, name: 'Node.js' } ];
    this.sub = this.search$
      .pipe(
        tap(q => {
          this.lastQuery = q;
          // Reset options immediately for new search
          if (q.trim().length < 3) {
            this.skillOptions = [];
            this.loadingSkills = false;
            return;
          }
        }),
        debounceTime(300),
        distinctUntilChanged(),
        filter(q => q?.trim().length >= 3),
        tap(() => {
          this.loadingSkills = true;
          this.skillOptions = [];
        }),
        switchMap(q => of(data.filter(skill =>
            skill.name.toLowerCase().includes(q.toLowerCase())
          )).pipe(
            delay(500),
            tap(() => {
              this.loadingSkills = false;
            }),
            catchError(error => {
              console.error(error);
              this.loadingSkills = false;
              this.skillOptions = [];
              return of([]);
            })
          ))
      )
      .subscribe((res) => {
        this.skillOptions = res;
      });
  }

  ngOnDestroy(): void {
    this.sub?.unsubscribe();
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
    if (this.newName && this.newLevel) {
      this.ds.addLang({ name: this.newName, level: this.newLevel });
      this.newName = this.newLevel = undefined;
    }
  }
  removeLang(index: number){
    this.ds.delLang(index)
  }
}
