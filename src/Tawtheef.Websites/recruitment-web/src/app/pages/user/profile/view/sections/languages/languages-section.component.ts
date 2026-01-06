import { ChangeDetectionStrategy, Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslatePipe } from '@ngx-translate/core';
import { ProfileStatusDto } from '../../../../../../core/models/auth/auth-response.model';

@Component({
  selector: 'app-profile-languages-section',
  standalone: true,
  imports: [CommonModule, TranslatePipe],
  templateUrl: './languages-section.component.html',
  styleUrls: ['./languages-section.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ProfileLanguagesSectionComponent {
  @Input() profile: ProfileStatusDto | null = null;
}
