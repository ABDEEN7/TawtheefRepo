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


  success(detail: string, summary = 'common.success'){
    if(summary == 'common.success'){
      this.translate.get('common.success').subscribe(translated => {
        summary = translated;
        this.show('success', summary, detail);
      });
    } else {
      this.show('success', summary, detail);
    }
  }
  error(detail: string, summary = 'common.error'){
    if(summary == 'common.error'){
      this.translate.get('common.error').subscribe(translated => {
        summary = translated;
        this.show('error', summary, detail);
      });
    } else {
      this.show('error', summary, detail);
    }
  }
  info(detail: string, summary = 'common.info'){
    if(summary == 'common.info'){
      this.translate.get('common.info').subscribe(translated => {
        summary = translated;
        this.show('info', summary, detail);
      });
    } else {
      this.show('info', summary, detail);
    }
  }
  warn(detail: string, summary = 'common.warning'){
    if(summary == 'common.warning'){
      this.translate.get('common.warning').subscribe(translated => {
        summary = translated;
        this.show('warn', summary, detail);
      });
    } else {
      this.show('warn', summary, detail);
    }
  }

  private show(severity: Severity, summary: string, detail: string) {
    this.zone.run(() => {
      this.messageService.add({ severity, summary, detail, life: 4000 });
    });
  }
}
