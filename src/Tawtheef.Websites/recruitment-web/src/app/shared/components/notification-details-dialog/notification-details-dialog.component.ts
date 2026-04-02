import { Component, Input, OnChanges, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { TranslateModule } from '@ngx-translate/core';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { NotificationModel } from '../../../shared/models/notification.model';

@Component({
  selector: 'app-notification-details-dialog',
  standalone: true,
  imports: [CommonModule, TranslateModule],
  template: `
    <div class="modal-header border-0 pb-0">
      <h5 class="modal-title fw-bold text-dark">{{ notification.subject }}</h5>
      <button type="button" class="btn-close" aria-label="Close" (click)="activeModal.dismiss()"></button>
    </div>
    <div class="modal-body bg-light-subtle">
      <div class="notification-body rounded-3 shadow-sm border overflow-hidden bg-white">
        <iframe
          [srcdoc]="safeBody"
          class="w-100 border-0"
          style="min-height: 450px; display: block;"
          scrolling="yes"
          title="Notification Content">
        </iframe>
      </div>
    </div>
    <div class="modal-footer border-0">
      <button type="button" class="btn btn-primary px-4" (click)="activeModal.close()">{{ 'common.close' | translate }}</button>
    </div>
  `,
  styles: [`
    .notification-body {
      background: #fff;
      min-height: 450px;
    }
    iframe {
      width: 100%;
      height: 100%;
    }
  `]
})
export class NotificationDetailsDialogComponent implements OnInit {
  @Input() notification!: NotificationModel;
  safeBody!: SafeHtml;

  constructor(public activeModal: NgbActiveModal,
    private sanitizer: DomSanitizer) { }

  ngOnInit() {
    const decoded = this.decodeHtml(this.notification.body ?? "");
    this.safeBody = this.sanitizer.bypassSecurityTrustHtml(decoded);
  }

  private decodeHtml(html: string): string {
    const textarea = document.createElement('textarea');
    textarea.innerHTML = html;
    return textarea.value;
  }
}
