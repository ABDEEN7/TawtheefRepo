import {Component, computed, inject, OnInit, signal} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import {UsersService} from './services/users.service';
import {UserDto} from './models/user.dto';
import {UserFilters} from './models/user-filters.dto';
import {Select} from 'primeng/select';
import {RoleSummaryDto} from './models/role-summary.dto';
import {Tooltip} from 'primeng/tooltip';
import {DialogService} from 'primeng/dynamicdialog';
import {ManageRolesDialogComponent} from './dialogs/manage-roles-dialog/manage-roles-dialog.component';
import {Lang, LanguageService} from '../../../../core/services/language.service';
import {PaginationComponent} from '../../../../shared/components/pagination/pagination.component';
import {I18nNamespaceDirective} from '../../../../shared/directives/i18n-namespace.directive';
import {NotificationService} from '../../../../core/services/notification.service';
import {SystemRoles} from '../../../../core/constants/systemRoles';

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

  users = this.usersService.users;
  paginationMetadata = this.usersService.paginationMetadata;

  filters = signal<UserFilters>({
    pageNumber: 1,
    pageSize: 10,
    name: '',
    email: '',
    isBlocked: undefined
  });

  nameFilter = '';
  emailFilter = '';
  roleLookups = signal<RoleSummaryDto[]>([]);
  currentLang = signal<Lang>(this.language.get());
  isRtl = computed(() => this.currentLang() === 'ar');

  totalItems = computed(() => this.paginationMetadata()?.totalCount || 0);
  blockedStatusOptions = [
    { id: false, name: 'USERS.BLOCKED_NO' },
    { id: true, name: 'USERS.BLOCKED_YES' }
  ];

  ngOnInit(): void {
    this.loadUsers();
    this.loadRoleLookups();
    this.language.current$.subscribe(lang => this.currentLang.set(lang));
  }

  loadUsers() {
    this.usersService.getUsers(this.filters()).subscribe({
      next: () => {
        const metadata = this.paginationMetadata();
        if (metadata) {
          this.filters.update(f => ({
            ...f,
            pageNumber: metadata.currentPage,
            pageSize: metadata.pageSize
          }));
        }
      },
      error: () => this.notification.error(this.translate.instant('USERS.LOAD_FAILED'))
    });
  }

  loadRoleLookups() {
    this.usersService.getRoleLookups().subscribe({
      next: roles => this.roleLookups.set(roles),
      error: () => this.notification.error(this.translate.instant('USERS.LOAD_FAILED'))
    });
  }

  onSearchChange() {
    this.filters.update(f => ({
      ...f,
      pageNumber: 1,
      name: this.nameFilter,
      email: this.emailFilter
    }));
    this.loadUsers();
  }

  onPageChange(page: number) {
    this.filters.update(f => ({...f, pageNumber: page}));
    this.loadUsers();
  }

  openManageRoles(user: UserDto) {
    const ref = this.dialogService.open(ManageRolesDialogComponent, {
      header: this.translate.instant('USERS.MANAGE_ROLES_TITLE'),
      width: '900px',
      styleClass: 'manage-roles-dialog',
      data: { user, roleOptions: this.roleLookups()  }
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
          user.isBlocked = desiredState;
          this.notification.success(this.translate.instant(desiredState ? 'USERS.BLOCK_SUCCESS' : 'USERS.UNBLOCK_SUCCESS'));
          this.loadUsers();
        },
        error: () => {
          this.notification.error(this.translate.instant(desiredState ? 'USERS.BLOCK_FAILED' : 'USERS.UNBLOCK_FAILED'));
        }
      });
  }

  onBlockedFilterChange(value: boolean | null) {
    this.filters.update(f => ({...f, isBlocked: value ?? null}));
    this.onSearchChange();
  }

  localizedRole(role: RoleSummaryDto) {
    return this.currentLang() === 'ar'
      ? role.nameAr || role.nameEn
      : role.nameEn || role.nameAr;
  }

  userRoles(user: UserDto) {
    const roles = user.roles ?? [];

    if (roles.length) {
      return roles.map(r => ({
        display: this.localizedRole(r),
        isSystemRole: !!r.isSystemRole
      }));
    }

    return (user.roleNames ?? []).map(name => ({ display: name, isSystemRole: false }));
  }

  hasSystemAdminRole(user: UserDto) {
    const namesFromRoles = (user.roles ?? []).map(r => r.systemName || r.nameEn || r.nameAr);
    const names = [...namesFromRoles, ...(user.roleNames ?? [])];

    return names.some(name => name === SystemRoles.SystemAdmin);
  }
}
