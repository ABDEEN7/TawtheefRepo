import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { CardModule } from 'primeng/card';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-profile-approval-photo-section',
  standalone: true,
  imports: [CommonModule, TranslateModule, CardModule],
  templateUrl: './photo-section.component.html',
  styleUrls: ['../../../profile-approval-detail.page.scss'],
})
export class PhotoSectionComponent {
  @Input() profilePhoto: string | null = null;
}
