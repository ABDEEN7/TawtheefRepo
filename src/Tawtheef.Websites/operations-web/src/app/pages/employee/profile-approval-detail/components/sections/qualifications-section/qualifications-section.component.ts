import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-profile-approval-qualifications-section',
  standalone: true,
  imports: [CommonModule, TranslateModule],
  templateUrl: './qualifications-section.component.html',
  styleUrls: ['../../../profile-approval-detail.page.scss'],
})
export class QualificationsSectionComponent {
  @Input() qualifications: any[] | null = null;
  @Output() viewFile = new EventEmitter<string>();

  preview(url?: string | null): void {
    if (url) {
      this.viewFile.emit(url);
    }
  }
}
