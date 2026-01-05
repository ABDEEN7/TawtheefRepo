import { ChangeDetectionStrategy, Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslatePipe } from '@ngx-translate/core';
import { ProfileStatusDto } from '../../../../../core/models/auth/auth-response.model';

@Component({
  selector: 'app-profile-personal-section',
  standalone: true,
  imports: [CommonModule, TranslatePipe],
  templateUrl: './personal-section.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ProfilePersonalSectionComponent {
  @Input() profile: ProfileStatusDto | null = null;
}
