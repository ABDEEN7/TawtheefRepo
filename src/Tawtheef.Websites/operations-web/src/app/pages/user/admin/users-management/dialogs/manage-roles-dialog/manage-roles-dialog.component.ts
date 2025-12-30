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
  allRoleOptions = signal<RoleSummaryDto[]>(this.config.data?.roleOptions as RoleSummaryDto[] ?? []);
  lockedSystemRoleIds = signal<string[]>([]);
  systemRoleIds = signal<string[]>([]);
  selectedAssignableRoleIds = signal<string[]>([]);
  selectedRoleIds = signal<string[]>([]);
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

          const assignedSystemRoles = roleIds.filter(id => this.isSystemRoleId(id));
          const assignableRoles = roleIds.filter(id => !this.isSystemRoleId(id));

          this.lockedSystemRoleIds.set(assignedSystemRoles);
          this.selectedAssignableRoleIds.set(assignableRoles);
          this.updateSelectedRoles(assignableRoles);
        },
        error: () => {
          this.notification.error(this.translate.instant('USERS.LOAD_FAILED'));
          this.dialogRef.close(false);
        }
      });
  }

  onRolesChange(assignableRoleIds: string[]) {
    this.selectedAssignableRoleIds.set(assignableRoleIds);
    this.updateSelectedRoles(assignableRoleIds);
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
    this.allRoleOptions.set(options);
    this.systemRoleIds.set(options.filter(role => role.isSystemRole).map(role => role.id));
  }

  private isSystemRoleId(roleId: string) {
    return this.systemRoleIds().includes(roleId);
  }

  private updateSelectedRoles(assignableRoleIds: string[]) {
    const merged = [...this.lockedSystemRoleIds(), ...assignableRoleIds];
    const uniqueIds = Array.from(new Set(merged));
    this.selectedRoleIds.set(uniqueIds);
  }
}
