import { ChangeDetectionStrategy, Component, Input, computed, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslatePipe } from '@ngx-translate/core';
import { ButtonDirective } from 'primeng/button';
import { FileRefDto, ProfileStatusDto } from '../../../../../core/models/auth/auth-response.model';
import { FileUtilsService } from '../../../../../core/utils/file-utils';

@Component({
  selector: 'app-profile-prerequisites-section',
  standalone: true,
  imports: [CommonModule, TranslatePipe, ButtonDirective],
  templateUrl: './prerequisites-section.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ProfilePrerequisitesSectionComponent {
  private readonly fileUtils = inject(FileUtilsService);

  @Input() profile: ProfileStatusDto | null = null;

  files = computed(() => {
    const p = this.profile;
    if (!p) return [] as { key: string; titleKey: string; file: FileRefDto | null }[];
    return [
      { key: 'resumeAttachment', titleKey: 'profileOverview.attachments.resume', file: p.resumeAttachment ?? null },
      { key: 'nationalCard', titleKey: 'profileOverview.attachments.nationalCard', file: p.nationalCard ?? null },
      { key: 'residenceAddressCertificate', titleKey: 'profileOverview.attachments.residenceAddressCertificate', file: p.residenceAddressCertificate ?? null },
      { key: 'birthdayCertificate', titleKey: 'profileOverview.attachments.birthdayCertificate', file: p.birthdayCertificate ?? null },
      { key: 'marriageCertificate', titleKey: 'profileOverview.attachments.marriageCertificate', file: p.marriageCertificate ?? null },
      { key: 'sponsorCard', titleKey: 'profileOverview.attachments.sponsorCard', file: p.sponsorCard ?? null }
    ];
  });

  open(file: FileRefDto | null | undefined) {
    if (!file) return;
    this.fileUtils.previewUrl(file.url ?? '');
  }
}
