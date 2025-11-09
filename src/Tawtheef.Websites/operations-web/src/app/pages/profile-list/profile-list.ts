import { Component, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ProfileService } from './profile-list.service';
import { ProfileListState } from './profile-list.model';
import {TranslatePipe} from '@ngx-translate/core';
import {LanguageService} from '../../core/services/language.service';

@Component({
  selector: 'app-profile-list',
  standalone: true,
  imports: [CommonModule, FormsModule, TranslatePipe],
  templateUrl: './profile-list.html',
  styleUrl: './profile-list.scss'
})
export class ProfileList {
  private profileService = inject(ProfileService);
  private language = inject(LanguageService);

  readonly state = signal<ProfileListState>({
    query: '',
    status: '',
    entity: '',
    page: 1,
    size: 10
  });

  readonly profiles = this.profileService.getProfiles();

  readonly filteredProfiles = computed(() => {
    let list = this.profiles();

    if (this.state().query) {
      const query = this.state().query;
      list = list.filter(p =>
        p.name.includes(query) ||
        p.major.includes(query) ||
        p.status.includes(query) ||
        p.id.includes(query)
      );
    }

    if (this.state().status) {
      list = list.filter(p => p.status === this.state().status);
    }

    if (this.state().entity) {
      list = list.filter(p => p.entity === this.state().entity);
    }

    return list;
  });

  readonly paginatedProfiles = computed(() => {
    const filtered = this.filteredProfiles();
    const total = filtered.length;
    const pages = Math.max(1, Math.ceil(total / this.state().size));
    const currentPage = Math.min(this.state().page, pages);
    const start = (currentPage - 1) * this.state().size;
    const end = start + this.state().size;

    return {
      total,
      pages,
      currentPage,
      slice: filtered.slice(start, end)
    };
  });

  updateState(updates: Partial<ProfileListState>): void {
    this.state.update(current => ({ ...current, ...updates }));
  }

  clearFilters(): void {
    this.state.set({
      query: '',
      status: '',
      entity: '',
      page: 1,
      size: 10
    });
  }

  previousPage(): void {
    if (this.state().page > 1) {
      this.updateState({ page: this.state().page - 1 });
    }
  }

  toggleLang(): void {
    this.language.toggle();
  }

  nextPage(): void {
    if (this.state().page < this.paginatedProfiles().pages) {
      this.updateState({ page: this.state().page + 1 });
    }
  }

  openApprovalPage(profileId: string): void {
    const url = `profile_approvals_actions_patched_finalsummary_fallback.html?profileId=${encodeURIComponent(profileId)}`;
    window.open(url, '_blank');
  }

  getDisplayRange(): string {
    const paginated = this.paginatedProfiles();
    if (paginated.slice.length === 0) return '0';

    const from = (this.state().size * (this.state().page - 1)) + 1;
    const to = (this.state().size * (this.state().page - 1)) + paginated.slice.length;
    return `${from}-${to}`;
  }
}
