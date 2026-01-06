import { ChangeDetectionStrategy, Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslatePipe } from '@ngx-translate/core';
import { ProfileStatusDto } from '../../../../../core/models/auth/auth-response.model';

@Component({
  selector: 'app-profile-training-section',
  standalone: true,
  imports: [CommonModule, TranslatePipe],
  templateUrl: './training-section.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ProfileTrainingSectionComponent {
  @Input() profile: ProfileStatusDto | null = null;
}
