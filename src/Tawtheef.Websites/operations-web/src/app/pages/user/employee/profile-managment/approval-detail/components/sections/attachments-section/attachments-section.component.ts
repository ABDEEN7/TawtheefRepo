import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-profile-approval-attachments-section',
  standalone: true,
  imports: [CommonModule, TranslateModule, CardModule, ButtonModule],
  templateUrl: './attachments-section.component.html',
  styleUrls: ['../../../profile-approval-detail.page.scss'],
})
export class AttachmentsSectionComponent {
  @Input() attachments?: any[] | null = null;
  @Output() viewFile = new EventEmitter<string>();

  preview(url?: string | null): void {
    if (url) {
      this.viewFile.emit(url);
    }
  }
}
