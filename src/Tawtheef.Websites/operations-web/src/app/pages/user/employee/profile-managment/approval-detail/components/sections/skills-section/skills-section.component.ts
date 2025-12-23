import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { CardModule } from 'primeng/card';
import { TagModule } from 'primeng/tag';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-profile-approval-skills-section',
  standalone: true,
  imports: [CommonModule, TranslateModule, CardModule, TagModule],
  templateUrl: './skills-section.component.html',
  styleUrls: ['../../../profile-approval-detail.page.scss'],
})
export class SkillsSectionComponent {
  @Input() skills?: any[] | null = null;

  displayOption(option: any, fallback?: string | number | null): string {
    if (!option && !fallback) return '-';

    const display = option?.name || option?.backendName || option?.description || fallback;
    return display || '-';
  }
}
