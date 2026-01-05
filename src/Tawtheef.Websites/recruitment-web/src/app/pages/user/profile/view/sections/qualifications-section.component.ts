import { ChangeDetectionStrategy, Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslatePipe } from '@ngx-translate/core';
import { ProfileStatusDto } from '../../../../../core/models/auth/auth-response.model';

@Component({
  selector: 'app-profile-qualifications-section',
  standalone: true,
  imports: [CommonModule, TranslatePipe],
  templateUrl: './qualifications-section.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ProfileQualificationsSectionComponent {
  @Input() profile: ProfileStatusDto | null = null;
}
