import { CommonModule } from '@angular/common';
import { Component, computed, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { CandidateUsersService } from './services/candidate-users.service';
import { CandidateUserDto } from './models/candidate-user.dto';
import { CandidateUserFilters } from './models/candidate-user-filters.dto';
import { PaginatedResult } from '../../../../core/models/paginated-result.model';
import { PaginationMetadata } from '../../../../core/models/pagination-metadata.model';
import { PaginationComponent } from '../../../../shared/components/pagination/pagination.component';
import { I18nNamespaceDirective } from '../../../../shared/directives/i18n-namespace.directive';
import { NotificationService } from '../../../../core/services/notification.service';
import { Lang, LanguageService } from '../../../../core/services/language.service';
import { Permissions } from '../../../../core/constants/permissions';
import { ProfileStatusNumber } from '../../../../core/enums/lookups.enum';
import { HasPermissionDirective } from '../../../../shared/directives/has-permission.directive';
import { debounceTime, distinctUntilChanged, Subject } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-candidate-users-management',
  standalone: true,
  templateUrl: './candidate-users-management.page.html',
  styleUrls: ['./candidate-users-management.page.scss'],
  imports: [
    CommonModule,
    FormsModule,
    TranslatePipe,
    TableModule,
    ButtonModule,
    PaginationComponent,
    I18nNamespaceDirective,
    HasPermissionDirective
  ]
})
export class CandidateUsersManagementPage implements OnInit {
  private candidateUsersService = inject(CandidateUsersService);
  private notification = inject(NotificationService);
  private translate = inject(TranslateService);
  private language = inject(LanguageService);
  private destroyRef = inject(DestroyRef);

  private _users = signal<CandidateUserDto[]>([]);
  private _paginationMetadata = signal<PaginationMetadata | null>(null);

  users = this._users.asReadonly();
  paginationMetadata = this._paginationMetadata.asReadonly();

  filters = signal<CandidateUserFilters>({
    pageNumber: 1,
    pageSize: 10,
    name: '',
    email: '',
    qid: '',
    mobileNumber: ''
  });

  nameFilter = '';
  emailFilter = '';
  qidFilter = '';
  mobileFilter = '';
  private searchChanges$ = new Subject<string>();

  currentLang = signal<Lang>(this.language.get());
  isRtl = computed(() => this.currentLang() === 'ar');
  totalItems = computed(() => this.paginationMetadata()?.totalCount || 0);

  protected readonly Permissions = Permissions;
  protected readonly ProfileStatus = ProfileStatusNumber;

  ngOnInit(): void {
    this.setupSearchListener();
    this.loadUsers();
    this.language.current$.subscribe(lang => this.currentLang.set(lang));
  }

  loadUsers(): void {
    this.candidateUsersService.getCandidateUsers(this.filters()).subscribe({
      next: (response: PaginatedResult<CandidateUserDto>) => {
        this._users.set(response.items);
        this._paginationMetadata.set(response.metadata);

        if (response.metadata) {
          this.filters.update(f => ({
            ...f,
            pageNumber: response.metadata.currentPage,
            pageSize: response.metadata.pageSize
          }));
        }
      }
    });
  }

  onSearchChange(): void {
    this.searchChanges$.next(`${this.nameFilter}|${this.emailFilter}|${this.qidFilter}|${this.mobileFilter}`);
  }

  private setupSearchListener(): void {
    this.searchChanges$
      .pipe(debounceTime(1000), distinctUntilChanged(), takeUntilDestroyed(this.destroyRef))
      .subscribe(() => {
        this.filters.update(f => ({
          ...f,
          pageNumber: 1,
          name: this.nameFilter,
          email: this.emailFilter,
          qid: this.qidFilter,
          mobileNumber: this.mobileFilter
        }));
        this.loadUsers();
      });
  }

  onPageChange(page: number): void {
    this.filters.update(f => ({ ...f, pageNumber: page }));
    this.loadUsers();
  }

  onPageSizeChange(size: number): void {
    this.filters.update(f => ({ ...f, pageSize: size, pageNumber: 1 }));
    this.loadUsers();
  }

  toggleBlock(user: CandidateUserDto): void {
    const desiredState = !user.isBlocked;
    this.candidateUsersService.updateBlockStatus(user.id, desiredState).subscribe({
      next: () => {
        this._users.update(users =>
          users.map(item => (item.id === user.id ? { ...item, isBlocked: desiredState } : item))
        );
        this.notification.success(
          this.translate.instant(
            desiredState ? 'CANDIDATE_USERS.BLOCK_SUCCESS' : 'CANDIDATE_USERS.UNBLOCK_SUCCESS'
          )
        );
      }
    });
  }
}
