import { CommonModule } from '@angular/common';
import { Component, ElementRef, inject, signal, ViewChild, OnInit } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { ButtonDirective, ButtonLabel } from 'primeng/button';
import { I18nNamespaceDirective } from '../../../../shared/directives/i18n-namespace.directive';
import { NotificationService } from '../../../../core/services/notification.service';
import { KawaderService } from './services/kawader.service';
import { KawaderUploadResult } from './models/kawader-upload.model';
import { finalize, debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { HttpService } from '../../../../core/http/http.service';
import { TableLazyLoadEvent, TableModule } from 'primeng/table';
import { GetKawaderQidsRequest, KawaderQidDto } from './models/kawader-list.model';
import { IconFieldModule } from 'primeng/iconfield';
import { InputIconModule } from 'primeng/inputicon';
import { InputTextModule } from 'primeng/inputtext';
import { TagModule } from 'primeng/tag';

@Component({
  selector: 'app-kawader-page',
  standalone: true,
  templateUrl: './kawader.page.html',
  styleUrls: ['./kawader.page.scss'],
  imports: [CommonModule, ReactiveFormsModule, TranslatePipe, I18nNamespaceDirective, ButtonDirective, ButtonLabel, TableModule, IconFieldModule, InputIconModule, InputTextModule, TagModule]
})
export class KawaderPage implements OnInit {
  private service = inject(KawaderService);
  private notifications = inject(NotificationService);
  private translate = inject(TranslateService);
  private http = inject(HttpService);

  @ViewChild('fileInput') fileInput?: ElementRef<HTMLInputElement>;

  form = new FormGroup({
    file: new FormControl<File | null>(null, Validators.required),
  });

  isUploading = signal(false);
  lastResult = signal<KawaderUploadResult | null>(null);

  // Table State
  items = signal<KawaderQidDto[]>([]);
  totalRecords = signal(0);
  loading = signal(false);

  // Search Control
  searchControl = new FormControl('');

  // Pagination State
  currentPage = 1;
  pageSize = 10;

  ngOnInit() {
    this.setupSearch();
  }

  private setupSearch() {
    this.searchControl.valueChanges
      .pipe(
        debounceTime(500),
        distinctUntilChanged()
      )
      .subscribe(() => {
        this.currentPage = 1; // Reset to first page on search
        this.loadData();
      });
  }

  onLazyLoad(event: TableLazyLoadEvent) {
    if (event.first !== undefined && event.rows) {
      this.currentPage = Math.floor(event.first / event.rows) + 1;
      this.pageSize = event.rows;
    }
    this.loadData();
  }

  loadData() {
    this.loading.set(true);
    const request: GetKawaderQidsRequest = {
      pageNumber: this.currentPage,
      pageSize: this.pageSize,
      searchTerm: this.searchControl.value || undefined
    };

    this.service.getList(request)
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (response) => {
          this.items.set(response.items);
          this.totalRecords.set(response.metadata.totalCount);
        },
        error: (err) => {
          console.error('Error loading kawader data', err);
        }
      });
  }

  onFileChange(event: Event) {
    const target = event.target as HTMLInputElement;
    const file = target.files?.[0];

    this.form.patchValue({ file: file ?? null });
    this.lastResult.set(null);
  }

  reset() {
    this.form.reset();
    this.lastResult.set(null);
    if (this.fileInput?.nativeElement) {
      this.fileInput.nativeElement.value = '';
    }
  }

  downloadTemplate(): void {
    const baseUrl = 'assets/templates/MOE_Qatar_Kawader_QID_Upload_Template.xlsx';

    // Cache-buster: forces a fresh fetch every time (browser treats it as a new URL)
    const cacheBuster = Date.now();
    const assetUrl = `${baseUrl}?v=${cacheBuster}`;

    const fileName = buildSmartFileNameWithUser();

    this.http.get<Blob>(assetUrl, null, {
      responseType: 'blob',
      headers: {
        // Helpful hints (server/CDN may still override)
        'Cache-Control': 'no-cache, no-store, must-revalidate',
        'Pragma': 'no-cache',
        'Expires': '0'
      }
    }).subscribe({
      next: (blob) => this.saveBlob(blob, fileName),
    });

    function buildSmartFileNameWithUser(): string {
      const now = new Date();

      const yyyy = now.getFullYear();
      const mm = String(now.getMonth() + 1).padStart(2, '0');
      const dd = String(now.getDate()).padStart(2, '0');
      const hh = String(now.getHours()).padStart(2, '0');
      const min = String(now.getMinutes()).padStart(2, '0');

      return `MOE_Qatar_Kawader_QID_Template_${yyyy}-${mm}-${dd}_${hh}-${min}.xlsx`;
    }
  }

  private sanitizeForFileName(value: string): string {
    return value
      .trim()
      .replace(/\s+/g, '_')
      .replace(/[\\/:*?"<>|]+/g, '')
      .replace(/_+/g, '_');
  }

  private saveBlob(blob: Blob, fileName: string): void {
    const url = window.URL.createObjectURL(blob);

    const a = document.createElement('a');
    a.href = url;
    a.download = fileName;
    a.rel = 'noopener';
    a.style.display = 'none';

    document.body.appendChild(a);
    a.click();
    a.remove();

    window.URL.revokeObjectURL(url);
  }

  onSubmit() {
    if (!this.form.valid || !this.form.value.file) {
      this.form.markAllAsTouched();
      return;
    }

    this.isUploading.set(true);
    this.lastResult.set(null);

    this.service.upload(this.form.value.file)
      .pipe(finalize(() => this.isUploading.set(false)))
      .subscribe({
        next: res => {
          this.lastResult.set(res);
          const messageKey = res.success ? 'kawader.upload.success' : 'kawader.upload.partial';
          const message = this.translate.instant(messageKey);

          if (res.success) {
            this.notifications.success(message);
          } else {
            this.notifications.warn(message);
          }

          // Refresh list after upload
          this.loadData();
        }
      });
  }

  translateReason(reason: string): string {
    const key = `kawader.errors.${reason}`;
    return this.translate.instant(key);
  }
}
