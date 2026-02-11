import {inject, Injectable} from '@angular/core';
import {DialogService} from 'primeng/dynamicdialog';
import {TranslateService} from "@ngx-translate/core";
import {ConfirmationDialogComponent} from '../../shared/dialogs/confirmation-dialog/confirmation-dialog.component';

@Injectable({ providedIn: 'root' })
export class DialogHelperService {
  private dialogService = inject(DialogService);
  private translateService = inject(TranslateService);

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
      draggable: false,   // ✅ disables dragging
      data: {
        ...options,
        showInputField: options.showInputField || false,
        inputType: options.inputType || 'text'
      }
    });
  }
}
