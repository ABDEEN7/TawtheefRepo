import { Component, computed, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { UsersService } from './services/users.service';
import { UserDto } from './models/user.dto';
import { UserFilters } from './models/user-filters.dto';
import { Select } from 'primeng/select';
import { IconFieldModule } from 'primeng/iconfield';
import { InputIconModule } from 'primeng/inputicon';
import { InputTextModule } from 'primeng/inputtext';
import { RoleSummaryDto } from './models/role-summary.dto';
import { Tooltip } from 'primeng/tooltip';
import { DialogService } from 'primeng/dynamicdialog';
import { ManageRolesDialogComponent } from './dialogs/manage-roles-dialog/manage-roles-dialog.component';
import { Lang, LanguageService } from '../../../../core/services/language.service';
import { PaginationComponent } from '../../../../shared/components/pagination/pagination.component';
import { I18nNamespaceDirective } from '../../../../shared/directives/i18n-namespace.directive';
import { NotificationService } from '../../../../core/services/notification.service';
import { SystemRoles } from '../../../../core/constants/systemRoles';
import { PaginatedResult } from '../../../../core/models/paginated-result.model';
import { PaginationMetadata } from '../../../../core/models/pagination-metadata.model';
import { debounceTime, distinctUntilChanged, Subject } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { PageFiltersComponent } from '../../../../shared/components/page-filters/page-filters.component';

@Component({
  selector: 'app-users-management',
  standalone: true,
  templateUrl: './users-management.html',
  styleUrls: ['./users-management.scss'],
  imports: [
    CommonModule,
    FormsModule,
    TranslatePipe,
    PaginationComponent,
    I18nNamespaceDirective,
    Select,
    IconFieldModule,
    InputIconModule,
    InputTextModule,
    PageFiltersComponent,
    Tooltip,
  ],
  providers: [DialogService],
})
export class UsersManagement implements OnInit {
  private usersService = inject(UsersService);
  private dialogService = inject(DialogService);
  private notification = inject(NotificationService);
  private translate = inject(TranslateService);
  private language = inject(LanguageService);
  private destroyRef = inject(DestroyRef);

  private _users = signal<UserDto[]>([]);
  private _paginationMetadata = signal<PaginationMetadata | null>(null);

  public users = this._users.asReadonly();
  public paginationMetadata = this._paginationMetadata.asReadonly();

  filters = signal<UserFilters>({
    pageNumber: 1,
    pageSize: 10,
    search: null,
    isBlocked: null,
  });

  roleLookups = signal<RoleSummaryDto[]>([]);
  private searchChanges$ = new Subject<string>();
  currentLang = signal<Lang>(this.language.get());
  isRtl = computed(() => this.currentLang() === 'ar');

  totalItems = computed(() => this.paginationMetadata()?.totalCount || 0);
  activeFilterCount = computed(() => Number(this.filters().isBlocked != null));
  blockedStatusOptions = [
    { id: false, name: 'USERS.BLOCKED_NO' },
    { id: true, name: 'USERS.BLOCKED_YES' }
  ];

  ngOnInit(): void {
    this.setupSearchListener();
    this.loadUsers();
    this.loadRoleLookups();
    let isInitialLanguage = true;
    this.language.current$
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(lang => {
        this.currentLang.set(lang);
        if (isInitialLanguage) {
          isInitialLanguage = false;
          return;
        }

        this.loadUsers();
      });
  }

  loadUsers() {
    this.usersService.getUsers(this.filters()).subscribe({
      next: (response: PaginatedResult<UserDto>) => {
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

  loadRoleLookups() {
    this.usersService.getRoleLookups().subscribe({
      next: roles => this.roleLookups.set(roles)
    });
  }

  onSearchChange(search: string) {
    this.filters.update(f => ({ ...f, search }));
    this.searchChanges$.next(search);
  }

  private setupSearchListener() {
    this.searchChanges$
      .pipe(debounceTime(400), distinctUntilChanged(), takeUntilDestroyed(this.destroyRef))
      .subscribe(search => {
        this.filters.update(f => ({
          ...f,
          pageNumber: 1,
          search: search.trim() || null,
        }));
        this.loadUsers();
      });
  }

  clearFilters() {
    this.filters.update(f => ({ ...f, pageNumber: 1, search: null, isBlocked: null }));
    this.loadUsers();
  }

  onPageChange(page: number) {
    this.filters.update(f => ({ ...f, pageNumber: page }));
    this.loadUsers();
  }

  onPageSizeChange(pageSize: number) {
    this.filters.update(f => ({ ...f, pageSize, pageNumber: 1 }));
    this.loadUsers();
  }

  openManageRoles(user: UserDto) {
    const ref = this.dialogService.open(ManageRolesDialogComponent, {
      header: this.translate.instant('USERS.MANAGE_ROLES_TITLE'),
      width: '900px',
      styleClass: 'manage-roles-dialog',
      draggable: false,   // ✅ disables dragging
      data: { user, roleOptions: this.roleLookups() }
    });
    ref?.onClose.subscribe((updated: boolean) => {
      if (updated) {
        this.loadUsers();
      }
    });
  }

  toggleBlock(user: UserDto) {
    const desiredState = !user.isBlocked;
    this.usersService.updateBlockStatus(user.id, desiredState)
      .subscribe({
        next: () => {
          // Update local state
          this._users.update(users =>
            users.map(u => u.id === user.id ? { ...u, isBlocked: desiredState } : u)
          );
          this.notification.success(this.translate.instant(desiredState ? 'USERS.BLOCK_SUCCESS' : 'USERS.UNBLOCK_SUCCESS'));
        }
      });
  }

  onBlockedFilterChange(value: boolean | null) {
    this.filters.update(f => ({ ...f, pageNumber: 1, isBlocked: value ?? null }));
    this.loadUsers();
  }

  hasSystemAdminRole(user: UserDto) {
    const namesFromRoles = (user.roles ?? []).map(r => r.systemName || r.nameEn || r.nameAr);
    const names = [...namesFromRoles, ...(user.roleNames ?? [])];

    return names.some(name => name === SystemRoles.SystemAdmin);
  }
}
