import { CommonModule } from '@angular/common';
import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { HttpService } from '../../../../core/http/http.service';
import { EndpointsService } from '../../../../core/http/endpoints.service';
import { NotificationService } from '../../../../core/services/notification.service';
import { I18nNamespaceDirective } from '../../../../shared/directives/i18n-namespace.directive';

interface JobTitleItem {
  id: string;
  jobNumber: string;
  jobNameAr: string;
  jobNameEn: string;
}

@Component({
  selector: 'app-job-titles-management',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, TranslatePipe, I18nNamespaceDirective],
  templateUrl: './job-titles-management.component.html'
})
export class JobTitlesManagementComponent implements OnInit {
  private http = inject(HttpService);
  private endpoints = inject(EndpointsService);
  private notify = inject(NotificationService);
  private translate = inject(TranslateService);
  private fb = inject(FormBuilder);

  readonly items = signal<JobTitleItem[]>([]);
  readonly editingId = signal<string | null>(null);

  readonly form = this.fb.nonNullable.group({
    jobNumber: ['', Validators.required],
    jobNameAr: ['', Validators.required],
    jobNameEn: ['', Validators.required]
  });

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.http.get<JobTitleItem[]>(this.endpoints.jobTitles.list).subscribe({
      next: res => this.items.set(res)
    });
  }

  edit(item: JobTitleItem): void {
    this.editingId.set(item.id);
    this.form.patchValue(item);
  }

  reset(): void {
    this.editingId.set(null);
    this.form.reset({ jobNumber: '', jobNameAr: '', jobNameEn: '' });
  }

  save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const payload = this.form.getRawValue();
    const request = this.editingId()
      ? this.http.put(this.endpoints.jobTitles.update(this.editingId()!), payload)
      : this.http.post(this.endpoints.jobTitles.create, payload);

    request.subscribe({
      next: () => {
        this.notify.success(this.translate.instant('common.savedSuccessfully'));
        this.reset();
        this.load();
      }
    });
  }

  delete(id: string): void {
    this.http.delete(this.endpoints.jobTitles.delete(id)).subscribe({
      next: () => this.load(),
      error: () => this.notify.error(this.translate.instant('common.operationFailed'))
    });
  }
}
