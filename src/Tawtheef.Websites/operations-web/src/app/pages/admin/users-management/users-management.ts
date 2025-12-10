import {Component, OnInit, computed, inject, signal} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';
import {TranslatePipe} from '@ngx-translate/core';
import {UsersService} from './services/users.service';
import {UserDto, UserFilters} from './models/user.model';
import {RoleSummaryDto, UserRolesResponse} from './models/user-roles.model';
import {PaginationComponent} from '../../../shared/components/pagination/pagination.component';
import {DialogModule} from 'primeng/dialog';
import {MultiSelectModule} from 'primeng/multiselect';
import {I18nNamespaceDirective} from '../../../shared/directives/i18n-namespace.directive';

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
    DialogModule,
    MultiSelectModule,
    I18nNamespaceDirective
  ]
})
export class UsersManagement implements OnInit {
  private usersService = inject(UsersService);

  users = this.usersService.users;
  paginationMetadata = this.usersService.paginationMetadata;

  filters = signal<UserFilters>({
    pageNumber: 1,
    pageSize: 10,
    name: '',
    email: '',
    isBlocked: null
  });

  totalItems = computed(() => this.paginationMetadata()?.totalCount || 0);

  isRolesDialogOpen = signal(false);
  selectedUser = signal<UserDto | null>(null);
  roleOptions = signal<RoleSummaryDto[]>([]);
  selectedRoleIds = signal<string[]>([]);

  ngOnInit(): void {
    this.loadUsers();
  }

  loadUsers() {
    this.usersService.getUsers(this.filters());
  }

  onSearchChange() {
    this.filters.update(f => ({...f, pageNumber: 1}));
    this.loadUsers();
  }

  onPageChange(page: number) {
    this.filters.update(f => ({...f, pageNumber: page}));
    this.loadUsers();
  }

  openManageRoles(user: UserDto) {
    this.selectedUser.set(user);
    this.usersService.getUserRoles(user.id).subscribe((res: UserRolesResponse) => {
      this.roleOptions.set(res.roles || []);
      this.selectedRoleIds.set(res.assignedRoleIds || []);
      this.isRolesDialogOpen.set(true);
    });
  }

  saveRoles() {
    const userId = this.selectedUser()?.id;
    if (!userId) return;

    this.usersService.updateUserRoles(userId, this.selectedRoleIds())
      .subscribe(() => {
        this.isRolesDialogOpen.set(false);
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
}
