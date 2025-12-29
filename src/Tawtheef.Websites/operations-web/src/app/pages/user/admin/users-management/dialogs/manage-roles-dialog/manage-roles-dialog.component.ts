import {Component, OnInit, inject, signal} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';
import {DynamicDialogConfig, DynamicDialogRef} from 'primeng/dynamicdialog';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import {MultiSelectModule} from 'primeng/multiselect';
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

  user: UserDto | undefined = this.config.data?.user as UserDto | undefined;
  roleOptions = signal<RoleSummaryDto[]>(this.config.data?.roleOptions as RoleSummaryDto[] ?? []);
  selectedRoleIds = signal<string[]>([]);
  isLoading = signal(false);
  currentLang = signal<Lang>(this.language.get());
  systemAdminRoleId = signal<string | null>(null);

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

          if (this.isSystemAdminSelected(roleIds)) {
            this.notification.error(this.translate.instant('USERS.SYSTEM_ADMIN_MANAGE_DISABLED'));
            this.dialogRef.close(false);
            return;
          }

          if (this.hasMultipleSystemRoles(roleIds)) {
            this.notification.error(this.translate.instant('USERS.MULTIPLE_SYSTEM_ROLES_NOT_ALLOWED'));
            this.dialogRef.close(false);
            return;
          }

          this.selectedRoleIds.set(roleIds);
        },
        error: () => {
          this.notification.error(this.translate.instant('USERS.LOAD_FAILED'));
          this.dialogRef.close(false);
        }
      });
  }

  onRolesChange(roleIds: string[]) {
    const previous = this.selectedRoleIds();

    if (this.isSystemAdminSelected(roleIds)) {
      this.notification.error(this.translate.instant('USERS.SYSTEM_ADMIN_MANAGE_DISABLED'));
      this.selectedRoleIds.set(previous);
      return;
    }

    if (this.hasMultipleSystemRoles(roleIds)) {
      this.notification.error(this.translate.instant('USERS.MULTIPLE_SYSTEM_ROLES_NOT_ALLOWED'));
      this.selectedRoleIds.set(previous);
      return;
    }

    this.selectedRoleIds.set(roleIds);
  }

  saveRoles() {
    if (!this.user) return;

    this.usersService.updateUserRoles(this.user.id, this.selectedRoleIds())
      .subscribe({
        next: () => {
          this.notification.success(this.translate.instant('USERS.ROLES_UPDATE_SUCCESS'));
          this.dialogRef.close(true);
        },
        error: () => this.notification.error(this.translate.instant('USERS.ROLES_UPDATE_FAILED'))
      });
  }

  cancel() {
    this.dialogRef.close(false);
  }

  private initializeRoleOptions() {
    const options = this.config.data?.roleOptions as RoleSummaryDto[] ?? [];
    const systemAdminRole = options.find(r => r.systemName === SystemRoles.SystemAdmin);
    this.systemAdminRoleId.set(systemAdminRole?.id ?? null);
    this.roleOptions.set(options.filter(r => r.systemName !== SystemRoles.SystemAdmin));
  }

  private isSystemAdminSelected(roleIds: string[]) {
    const systemAdminId = this.systemAdminRoleId();
    return !!systemAdminId && roleIds.includes(systemAdminId);
  }

  private hasMultipleSystemRoles(roleIds: string[]) {
    const selectableSystemRoles = [
      ...this.roleOptions().filter(r => r.isSystemRole).map(r => r.id),
      ...(this.systemAdminRoleId() ? [this.systemAdminRoleId() as string] : [])
    ];

    const selectedSystemRoles = roleIds.filter(id => selectableSystemRoles.includes(id));
    return selectedSystemRoles.length > 1;
  }
}
