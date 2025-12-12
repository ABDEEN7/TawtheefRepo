import { CommonModule } from '@angular/common';
import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { ProfileOverviewService } from './services/profile-overview.service';
import { ProfileOverview } from './models/profile-overview.model';
import {I18nNamespaceDirective} from '../../../shared/directives/i18n-namespace.directive';

@Component({
  selector: 'app-profile-overview',
  standalone: true,
  imports: [CommonModule, TranslateModule, I18nNamespaceDirective],
  templateUrl: './profile-overview.page.html',
  styleUrl: './profile-overview.page.scss',
})
export class ProfileOverviewPage implements OnInit {
  private api = inject(ProfileOverviewService);

  overview = signal<ProfileOverview | null>(null);
  loading = signal(false);
  error = signal<string | null>(null);

  readonly hasPending = computed(() => this.overview()?.hasPendingChanges ?? false);

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading.set(true);
    this.error.set(null);
    this.api.getOverview().subscribe({
      next: data => {
        this.overview.set(data);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('profileOverview.error');
        this.loading.set(false);
      },
    });
  }

  sectionName(section: number): string {
    switch (section) {
      case 1:
        return 'profileOverview.sections.basicInfo';
      case 2:
        return 'profileOverview.sections.qualifications';
      case 3:
        return 'profileOverview.sections.experiences';
      case 4:
        return 'profileOverview.sections.training';
      case 5:
        return 'profileOverview.sections.certificates';
      case 6:
        return 'profileOverview.sections.skillsLanguages';
      case 7:
        return 'profileOverview.sections.attachments';
      case 8:
        return 'profileOverview.sections.profilePhoto';
      case 9:
        return 'profileOverview.sections.cv';
      default:
        return 'profileOverview.sections.generic';
    }
  }
}
