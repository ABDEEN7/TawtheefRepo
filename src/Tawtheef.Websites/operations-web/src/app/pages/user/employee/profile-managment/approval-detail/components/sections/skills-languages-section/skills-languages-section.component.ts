import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { CardModule } from 'primeng/card';
import { TagModule } from 'primeng/tag';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-profile-approval-skills-languages-section',
  standalone: true,
  imports: [CommonModule, TranslateModule, CardModule, TagModule],
  templateUrl: './skills-languages-section.component.html',
  styleUrls: ['../../../profile-approval-detail.page.scss'],
})
export class SkillsLanguagesSectionComponent {
  @Input() skillsAndLanguages: any[] | null = null;
  @Input() languages: any[] | null = null;

  displayOption(option: any, fallback?: string | number | null): string {
    if (!option && !fallback) return '-';

    const display = option?.name || option?.backendName || option?.description || fallback;
    return display || '-';
  }
}
