import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-profile-approval-experiences-section',
  standalone: true,
  imports: [CommonModule, TranslateModule],
  templateUrl: './experiences-section.component.html',
  styleUrls: ['../../../profile-approval-detail.page.scss'],
})
export class ExperiencesSectionComponent {
  @Input() experiences: any[] | null = null;
  @Output() viewFile = new EventEmitter<string>();

  preview(url?: string | null): void {
    if (url) {
      this.viewFile.emit(url);
    }
  }
}
