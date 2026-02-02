import { CommonModule } from '@angular/common';
import { Component, Input, OnDestroy, OnInit, forwardRef, OnChanges, SimpleChanges, inject } from '@angular/core';
import { FormsModule, NG_VALUE_ACCESSOR, ControlValueAccessor } from '@angular/forms';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Subject, Subscription, of } from 'rxjs';
import { catchError, debounceTime, distinctUntilChanged, finalize, map, switchMap } from 'rxjs/operators';
import { SelectModule } from 'primeng/select';
import { TranslateService } from '@ngx-translate/core';

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
  translate = inject(TranslateService);
  emptyMessage = '';
  lastQuery = '';
  @Input() searchUrl!: string;
  @Input() minChars = 3;
  @Input() debounceMs = 400;
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

  // ====== Parent dependency (cascading) ======
  @Input() parentId: string | number | null | undefined;
  @Input() parentParamName = 'parentId';
  @Input() requireParent = false;

  // ====== Validation styling from parent ======
  @Input() invalid = false;

  disabled = false;
  options: any[] = [];
  isLoading = false;
  hasMore = true;
  pageIndex = 0;
  searchTerm = '';
  value: any;

  private search$ = new Subject<string>();
  private load$ = new Subject<{ searchTerm: string; pageIndex: number; append: boolean }>();
  private searchSub?: Subscription;
  private loadSub?: Subscription;

  constructor(private http: HttpClient) {}

  private onChange: (value: any) => void = () => {};
  private onTouched: () => void = () => {};

  writeValue(val: any): void {
    this.value = val;
    this.options = this.mergeWithSelected(this.preloadedOptions);

    if (this.value !== null && this.value !== undefined && this.value !== '') {
      this.loadPage(this.searchTerm, 0, false);
    }
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

  ngOnInit(): void {
    this.options = this.mergeWithSelected(this.preloadedOptions);

    this.loadSub = this.load$
      .pipe(
        switchMap(request => {
          if (!this.searchUrl) return of({ request, res: [] });

          if (this.requireParent && (this.parentId === null || this.parentId === undefined || this.parentId === '')) {
            return of({ request, res: [] });
          }

          this.isLoading = true;
          const params = this.buildParams(request.searchTerm, request.pageIndex, this.pageSize);

          return this.http.get<any[]>(this.searchUrl, { params }).pipe(
            map(res => ({ request, res: res ?? [] })),
            catchError(() => of({ request, res: [] })),
            finalize(() => (this.isLoading = false))
          );
        })
      )
      .subscribe(({ request, res }) => this.applyResults(request, res));

    this.searchSub = this.search$
      .pipe(
        debounceTime(this.debounceMs),
        distinctUntilChanged()
      )
      .subscribe(term => this.applySearch(term));

    this.loadPage('', 0, false);
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['parentId'] && !changes['parentId'].firstChange) {
      this.options = [];
      this.value = null;
      this.onChange(null);
      this.pageIndex = 0;
      this.searchTerm = '';
      this.hasMore = true;

      if (!this.requireParent || (this.parentId !== null && this.parentId !== undefined && this.parentId !== '')) {
        this.loadPage('', 0, false);
      }
    }

    if (changes['preloadedOptions']) {
      this.options = this.mergeWithSelected(this.preloadedOptions);

      if (this.value !== null && this.value !== undefined && this.value !== '') {
        this.loadPage(this.searchTerm, 0, false);
      }
    }
  }

  ngOnDestroy(): void {
    this.searchSub?.unsubscribe();
    this.loadSub?.unsubscribe();
  }

  onOpen(): void {
    if (!this.lastQuery || this.lastQuery.length < this.minChars) {
      this.emptyMessage = this.translate.instant('remote-select.search-hint', { minChars: this.minChars });
    }
  }
  // ====== Template helpers ======
  get isDisabledComputed(): boolean {
    if (this.disabled) return true;
    if (this.requireParent && (this.parentId === null || this.parentId === undefined || this.parentId === '')) {
      return true;
    }
    if (this.disableWhileLoading && this.isLoading) return true;
    return false;
  }

  get computedPlaceholder(): string {
    if (this.requireParent && (this.parentId === null || this.parentId === undefined || this.parentId === '')) {
      return this.placeholder;
    }
    return this.placeholder;
  }

  // ====== Events ======
  onFilter(event: any): void {
    const query = event?.filter ?? '';
    this.lastQuery = query;

    if (!query) {
      this.emptyMessage = '';
      this.pageIndex = 0;
      this.searchTerm = '';
      this.hasMore = true;
      this.loadPage('', 0, false);
      return;
    }

    if (query.length < this.minChars) {
      this.emptyMessage = this.translate.instant('remote-select.search-hint', { minChars: this.minChars });
      this.options = this.mergeWithSelected([]);
      this.hasMore = false;
      return;
    }
    this.emptyMessage = '';
    this.search$.next(query);
  }

  onLazyLoad(event: { first?: number; rows?: number }): void {
    if (this.isLoading || !this.hasMore) return;

    const first = event?.first ?? 0;
    const nextPage = Math.floor(first / this.pageSize);
    if (nextPage <= this.pageIndex) return;

    this.loadPage(this.searchTerm, nextPage, true);
  }

  handleChange(event: any): void {
    const newVal = event?.value;
    this.value = newVal;
    this.onChange(newVal);
    this.onTouched();
  }

  private buildParams(searchTerm: string, pageIndex: number, pageSize: number): HttpParams {
    let params = new HttpParams();

    if (typeof this.value !== 'object' && this.value !== null && this.value !== undefined && this.value !== '') {
      params = params.set(this.idParamName, String(this.value));
    }

    const q = (searchTerm ?? '').trim();
    if (q) {
      params = params.set(this.searchParamName, q);
    }

    if (this.parentId !== null && this.parentId !== undefined && this.parentParamName) {
      params = params.set(this.parentParamName, String(this.parentId));
    }

    params = params.set('pageIndex', String(pageIndex));
    params = params.set('pageSize', String(pageSize));

    return params;
  }

  private loadPage(searchTerm: string, pageIndex: number, append: boolean): void {
    if (!this.searchUrl) return;
    if (this.requireParent && (this.parentId === null || this.parentId === undefined || this.parentId === '')) {
      return;
    }

    this.searchTerm = searchTerm;
    this.pageIndex = pageIndex;
    this.load$.next({ searchTerm, pageIndex, append });
  }

  private applySearch(term: string): void {
    const query = term?.trim() ?? '';
    this.lastQuery = query;

    if (!query) {
      this.emptyMessage = '';
      this.pageIndex = 0;
      this.searchTerm = '';
      this.hasMore = true;
      this.loadPage('', 0, false);
      return;
    }

    if (query.length < this.minChars) {
      this.emptyMessage = this.translate.instant('remote-select.search-hint', { minChars: this.minChars });
      this.options = this.mergeWithSelected([]);
      this.hasMore = false;
      return;
    }

    this.emptyMessage = '';
    this.pageIndex = 0;
    this.hasMore = true;
    this.loadPage(query, 0, false);
  }

  private mergeWithSelected(nextOptions: any[]): any[] {
    const merged = [...(nextOptions || [])];

    const selectedOption = this.findSelectedOption();
    if (selectedOption && !this.containsOption(merged, selectedOption)) {
      merged.push(selectedOption);
    }

    return merged;
  }

  private findSelectedOption(): any | null {
    if (this.value === null || this.value === undefined) return null;

    if (!this.optionValue) {
      return this.value;
    }

    const fromPreloaded = this.preloadedOptions?.find(opt => this.getOptionValue(opt) === this.value);
    if (fromPreloaded) return fromPreloaded;

    const fromOptions = this.options?.find(opt => this.getOptionValue(opt) === this.value);
    return fromOptions ?? null;
  }

  private containsOption(options: any[], option: any): boolean {
    if (!this.optionValue) {
      return options.includes(option);
    }

    const optionVal = this.getOptionValue(option);
    return options.some(opt => this.getOptionValue(opt) === optionVal);
  }

  private getOptionValue(option: any): any {
    if (!option || !this.optionValue) return option;
    return option?.[this.optionValue];
  }

  private applyResults(
    request: { searchTerm: string; pageIndex: number; append: boolean },
    res: any[]
  ): void {
    const next = res || [];
    if (request.append) {
      const merged = this.mergeById([...this.options, ...next]);
      this.options = this.mergeWithSelected(merged);
    } else {
      this.options = this.mergeWithSelected(this.mergeById(next));
    }

    this.hasMore = next.length === this.pageSize;
  }

  private mergeById(items: any[]): any[] {
    const seen = new Map<string, any>();
    for (const item of items) {
      const key = this.getOptionId(item);
      if (!key) continue;
      if (!seen.has(key)) {
        seen.set(key, item);
      }
    }
    return Array.from(seen.values());
  }

  private getOptionId(option: any): string | null {
    const id = option?.id ?? option?.Id;
    return id ? String(id) : null;
  }
}
