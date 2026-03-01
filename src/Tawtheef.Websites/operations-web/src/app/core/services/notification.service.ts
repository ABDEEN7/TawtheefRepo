import {inject, Injectable, NgZone} from '@angular/core';
import {MessageService} from 'primeng/api';
import {Subject} from 'rxjs';
import {TranslateModule, TranslateService} from '@ngx-translate/core';


type Severity = 'success' | 'info' | 'warn' | 'error';
@Injectable({ providedIn: 'root' })
export class NotificationService {
  private messageService = inject(MessageService);
  private zone = inject(NgZone);
  private translate = inject(TranslateService);


  success(detail: string, summary = this.translate.instant('common.success')){
    this.show('success', summary, detail);
  }
  error(detail: string, summary = this.translate.instant('common.error')){
    this.show('error', summary, detail);
  }
  info(detail: string, summary = this.translate.instant('common.info')){
    this.show('info', summary, detail);
  }
  warn(detail: string, summary = this.translate.instant('common.warning')){
    this.show('warn', summary, detail);
  }

  private show(severity: Severity, summary: string, detail: string) {
    this.zone.run(() => {
      this.messageService.add({ severity, summary, detail, life: 4000 });
    });
  }
}
