import {Component, computed, inject, OnInit, signal} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';
import {TranslatePipe} from '@ngx-translate/core';
import {UsersService} from './services/users.service';
import {UserDto} from './models/user.dto';
import {UserFilters} from './models/user-filters.dto';
import {PaginationComponent} from '../../../shared/components/pagination/pagination.component';
import {I18nNamespaceDirective} from '../../../shared/directives/i18n-namespace.directive';
import {Select} from 'primeng/select';
import {DialogHelperService} from '../../../core/services/dialog-helper.service';

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
    Select
  ]
})
export class UsersManagement implements OnInit {
  private usersService = inject(UsersService);
  private dialogHelper = inject(DialogHelperService);

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

  totalItems = computed(() => this.paginationMetadata()?.totalCount || 0);
  blockedStatusOptions = [
    { id: false, name: 'USERS.BLOCKED_NO' },
    { id: true, name: 'USERS.BLOCKED_YES' }
  ];

  ngOnInit(): void {
    this.loadUsers();
  }

  loadUsers() {
    this.usersService.getUsers(this.filters());
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
    const ref = this.dialogHelper.openManageRolesDialog(user);

    ref?.onClose.subscribe((updated: boolean) => {
      if (updated) {
        this.loadUsers();
      }
    });
  }

  toggleBlock(user: UserDto) {
    const desiredState = !user.isBlocked;
    this.usersService.updateBlockStatus(user.id, desiredState)
      .subscribe(() => {
        user.isBlocked = desiredState;
        this.usersService.getUsers(this.filters());
      });
  }

  onBlockedFilterChange(value: boolean | null) {
    this.filters.update(f => ({...f, isBlocked: value ?? null}));
    this.onSearchChange();
  }
}
