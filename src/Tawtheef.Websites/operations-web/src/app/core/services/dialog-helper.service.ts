import {Injectable} from '@angular/core';
import {DialogService} from 'primeng/dynamicdialog';
import {TranslateService} from "@ngx-translate/core";
import {ConfirmationDialogComponent} from '../../shared/dialogs/confirmation-dialog/confirmation-dialog.component';
import {ManageRolesDialogComponent} from '../../pages/admin/users-management/dialogs/manage-roles-dialog/manage-roles-dialog.component';
import {UserDto} from '../../pages/admin/users-management/models/user.dto';
import {RoleSummaryDto} from '../../pages/admin/users-management/models/role-summary.dto';

@Injectable({ providedIn: 'root' })
export class DialogHelperService {
  constructor(
    private dialogService: DialogService,
    private translateService: TranslateService
  ) {}

  openConfirmDialog(options: {
    type: 'delete' | 'submit' | 'warning';
    title: string;
    description: string;
    cancelText?: string;
    confirmText?: string;
    showInputField?: boolean;
    inputType?: 'text' | 'textarea';
    inputLabel?: string;
    inputPlaceholder?: string;
  }) {
    return this.dialogService.open(ConfirmationDialogComponent, {
      header: this.translateService.instant(options.title),
      width: '500px',
      contentStyle: { 'border-radius': '12px' },
      data: {
        ...options,
        showInputField: options.showInputField || false,
        inputType: options.inputType || 'text'
      }
    });
  }

  openManageRolesDialog(user: UserDto, roleOptions: RoleSummaryDto[]) {
    return this.dialogService.open(ManageRolesDialogComponent, {
      header: this.translateService.instant('USERS.MANAGE_ROLES_TITLE'),
      width: '900px',
      styleClass: 'manage-roles-dialog',
      data: { user, roleOptions }
    });
  }
}
