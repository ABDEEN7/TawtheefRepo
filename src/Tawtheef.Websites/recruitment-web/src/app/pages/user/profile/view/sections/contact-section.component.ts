import { ChangeDetectionStrategy, Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslatePipe } from '@ngx-translate/core';
import { ProfileStatusDto } from '../../../../../core/models/auth/auth-response.model';

@Component({
  selector: 'app-profile-contact-section',
  standalone: true,
  imports: [CommonModule, TranslatePipe],
  templateUrl: './contact-section.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ProfileContactSectionComponent {
  @Input() profile: ProfileStatusDto | null = null;
}
