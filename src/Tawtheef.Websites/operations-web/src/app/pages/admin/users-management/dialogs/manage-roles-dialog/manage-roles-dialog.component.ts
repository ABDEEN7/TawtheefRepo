import {Component, OnInit, inject, signal} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';
import {DynamicDialogConfig, DynamicDialogRef} from 'primeng/dynamicdialog';
import {TranslatePipe} from '@ngx-translate/core';
import {MultiSelectModule} from 'primeng/multiselect';
import {UserDto} from '../../models/user.dto';
import {RoleSummaryDto} from '../../models/role-summary.dto';
import {UsersService} from '../../services/users.service';
import {I18nNamespaceDirective} from '../../../../../shared/directives/i18n-namespace.directive';
import {UserRolesResponse} from '../../models/user-roles-response.dto';
import {finalize} from 'rxjs/operators';

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

  user: UserDto | undefined = this.config.data?.user as UserDto | undefined;
  roleOptions = signal<RoleSummaryDto[]>([]);
  selectedRoleIds = signal<string[]>([]);
  isLoading = signal(false);

  ngOnInit(): void {
    if (!this.user) {
      this.dialogRef.close();
      return;
    }

    this.isLoading.set(true);
    this.usersService.getUserRoles(this.user.id)
      .pipe(finalize(() => this.isLoading.set(false)))
      .subscribe((res: UserRolesResponse) => {
        this.user = {
          id: res.userId,
          name: res.name,
          email: res.email,
          isBlocked: res.isBlocked,
          lastLoginDate: res.lastLoginDate
        };

        this.roleOptions.set(res.roles || []);
        this.selectedRoleIds.set(res.assignedRoleIds?.map(id => id.toString()) || []);
      });
  }

  onRolesChange(roleIds: string[]) {
    this.selectedRoleIds.set(roleIds);
  }

  saveRoles() {
    if (!this.user) return;

    this.usersService.updateUserRoles(this.user.id, this.selectedRoleIds())
      .subscribe(() => this.dialogRef.close(true));
  }

  cancel() {
    this.dialogRef.close(false);
  }
}
