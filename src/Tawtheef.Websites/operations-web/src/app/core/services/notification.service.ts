import {Injectable} from '@angular/core';
import {MessageService} from 'primeng/api';
import {Subject} from 'rxjs';

export type Toast = { type: 'success'|'error'|'info'|'warning', message: string, id?: string };

@Injectable({ providedIn: 'root' })
export class NotificationService {
  private subject = new Subject<Toast>();
  public toast$ = this.subject.asObservable();

  constructor(private messageService: MessageService) {}

  success(message: string) { this.show('success', message); }
  error(message: string) { this.show('error', message); }
  info(message: string) { this.show('info', message); }
  warn(message: string) { this.show('warning', message); }

  private show(type: Toast['type'], message: string) {
    this.subject.next({ type, message });
    this.messageService.add({ severity: type, detail: message });
  }
}
