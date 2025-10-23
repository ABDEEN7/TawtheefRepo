import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';


export type Toast = { type: 'success'|'error'|'info'|'warning', message: string, id?: string }


@Injectable({ providedIn: 'root' })
export class NotificationService {
  private subject = new Subject<Toast>();
  public toast$ = this.subject.asObservable();


  success(message: string) { this.subject.next({ type: 'success', message }); }
  error(message: string) { this.subject.next({ type: 'error', message }); }
  info(message: string) { this.subject.next({ type: 'info', message }); }
  warn(message: string) { this.subject.next({ type: 'warning', message }); }
}
