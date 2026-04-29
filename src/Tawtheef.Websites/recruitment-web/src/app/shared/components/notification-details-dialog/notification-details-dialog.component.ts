import { Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { TranslateModule } from '@ngx-translate/core';
import { NotificationModel } from '../../../shared/models/notification.model';

@Component({
  selector: 'app-notification-details-dialog',
  standalone: true,
  imports: [CommonModule, TranslateModule],
  template: `
    <div class="modal-header border-0 pb-2 pt-3 px-3">
      <div class="d-flex align-items-center">
        <div class="icon-box me-2 rounded-3 d-flex align-items-center justify-content-center" style="width: 38px; height: 38px; background: #8a153815;">
          <i class="hgi hgi-notification-02" style="color: #8a1538; font-size: 1.4rem;"></i>
        </div>
        <div class="d-flex flex-column">
          <h5 class="modal-title fw-bold mb-1" style="color: #8a1538;">{{ notification.subject }}</h5>
          <div class="d-flex align-items-center text-muted small">
            <i class="pi pi-clock me-2"></i>
            <span>{{ notification.createdDate | date:'medium' }}</span>
          </div>
        </div>
      </div>
      <button type="button" class="btn-close shadow-none" aria-label="Close" (click)="activeModal.dismiss()"></button>
    </div>
    <div class="modal-body p-3">
      <div class="notification-card p-3 rounded-4 shadow-sm border bg-white">
        <div class="notification-text">
          {{ plainTextBody }}
        </div>
      </div>
    </div>
    <div class="modal-footer border-0 p-3 pt-0">
      <button type="button" class="btn btn-primary px-5 py-2 rounded-pill fw-semibold shadow-sm" (click)="activeModal.close()">
        {{ 'common.close' | translate }}
      </button>
    </div>
  `,
  styles: [`
    :host {
      display: block;
    }
    .modal-title {
      font-size: 1.35rem;
      letter-spacing: -0.01em;
    }
    .notification-card {
      max-height: 550px;
      overflow-y: auto;
      border: 1px solid #edf2f7 !important;
      background: #ffffff;
      scrollbar-width: thin;
      scrollbar-color: #8a1538 #f7fafc;
    }
    .notification-card::-webkit-scrollbar {
      width: 6px;
    }
    .notification-card::-webkit-scrollbar-track {
      background: #f7fafc;
    }
    .notification-card::-webkit-scrollbar-thumb {
      background-color: #8a1538;
      border-radius: 10px;
    }
    .notification-text {
      font-size: 1.1rem;
      color: #333;
      line-height: 1.5;
      white-space: pre-wrap;
      word-break: break-word;
      font-family: 'Cairo', sans-serif;
    }
    .pi-clock {
      color: #a0aec0;
      font-size: 0.85rem;
    }
  `]
})
export class NotificationDetailsDialogComponent implements OnInit {
  @Input() notification!: NotificationModel;
  plainTextBody: string = '';

  constructor(public activeModal: NgbActiveModal) { }

  ngOnInit() {
    this.plainTextBody = this.stripHtml(this.notification.body ?? "");
  }

  private stripHtml(html: string): string {
    if (!html) return "";
    
    // Decode HTML entities first using a temporary element
    const textarea = document.createElement('textarea');
    textarea.innerHTML = html;
    let decoded = textarea.value;
    
    // Replace <br> and <p> tags with newlines
    decoded = decoded.replace(/<br\s*\/?>/gi, '\n');
    decoded = decoded.replace(/<\/p>/gi, '\n');
    decoded = decoded.replace(/<p>/gi, '\n');

    // Strip remaining tags
    const doc = new DOMParser().parseFromString(decoded, 'text/html');
    const text = doc.body.textContent || doc.body.innerText || "";
    
    // Clean up excessive whitespace:
    // 1. Replace multiple spaces with a single space
    // 2. Replace three or more newlines with just two (to preserve paragraph breaks but remove huge gaps)
    // 3. Trim start and end
    return text
      .replace(/[ \t]+/g, ' ')
      .replace(/\n\s*\n\s*\n+/g, '\n\n')
      .replace(/^\s+|\s+$/g, '');
  }
}

