import { ChangeDetectionStrategy, Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslatePipe } from '@ngx-translate/core';
import { ProfileStatusDto } from '../../../../../core/models/auth/auth-response.model';

@Component({
  selector: 'app-profile-achievements-section',
  standalone: true,
  imports: [CommonModule, TranslatePipe],
  templateUrl: './achievements-section.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ProfileAchievementsSectionComponent {
  @Input() profile: ProfileStatusDto | null = null;
}
