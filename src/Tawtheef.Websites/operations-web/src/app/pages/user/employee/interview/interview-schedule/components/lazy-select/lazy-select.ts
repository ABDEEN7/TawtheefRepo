import { ChangeDetectionStrategy, Component, computed, input, output, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormsModule } from '@angular/forms';
import { TranslatePipe } from '@ngx-translate/core';
import { catchError, debounceTime, map, merge, Observable, of, Subject, switchMap, tap } from 'rxjs';

import { Select } from 'primeng/select';

import { LazySelectOption } from '../../models/lazy-select-option.model';

// Debounce before a typed term hits the server; opening the dropdown loads immediately.
const SEARCH_DEBOUNCE_MS = 300;

/**
 * Searchable dropdown fed by the server instead of a preloaded list: it loads when opened and re-queries as the
 * user types, so no page state or full list is ever held. The caller supplies `search(term)`; stale in-flight
 * requests are cancelled, and the current selection stays renderable even when it isn't in the latest results.
 */
@Component({
  selector: 'app-lazy-select',
  imports: [FormsModule, TranslatePipe, Select],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { class: 'd-block w-100' },
  template: `
    <p-select
      class="w-100"
      optionLabel="label"
      optionValue="value"
      filterBy="searchText"
      [options]="viewOptions()"
      [filter]="true"
      [resetFilterOnHide]="true"
      [showClear]="showClear()"
      [appendTo]="'body'"
      [loading]="loading()"
      [disabled]="disabled()"
      [placeholder]="placeholder()"
      [ariaLabel]="ariaLabel()"
      [filterPlaceholder]="'common.search' | translate"
      [emptyMessage]="emptyMessageKey() | translate"
      [emptyFilterMessage]="emptyMessageKey() | translate"
      [ngModel]="selected()?.value ?? null"
      (ngModelChange)="onSelect($event)"
      (onShow)="show$.next()"
      (onFilter)="onFilter($event.filter ?? '')"
    >
      <ng-template #item let-option>
        <div class="d-flex flex-column">
          <span>{{ option.label }}</span>
          @if (option.hint) {
            <small class="text-muted">{{ option.hint }}</small>
          }
        </div>
      </ng-template>
    </p-select>
  `,
})
export class LazySelectComponent<T = unknown> {
  readonly search = input.required<(term: string) => Observable<LazySelectOption<T>[]>>();
  readonly selected = input<LazySelectOption<T> | null>(null);
  readonly placeholder = input('');
  readonly ariaLabel = input<string | undefined>(undefined);
  readonly disabled = input(false);
  readonly showClear = input(false);
  /** Values hidden from the list (e.g. people already on the roster); the current selection is never hidden. */
  readonly excludeValues = input<ReadonlySet<string>>(new Set());

  readonly selectedChange = output<LazySelectOption<T> | null>();

  protected readonly show$ = new Subject<void>();
  protected readonly filter$ = new Subject<string>();
  protected readonly loading = signal(false);
  private readonly loaded = signal(false);
  private readonly results = signal<LazySelectOption<T>[]>([]);

  // An empty list only means "no matches" once a search has actually come back. Until then (including the instant the panel
  // opens, before the first request starts) it means "not here yet", so say that instead of a misleading "No results".
  protected readonly emptyMessageKey = computed(() =>
    this.loading() || !this.loaded() ? 'common.loading' : 'common.noResults',
  );
  protected readonly viewOptions = computed(() => {
    const selected = this.selected();
    const excluded = this.excludeValues();
    const available = this.results().filter((o) => o.value === selected?.value || !excluded.has(o.value));

    if (!selected || available.some((o) => o.value === selected.value)) return available;
    return [this.withSearchText(selected), ...available];
  });

  constructor() {
    merge(this.show$.pipe(map(() => '')), this.filter$.pipe(debounceTime(SEARCH_DEBOUNCE_MS)))
      .pipe(
        tap(() => this.loading.set(true)),
        switchMap((term) =>
          this.search()(term).pipe(
            map((options) => options.map((o) => this.withSearchText(o))),
            catchError(() => of([] as LazySelectOption<T>[])),
          ),
        ),
        takeUntilDestroyed(),
      )
      .subscribe((options) => {
        this.results.set(options);
        this.loaded.set(true);
        this.loading.set(false);
      });
  }

  protected onFilter(term: string) {
    // PrimeNG filters the previous results instantly, but the server search only fires after the debounce - flag it as
    // searching right away so the panel never flashes "No results" in between.
    this.loading.set(true);
    this.filter$.next(term);
  }

  protected onSelect(value: string | null) {
    this.selectedChange.emit(this.viewOptions().find((o) => o.value === value) ?? null);
  }

  private withSearchText(option: LazySelectOption<T>): LazySelectOption<T> {
    return option.searchText ? option : { ...option, searchText: `${option.label} ${option.hint ?? ''}`.trim() };
  }
}
