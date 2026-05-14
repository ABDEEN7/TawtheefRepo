import { inject, Injectable, NgZone } from '@angular/core';
import { MessageService } from 'primeng/api';
import { TranslateService } from '@ngx-translate/core';

type Severity = 'success' | 'info' | 'warn' | 'error';

@Injectable({ providedIn: 'root' })
export class NotificationService {
  private messageService = inject(MessageService);
  private zone = inject(NgZone);
  private translate = inject(TranslateService);

  private readonly defaultLife = 4000;

  success(
    detail: string,
    summary = this.translate.instant('common.success'),
    life = this.defaultLife
  ) {
    this.show('success', summary, detail, life);
  }

  error(
    detail?: string,
    summary = this.translate.instant('common.error'),
    life = this.defaultLife
  ) {
    this.show(
      'error',
      summary,
      detail ?? this.translate.instant('server-error.UN_EXPECTED_ERROR'),
      life
    );
  }

  info(
    detail: string,
    summary = this.translate.instant('common.info'),
    life = this.defaultLife
  ) {
    this.show('info', summary, detail, life);
  }

  warn(
    detail: string,
    summary = this.translate.instant('common.warning'),
    life = this.defaultLife
  ) {
    this.show('warn', summary, detail, life);
  }

  private show(
    severity: Severity,
    summary: string,
    detail: string,
    life: number
  ) {
    this.zone.run(() => {
      this.messageService.add({
        severity,
        summary,
        detail,
        life,
      });
    });
  }
}