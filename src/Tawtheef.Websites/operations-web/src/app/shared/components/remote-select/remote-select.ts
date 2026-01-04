import { CommonModule } from '@angular/common';
import {Component, Input, OnDestroy, OnInit, forwardRef, OnChanges, SimpleChanges, inject} from '@angular/core';
import { FormsModule, NG_VALUE_ACCESSOR, ControlValueAccessor } from '@angular/forms';
import {HttpClient, HttpParams} from '@angular/common/http';
import { Subject, Subscription, of } from 'rxjs';
import { debounceTime, distinctUntilChanged, switchMap, finalize } from 'rxjs/operators';
import { SelectModule } from 'primeng/select';
import {TranslateService} from '@ngx-translate/core';

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
export class RemoteSelectComponent
  implements OnInit, OnDestroy, OnChanges, ControlValueAccessor {
  translate = inject(TranslateService);
  emptyMessage = '';
  lastQuery = '';
  // ====== API / search config ======
  @Input() searchUrl!: string;
  @Input() minChars = 3;
  @Input() debounceMs = 400;
  @Input() showClear = false;

  @Input() optionLabel = 'name';
  @Input() optionValue?: string;
  @Input() placeholder = '';
  @Input() size: "small" | "large" | undefined = undefined;
  @Input() appendTo: any = 'body';
  @Input() panelStyle: any;

  @Input() disableWhileLoading = true;
  /**
   * Preload a list of options so the component can display existing values (e.g. in edit mode)
   * without requiring the user to search again.
   */
  @Input() preloadedOptions: any[] = [];

  // ====== Parent dependency (cascading) ======
  /** ID from previous select (e.g. degreeId, countryId, etc.) */
  @Input() parentId: string | number | null | undefined;
  /** Query-string name to use for parentId, e.g. degreeId, countryId */
  @Input() parentParamName = 'parentId';
  /** If true, component won’t search until parentId is set */
  @Input() requireParent = false;

  // ====== Validation styling from parent ======
  @Input() invalid = false;

  disabled = false;
  options: any[] = [];
  isLoading = false;
  value: any;

  private search$ = new Subject<string>();
  private sub?: Subscription;

  constructor(private http: HttpClient) {}

  // ====== CVA ======
  private onChange: (value: any) => void = () => {};
  private onTouched: () => void = () => {};

  writeValue(val: any): void {
    this.value = val;
    this.options = this.mergeWithSelected(this.options);
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

  // ====== Lifecycle ======
  ngOnInit(): void {
    this.options = this.mergeWithSelected(this.preloadedOptions);

    this.sub = this.search$
      .pipe(
        debounceTime(this.debounceMs),
        distinctUntilChanged(),
        switchMap(term => {
          const query = term?.trim() ?? '';

          // Guard: no search if query is too short
          if (!query || query.length < this.minChars || !this.searchUrl) {
            this.isLoading = false;
            return of([]);
          }

          // Guard: requirement for parentId
          if (this.requireParent && (this.parentId === null || this.parentId === undefined || this.parentId === '')) {
            this.isLoading = false;
            return of([]);
          }

          this.isLoading = true;

          let params = new HttpParams().set('search', query);
          if (this.parentId !== null && this.parentId !== undefined && this.parentParamName) {
            params = params.set(this.parentParamName, String(this.parentId));
          }

          return this.http
            .get<any[]>(this.searchUrl, { params })
            .pipe(finalize(() => (this.isLoading = false)));
        })
      )
      .subscribe({
        next: res => (this.options = this.mergeWithSelected(res || [])),
        error: () => (this.options = this.mergeWithSelected([])),
      });
  }

  ngOnChanges(changes: SimpleChanges): void {
    // If parentId changed, reset value and options
    if (changes['parentId'] && !changes['parentId'].firstChange) {
      this.options = [];
      this.value = null;
      this.onChange(null);
    }

    if (changes['preloadedOptions']) {
      this.options = this.mergeWithSelected(this.preloadedOptions);
    }
  }

  ngOnDestroy(): void {
    this.sub?.unsubscribe();
  }
  onOpen() {
    // Before typing anything
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
      return this.placeholder || this.translate.instant('remote-select.select-parent');
    }
    return this.placeholder;
  }

  // ====== Events ======
  onFilter(event: any): void {
    const query = event?.filter ?? '';
    this.lastQuery = query;

    if (query.length < this.minChars) {
      this.emptyMessage = this.translate.instant('remote-select.search-hint', { minChars: this.minChars });
      this.options = [];
      return;
    }
    this.emptyMessage = '';
    this.search$.next(query);
  }

  handleChange(event: any): void {
    const newVal = event?.value;
    this.value = newVal;
    this.onChange(newVal);
    this.onTouched();
  }

  private mergeWithSelected(nextOptions: any[]): any[] {
    const merged = [...nextOptions];

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

    const fromPreloaded = this.preloadedOptions.find(
      opt => this.getOptionValue(opt) === this.value
    );
    return fromPreloaded ?? null;
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
}
