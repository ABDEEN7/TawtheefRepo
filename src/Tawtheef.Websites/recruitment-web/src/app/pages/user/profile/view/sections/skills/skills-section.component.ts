import { ChangeDetectionStrategy, Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslatePipe } from '@ngx-translate/core';
import { ProfileStatusDto } from '../../../../../../core/models/auth/auth-response.model';

@Component({
  selector: 'app-profile-skills-section',
  standalone: true,
  imports: [CommonModule, TranslatePipe],
  templateUrl: './skills-section.component.html',
  styleUrls: ['./skills-section.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ProfileSkillsSectionComponent {
  @Input() profile: ProfileStatusDto | null = null;
}
