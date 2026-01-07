import {CommonModule} from '@angular/common';
import {Component, ElementRef, inject, signal, ViewChild} from '@angular/core';
import {FormControl, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import {ButtonDirective, ButtonLabel} from 'primeng/button';
import {I18nNamespaceDirective} from '../../../../shared/directives/i18n-namespace.directive';
import {NotificationService} from '../../../../core/services/notification.service';
import {KawaderService} from './services/kawader.service';
import {KawaderUploadResult} from './models/kawader-upload.model';
import {finalize} from 'rxjs/operators';

@Component({
  selector: 'app-kawader-page',
  standalone: true,
  templateUrl: './kawader.page.html',
  styleUrls: ['./kawader.page.scss'],
  imports: [CommonModule, ReactiveFormsModule, TranslatePipe, I18nNamespaceDirective, ButtonDirective, ButtonLabel]
})
export class KawaderPage {
  private service = inject(KawaderService);
  private notifications = inject(NotificationService);
  private translate = inject(TranslateService);

  @ViewChild('fileInput') fileInput?: ElementRef<HTMLInputElement>;

  form = new FormGroup({
    file: new FormControl<File | null>(null, Validators.required),
  });

  isUploading = signal(false);
  lastResult = signal<KawaderUploadResult | null>(null);

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
        }
    });
  }

  translateReason(reason: string): string {
    const key = `kawader.errors.${reason}`;
    return this.translate.instant(key);
  }
}
