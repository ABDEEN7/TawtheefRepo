import { CommonModule } from '@angular/common';
import {
  Component,
  Input,
  OnDestroy,
  OnInit,
  OnChanges,
  SimpleChanges,
  forwardRef,
  inject,
  EventEmitter,
  Output,
} from '@angular/core';
import { ControlValueAccessor, FormsModule, NG_VALUE_ACCESSOR } from '@angular/forms';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Subject, of } from 'rxjs';
import { catchError, finalize, map, switchMap, takeUntil } from 'rxjs/operators';
import { SelectModule } from 'primeng/select';
import { TranslateService } from '@ngx-translate/core';

type LoadRequest = { term: string; page: number; append: boolean };

type QueryParamValue =
  | string
  | number
  | boolean
  | null
  | undefined
  | Array<string | number | boolean>;

type ExtraQueryParams =
  | Record<string, QueryParamValue>
  | (() => Record<string, QueryParamValue>);

@Component({
  selector: 'app-remote-select',
  standalone: true,
  imports: [CommonModule, FormsModule, SelectModule],
  templateUrl: './remote-select.html',
  styleUrls: ['./remote-select.scss'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => RemoteSelectComponent),
      multi: true,
    },
  ],
})
export class RemoteSelectComponent implements OnInit, OnDestroy, OnChanges, ControlValueAccessor {
  private http = inject(HttpClient);
  private translate = inject(TranslateService);

  /** Optional: keep if you still want external listeners */
  @Output() valueChange = new EventEmitter<any>();

  @Input() searchUrl!: string;
  @Input() minChars = 3;
  @Input() searchParamName = 'search';
  @Input() idParamName = 'id';
  @Input() pageSize = 10;

  @Input() optionLabel = 'name';
  @Input() optionValue?: string;
  @Input() placeholder = '';
  @Input() size: 'small' | 'large' | undefined = undefined;
  @Input() appendTo: any = 'body';
  @Input() panelStyle: any;

  @Input() disableWhileLoading = true;
  @Input() showClear = false;
  @Input() preloadedOptions: any[] = [];

  @Input() virtualScrollItemSize = 38;
  @Input() parentId: string | number | null | undefined;
  @Input() parentParamName = 'parentId';
  @Input() requireParent = false;

  @Input() invalid = false;
  @Input() extraQueryParams?: ExtraQueryParams;

  options: any[] = [];
  value: any = null;

  isLoading = false;
  hasMore = true;
  emptyMessage = '';

  private panelOpen = false;
  private currentTerm = '';
  private pageNumber = 0;

  private requestedPages = new Set<string>();
  private destroy$ = new Subject<void>();
  private request$ = new Subject<LoadRequest>();

  private onChange: (value: any) => void = () => {};
  private onTouched: () => void = () => {};
  disabled = false;

  /** --- Option B flags --- */
  private initialized = false;
  private pendingFirstLoad: LoadRequest | null = null;

  /**
   * “One-way” behavior:
   * - allow only the FIRST parent write to set initial value
   * - ignore subsequent parent writes (so parent can’t overwrite the user selection)
   */
  private hasAcceptedInitialWrite = false;

  get isDisabledComputed(): boolean {
    if (this.disabled) return true;
    if (this.requireParent && this.isParentMissing()) return true;
    if (this.disableWhileLoading && this.isLoading && !this.panelOpen) return true;
    return false;
  }

  get computedPlaceholder(): string {
    return this.placeholder;
  }

  // =============================
  // CVA
  // =============================
  writeValue(val: any): void {
    const isClear = val === null || val === undefined || val === '';

    // ✅ ALWAYS allow programmatic clear from parent
    if (isClear) {
      this.value = null;

      // keep dataset stable (don’t collapse height)
      const merged = this.mergeById([...(this.options ?? []), ...(this.preloadedOptions ?? [])]);
      this.options = this.mergeWithSelected(merged);

      // optional: reload first page after clear
      if (!this.requireParent || !this.isParentMissing()) {
        this.reloadFirstPage(this.currentTerm ?? '');
      }

      return;
    }

    // ✅ Option B: ignore parent writes after initial accept
    if (this.hasAcceptedInitialWrite) {
      return;
    }

    // Accept initial value once
    this.hasAcceptedInitialWrite = true;
    this.value = val;

    const merged = this.mergeById([...(this.options ?? []), ...(this.preloadedOptions ?? [])]);
    this.options = this.mergeWithSelected(merged);

    if (this.requireParent && this.isParentMissing()) return;

    const req: LoadRequest = { term: this.currentTerm ?? '', page: 0, append: false };

    if (!this.initialized) {
      this.pendingFirstLoad = req;
      return;
    }

    this.resetPagingOnly(true);
    this.requestedPages.add(this.requestKey(0, req.term));
    this.load(req);
  }

  registerOnChange(fn: any): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.disabled = isDisabled;
  }

  // =============================
  // Lifecycle
  // =============================
  ngOnInit(): void {
    this.options = this.mergeWithSelected(this.preloadedOptions);

    this.request$
      .pipe(
        switchMap(req => {
          if (!this.searchUrl) return of({ req, res: [] as any[] });
          if (this.requireParent && this.isParentMissing()) return of({ req, res: [] as any[] });

          this.isLoading = true;
          const params = this.buildParams(req.term, req.page, this.pageSize);

          return this.http
            .get<any[]>(this.searchUrl, {
              params,
              headers: new HttpHeaders({ 'X-Skip-Loading': 'true' }),
            })
            .pipe(
              map(res => ({ req, res: res ?? [] })),
              catchError(() => of({ req, res: [] as any[] })),
              finalize(() => (this.isLoading = false))
            );
        }),
        takeUntil(this.destroy$)
      )
      .subscribe(({ req, res }) => this.applyResults(req, res));

    this.initialized = true;

    // ✅ flush any pre-init request
    if (this.pendingFirstLoad) {
      const req = this.pendingFirstLoad;
      this.pendingFirstLoad = null;

      this.requestedPages.add(this.requestKey(req.page, req.term));
      this.load(req);
      return;
    }

    // initial load if no parent requirement
    if (!this.requireParent || !this.isParentMissing()) {
      this.reloadFirstPage('');
    }
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['preloadedOptions']) {
      // don’t wipe current options; merge
      const merged = this.mergeById([...(this.options ?? []), ...(this.preloadedOptions ?? [])]);
      this.options = this.sortByOptionLabel(this.mergeWithSelected(merged));
    }

    if (changes['parentId'] && !changes['parentId'].firstChange) {
      // parent changed => we must reset selection
      this.value = null;

      // allow parent to set value again if it wants (new context)
      this.hasAcceptedInitialWrite = false;

      this.onChange(null);
      this.onTouched();
      this.valueChange.emit(null);

      this.options = this.mergeWithSelected([]);
      this.resetPagingOnly(false);

      if (!this.requireParent || !this.isParentMissing()) {
        this.reloadFirstPage('');
      }
    }
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  // =============================
  // UI events
  // =============================
  onOpen(): void {
    this.panelOpen = true;
    if (!this.currentTerm || this.currentTerm.length < this.minChars) {
      this.emptyMessage = this.translate.instant('remote-select.search-hint', { minChars: this.minChars });
    }
  }

  onClose(): void {
    this.panelOpen = false;
  }

  onFilter(event: any): void {
    const term = (event?.filter ?? '').toString().trim();

    if (this.requireParent && this.isParentMissing()) {
      this.options = this.mergeWithSelected([]);
      this.hasMore = false;
      this.emptyMessage = '';
      return;
    }

    if (!term) {
      this.emptyMessage = '';
      this.reloadFirstPage('');
      return;
    }

    if (term.length < this.minChars) {
      this.emptyMessage = this.translate.instant('remote-select.search-hint', { minChars: this.minChars });

      // ✅ IMPORTANT: don’t clear options (prevents empty/height collapse)
      this.hasMore = false;
      return;
    }

    this.emptyMessage = '';
    this.reloadFirstPage(term);
  }

  /**
   * ✅ PrimeNG correct lazy paging: derive page from first/rows
   */
  onLazyLoad(event: { first?: number; rows?: number }): void {
    if (this.isLoading) return;
    if (!this.hasMore) return;
    if (this.requireParent && this.isParentMissing()) return;

    const first = event?.first ?? 0;
    const rows = event?.rows ?? this.pageSize;
    const page = Math.floor(first / rows);

    const key = this.requestKey(page, this.currentTerm);
    if (this.requestedPages.has(key)) return;

    this.requestedPages.add(key);
    this.load({ term: this.currentTerm, page, append: page > 0 });
  }

  handleChange(event: any): void {
    const newVal = event?.value;
    this.value = newVal;

    // ✅ user-driven change flows outward
    this.onChange(newVal);
    this.onTouched();
    this.valueChange.emit(newVal);

    // ✅ lock out future parent writes (one-way)
    this.hasAcceptedInitialWrite = true;
  }

  onClearSelection(): void {
    this.value = null;

    this.onChange(null);
    this.onTouched();
    this.valueChange.emit(null);

    // still one-way after user action
    this.hasAcceptedInitialWrite = true;

    if (!this.requireParent || !this.isParentMissing()) {
      this.reloadFirstPage('');
    }
  }

  // =============================
  // Data flow
  // =============================
  private reloadFirstPage(term: string): void {
    this.currentTerm = term;
    this.resetPagingOnly(true);

    const key0 = this.requestKey(0, term);
    this.requestedPages.add(key0);

    // ✅ don’t clear options here
    this.load({ term, page: 0, append: false });
  }

  private resetPagingOnly(preserveTerm: boolean): void {
    this.requestedPages.clear();
    this.pageNumber = 0;
    this.hasMore = true;
    if (!preserveTerm) this.currentTerm = '';
  }

  private load(req: LoadRequest): void {
    if (!this.searchUrl) return;
    if (this.requireParent && this.isParentMissing()) return;

    this.currentTerm = (req.term ?? '').trim();
    this.pageNumber = req.page;

    this.request$.next(req);
  }

  private isParentMissing(): boolean {
    return this.parentId === null || this.parentId === undefined || this.parentId === '';
  }

  private requestKey(page: number, term: string): string {
    const parentKey = this.parentId ?? '';
    const t = (term ?? '').trim();
    return `${parentKey}::${t}::${page}`;
  }

  private buildParams(term: string, pageNumber: number, pageSize: number): HttpParams {
    let params = new HttpParams();

    // include selected id for “fetch by id” support
    if (typeof this.value !== 'object' && this.value !== null && this.value !== undefined && this.value !== '') {
      params = params.set(this.idParamName, String(this.value));
    }

    const q = (term ?? '').trim();
    if (q) params = params.set(this.searchParamName, q);

    if (this.parentId !== null && this.parentId !== undefined && this.parentParamName) {
      params = params.set(this.parentParamName, String(this.parentId));
    }

    params = params
      .set('PaginatedRequest.PageNumber', String(pageNumber + 1))
      .set('PaginatedRequest.PageSize', String(pageSize))
      .set('PaginatedRequest.SortBy', 'name')
      .set('PaginatedRequest.SortDirection', 'asc');

    return this.appendExtraParams(params);
  }

  private applyResults(req: LoadRequest, res: any[]): void {
    const next = res ?? [];

    const mergedBase = req.append
      ? this.mergeById([...(this.options ?? []), ...next])
      : this.mergeById(next);

    const mergedWithSelected = this.mergeWithSelected(mergedBase);

    this.options = this.sortByOptionLabel(mergedWithSelected);
    this.hasMore = next.length === this.pageSize;
  }

  // =============================
  // helpers (yours unchanged)
  // =============================
  private mergeWithSelected(nextOptions: any[]): any[] {
    const merged = [...(nextOptions || [])];
    const selectedOption = this.findSelectedOption();
    if (selectedOption && !this.containsOption(merged, selectedOption)) merged.push(selectedOption);
    return merged;
  }

  private findSelectedOption(): any | null {
    if (this.value === null || this.value === undefined) return null;

    if (!this.optionValue) return this.value;

    const fromPreloaded = this.preloadedOptions?.find(opt => this.getOptionValue(opt) === this.value);
    if (fromPreloaded) return fromPreloaded;

    const fromOptions = this.options?.find(opt => this.getOptionValue(opt) === this.value);
    return fromOptions ?? null;
  }

  private containsOption(options: any[], option: any): boolean {
    if (!this.optionValue) return options.includes(option);
    const optionVal = this.getOptionValue(option);
    return options.some(opt => this.getOptionValue(opt) === optionVal);
  }

  private getOptionValue(option: any): any {
    if (!option || !this.optionValue) return option;
    return option?.[this.optionValue];
  }

  private mergeById(items: any[]): any[] {
    const seen = new Map<string, any>();
    for (const item of items) {
      const key = this.getOptionId(item);
      if (!key) continue;
      if (!seen.has(key)) seen.set(key, item);
    }
    return Array.from(seen.values());
  }

  private getOptionId(option: any): string | null {
    const id = option?.id ?? option?.Id;
    return id ? String(id) : null;
  }

  private getByPath(obj: any, path: string): any {
    if (!obj || !path) return undefined;
    return path.split('.').reduce((acc, key) => (acc == null ? undefined : acc[key]), obj);
  }

  private getOptionLabelValue(option: any): string {
    const key = (this.optionLabel ?? '').trim();
    const raw = key.includes('.') ? this.getByPath(option, key) : option?.[key];
    return (raw ?? '').toString().trim();
  }

  private sortByOptionLabel(items: any[]): any[] {
    const collator = new Intl.Collator(undefined, { numeric: true, sensitivity: 'base' });
    return [...(items ?? [])].sort((a, b) => collator.compare(this.getOptionLabelValue(a), this.getOptionLabelValue(b)));
  }

  private appendExtraParams(params: HttpParams): HttpParams {
    const extra = typeof this.extraQueryParams === 'function' ? this.extraQueryParams() : this.extraQueryParams;
    if (!extra) return params;

    for (const [key, value] of Object.entries(extra)) {
      if (!key) continue;
      if (value === null || value === undefined || value === '') continue;

      if (Array.isArray(value)) {
        for (const v of value) {
          if (v === null || v === undefined || v === ('' as any)) continue;
          params = params.append(key, String(v));
        }
        continue;
      }

      params = params.set(key, String(value));
    }
    return params;
  }
}
