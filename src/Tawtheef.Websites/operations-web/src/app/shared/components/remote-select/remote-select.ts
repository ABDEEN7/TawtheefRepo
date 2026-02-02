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
} from '@angular/core';
import { ControlValueAccessor, FormsModule, NG_VALUE_ACCESSOR } from '@angular/forms';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Subject, of } from 'rxjs';
import { catchError, finalize, map, switchMap, takeUntil } from 'rxjs/operators';
import { SelectModule } from 'primeng/select';
import { TranslateService } from '@ngx-translate/core';
type LoadRequest = { term: string; page: number; append: boolean };

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

  options: any[] = [];
  value: any = null;
  isLoading = false;
  hasMore = true;
  emptyMessage = '';
  private panelOpen = false;
  private currentTerm = '';
  private pageIndex = 0;

  private requestedPages = new Set<string>();
  private destroy$ = new Subject<void>();
  private request$ = new Subject<LoadRequest>();

  private onChange: (value: any) => void = () => {};
  private onTouched: () => void = () => {};

  writeValue(val: any): void {
    this.value = val;
    this.options = this.mergeWithSelected(this.preloadedOptions);

    if (this.value !== null && this.value !== undefined && this.value !== '') {
      this.resetDataset(false);
      this.load({ term: this.currentTerm, page: 0, append: false });
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

  disabled = false;

  get isDisabledComputed(): boolean {
    if (this.disabled) return true;
    if (this.requireParent && this.isParentMissing()) return true;
    if (this.disableWhileLoading && this.isLoading && !this.panelOpen) return true;
    return false;
  }

  get computedPlaceholder(): string {
    return this.placeholder;
  }

  ngOnInit(): void {
    this.options = this.mergeWithSelected(this.preloadedOptions);

    this.request$
      .pipe(
        switchMap(req => {
          if (!this.searchUrl) return of({ req, res: [] as any[] });
          if (this.requireParent && this.isParentMissing()) return of({ req, res: [] as any[] });

          this.isLoading = true;
          const params = this.buildParams(req.term, req.page, this.pageSize);

          return this.http.get<any[]>(this.searchUrl, { params,headers:new HttpHeaders({ 'X-Skip-Loading': 'true' }) }).pipe(
            map(res => ({ req, res: res ?? [] })),
            catchError(() => of({ req, res: [] as any[] })),
            finalize(() => (this.isLoading = false))
          );
        }),
        takeUntil(this.destroy$)
      )
      .subscribe(({ req, res }) => this.applyResults(req, res));

    this.resetDataset(true);
    if (!this.requireParent || !this.isParentMissing()) {
      this.load({ term: '', page: 0, append: false });
    }
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['preloadedOptions']) {
      this.options = this.mergeWithSelected(this.preloadedOptions);
    }

    if (changes['parentId'] && !changes['parentId'].firstChange) {
      this.value = null;
      this.onChange(null);
      this.resetDataset(true);

      if (!this.requireParent || !this.isParentMissing()) {
        this.load({ term: '', page: 0, append: false });
      }
    }
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

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
      this.options = this.mergeWithSelected([]);
      this.hasMore = false;
      return;
    }
    this.emptyMessage = '';
    this.reloadFirstPage(term);
  }

  onLazyLoad(event: { first?: number; rows?: number }): void {
    if (this.isLoading || !this.hasMore) return;
    if (this.requireParent && this.isParentMissing()) return;

    const first = event?.first ?? 0;
    const rows = event?.rows ?? this.pageSize;
    const lastVisibleIndex = first + rows;
    const nearEnd = lastVisibleIndex >= this.options.length - 2;
    if (!nearEnd) return;
    const nextPage = this.pageIndex + 1;
    const key = this.requestKey(nextPage, this.currentTerm);
    if (this.requestedPages.has(key)) return;
    this.requestedPages.add(key);
    this.load({ term: this.currentTerm, page: nextPage, append: true });
  }

  handleChange(event: any): void {
    const newVal = event?.value;
    this.value = newVal;
    this.onChange(newVal);
    this.onTouched();
  }

  onClearSelection(): void {
    this.value = null;
    this.onChange(null);
    this.onTouched();

    if (!this.requireParent || !this.isParentMissing()) {
      this.reloadFirstPage('');
    }
  }

  private reloadFirstPage(term: string): void {
    this.resetDataset(true);
    this.currentTerm = term;
    this.requestedPages.add(this.requestKey(0, term));
    this.load({ term, page: 0, append: false });
  }

  private resetDataset(clearOptions: boolean): void {
    this.requestedPages.clear();
    this.pageIndex = 0;
    this.currentTerm = '';
    this.hasMore = true;

    if (clearOptions) {
      this.options = this.mergeWithSelected([]);
    }
  }

  private load(req: LoadRequest): void {
    if (!this.searchUrl) return;
    if (this.requireParent && this.isParentMissing()) return;

    this.currentTerm = (req.term ?? '').trim();
    this.pageIndex = req.page;

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

  private buildParams(term: string, pageIndex: number, pageSize: number): HttpParams {
    let params = new HttpParams();

    if (typeof this.value !== 'object' && this.value !== null && this.value !== undefined && this.value !== '') {
      params = params.set(this.idParamName, String(this.value));
    }

    const q = (term ?? '').trim();
    if (q) params = params.set(this.searchParamName, q);

    if (this.parentId !== null && this.parentId !== undefined && this.parentParamName) {
      params = params.set(this.parentParamName, String(this.parentId));
    }

    params = params.set('pageIndex', String(pageIndex));
    params = params.set('pageSize', String(pageSize));

    return params;
  }

  private applyResults(req: LoadRequest, res: any[]): void {
    const next = res ?? [];
    const merged = req.append ? this.mergeById([...this.options, ...next]) : this.mergeById(next);
    this.options = this.mergeWithSelected(merged);
    this.hasMore = next.length === this.pageSize;
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
}
