import {Component, inject, OnInit, signal} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';
import {DynamicDialogConfig, DynamicDialogRef} from 'primeng/dynamicdialog';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import {MultiSelectModule} from 'primeng/multiselect';
import {Select} from 'primeng/select';
import {UserDto} from '../../models/user.dto';
import {RoleSummaryDto} from '../../models/role-summary.dto';
import {UsersService} from '../../services/users.service';
import {finalize} from 'rxjs/operators';
import {I18nNamespaceDirective} from '../../../../../../shared/directives/i18n-namespace.directive';
import {Lang, LanguageService} from '../../../../../../core/services/language.service';
import {NotificationService} from '../../../../../../core/services/notification.service';
import {SystemRoles} from '../../../../../../core/constants/systemRoles';

@Component({
  selector: 'app-manage-roles-dialog',
  standalone: true,
  templateUrl: './manage-roles-dialog.component.html',
  styleUrls: ['./manage-roles-dialog.component.scss'],
  imports: [
    CommonModule,
    FormsModule,
    TranslatePipe,
    MultiSelectModule,
    Select,
    I18nNamespaceDirective
  ]
})
export class ManageRolesDialogComponent implements OnInit {
  private usersService = inject(UsersService);
  private dialogRef = inject(DynamicDialogRef);
  private config = inject(DynamicDialogConfig);
  private language = inject(LanguageService);
  private notification = inject(NotificationService);
  private translate = inject(TranslateService);

  private readonly switchableSystemRoles = [
    SystemRoles.Employee,
    SystemRoles.HrManager,
    SystemRoles.DepartmentManager
  ];
  private readonly lockedSystemRoles = [
    SystemRoles.SystemAdmin,
    SystemRoles.OfficeAdmin,
    SystemRoles.OfficeUser
  ];

  user: UserDto | undefined = this.config.data?.user as UserDto | undefined;
  allRoleOptions = signal<RoleSummaryDto[]>(this.config.data?.roleOptions as RoleSummaryDto[] ?? []);
  allSystemRoleOptions = signal<RoleSummaryDto[]>([]);
  assignableRoleOptions = signal<RoleSummaryDto[]>([]);
  systemRoleOptions = signal<RoleSummaryDto[]>([]);
  selectedAssignableRoleIds = signal<string[]>([]);
  selectedSystemRoleId = signal<string | null>(null);
  isSystemRoleLocked = signal(false);
  showSystemRoleError = signal(false);
  isLoading = signal(false);
  currentLang = signal<Lang>(this.language.get());

  ngOnInit(): void {
    if (!this.user) {
      this.dialogRef.close();
      return;
    }

    this.initializeRoleOptions();
    this.language.current$.subscribe(lang => this.currentLang.set(lang));
    this.isLoading.set(true);
    this.usersService.getUserAssignedRoleIds(this.user.id)
      .pipe(finalize(() => this.isLoading.set(false)))
      .subscribe({
        next: (assignedRoles: string[]) => {
          const roleIds = assignedRoles.map(id => id.toString());

          const assignedSystemRole = roleIds
            .map(id => this.getRoleById(id))
            .find(role => role?.isSystemRole);
          const assignableRoles = roleIds.filter(id => {
            const role = this.getRoleById(id);
            return role && !role.isSystemRole;
          });

          const assignedSystemRoleName = assignedSystemRole?.systemName ?? '';
          const isLockedSystemRole = this.lockedSystemRoles
            .some(role => role === assignedSystemRoleName);
          const availableSystemRoles = isLockedSystemRole
            ? assignedSystemRole ? [assignedSystemRole] : []
            : this.allSystemRoleOptions().filter(role =>
              this.switchableSystemRoles.includes(role.systemName));

          this.systemRoleOptions.set(availableSystemRoles);
          this.selectedSystemRoleId.set(assignedSystemRole?.id ?? null);
          this.isSystemRoleLocked.set(isLockedSystemRole);
          this.selectedAssignableRoleIds.set(assignableRoles);
        },
        error: () => {
          this.dialogRef.close(false);
        }
      });
  }

  onRolesChange(assignableRoleIds: string[]) {
    this.selectedAssignableRoleIds.set(assignableRoleIds);
  }

  onSystemRoleChange(roleId: string | null) {
    this.selectedSystemRoleId.set(roleId);
    if (roleId) {
      this.showSystemRoleError.set(false);
    }
  }

  saveRoles() {
    if (!this.user) return;
    if (!this.selectedSystemRoleId()) {
      this.showSystemRoleError.set(true);
      return;
    }

    const roleIds = Array.from(
      new Set([this.selectedSystemRoleId(), ...this.selectedAssignableRoleIds()].filter(Boolean))
    ) as string[];

    this.usersService.updateUserRoles(this.user.id, roleIds)
      .subscribe({
        next: () => {
          this.notification.success(this.translate.instant('USERS.ROLES_UPDATE_SUCCESS'));
          this.dialogRef.close(true);
        },
        error: () => {
          this.notification.error(this.translate.instant('USERS.ROLES_UPDATE_FAILED'));
        }
      });
  }

  cancel() {
    this.dialogRef.close(false);
  }

  private initializeRoleOptions() {
    const options = this.config.data?.roleOptions as RoleSummaryDto[] ?? [];
    this.allRoleOptions.set(options);
    this.allSystemRoleOptions.set(options.filter(role => role.isSystemRole));
    this.assignableRoleOptions.set(options.filter(role => !role.isSystemRole));
    this.systemRoleOptions.set(this.allSystemRoleOptions()
      .filter(role => this.switchableSystemRoles.includes(role.systemName)));
  }

  private getRoleById(roleId: string) {
    return this.allRoleOptions().find(role => role.id === roleId);
  }
}
