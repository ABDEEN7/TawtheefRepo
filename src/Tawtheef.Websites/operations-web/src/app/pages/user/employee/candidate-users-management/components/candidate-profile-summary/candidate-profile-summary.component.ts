import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { ProfileApprovalService } from '../../../profile-managment/approval-list/services/profile-approval.service';
import { ProfileApprovalDetail, FileRefDto } from '../../../profile-managment/approval-list/models/profile-approval.models';
import { FileUtilsService } from '../../../../../../core/utils/file-utils';
import { LanguageService } from '../../../../../../core/services/language.service';
import { CandidateUsersService } from '../../services/candidate-users.service';
import { routes } from '../../../../../../routes/routes';
import { FaDirArrowDirective } from '../../../../../../shared/directives/dir-arrow.directive';
import { I18nNamespaceDirective } from '../../../../../../shared/directives/i18n-namespace.directive';

@Component({
  selector: 'app-candidate-profile-summary',
  standalone: true,
  imports: [
    CommonModule,
    TranslatePipe,
    TableModule,
    ButtonModule,
    ProgressSpinnerModule,
    I18nNamespaceDirective,
    FaDirArrowDirective
  ],
  templateUrl: './candidate-profile-summary.component.html',
  styleUrls: ['./candidate-profile-summary.component.scss']
})
export class CandidateProfileSummaryComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private api = inject(ProfileApprovalService);
  private candidateUsersService = inject(CandidateUsersService);
  private fileUtils = inject(FileUtilsService);
  private language = inject(LanguageService);
  private translate = inject(TranslateService);

  loading = signal(false);
  detail = signal<ProfileApprovalDetail | null>(null);
  error = signal<string | null>(null);

  isRtl = computed(() => this.language.get() === 'ar');
  routes = routes;

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      const profileId = params.get('profileId');
      const email = this.route.snapshot.queryParamMap.get('email');
      if (profileId) {
        this.loadProfile(profileId, email);
      }
    });
  }

  loadProfile(id: string, email?: string | null): void {
    this.loading.set(true);
    this.error.set(null);

    // Primary: use the new specialized candidate profile endpoint
    this.candidateUsersService.getCandidateProfile(id).subscribe({
      next: (res: ProfileApprovalDetail) => {
        this.detail.set(res);
        this.loading.set(false);
      },
      error: () => {
        // Fallback 1: try the standard approval detail if it's a profile ID
        this.api.getProfile(id).subscribe({
          next: (res: ProfileApprovalDetail) => {
            this.detail.set(res);
            this.loading.set(false);
          },
          error: () => {
            // Fallback 2: Search by email or ID in the approvals list
            const searchTerm = email || id;
            this.api.getProfiles({ search: searchTerm, pageNumber: 1, pageSize: 1 }).subscribe({
              next: (resList) => {
                if (resList.items && resList.items.length > 0) {
                  const realProfileId = resList.items[0].userProfileId;
                  this.api.getProfile(realProfileId).subscribe({
                    next: (resDetail) => {
                      this.detail.set(resDetail);
                      this.loading.set(false);
                    },
                    error: () => {
                      this.error.set(this.translate.instant('CANDIDATE_USERS.PROFILE_NOT_FOUND'));
                      this.loading.set(false);
                    }
                  });
                } else {
                  this.error.set(this.translate.instant('CANDIDATE_USERS.PROFILE_NOT_FOUND'));
                  this.loading.set(false);
                }
              },
              error: () => {
                this.error.set(this.translate.instant('common.loadFailed'));
                this.loading.set(false);
              }
            });
          }
        });
      }
    });
  }

  displayOption(option: any, fallback?: string | number | null): string {
    if (!option && !fallback) return '—';
    const display = option?.label || option?.name || option?.backendName || option?.description || fallback;
    return display || '—';
  }

  preview(file?: FileRefDto | null, ev?: Event): void {
    ev?.stopPropagation();
    if (file?.url) {
      this.fileUtils.previewUrl(file.url, false);
    }
  }

  back(): void {
    this.router.navigate([routes.portal.candidateUsersManagement]);
  }
}
