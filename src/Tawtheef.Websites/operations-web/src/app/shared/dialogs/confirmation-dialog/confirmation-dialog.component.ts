import { Component} from '@angular/core';
import { DynamicDialogRef, DynamicDialogConfig } from 'primeng/dynamicdialog';
import {NgClass, NgIf} from "@angular/common";
import {FormsModule} from "@angular/forms";
import {TranslatePipe} from "@ngx-translate/core";

@Component({
  selector: 'app-confirmation-dialog',
  imports: [
    NgClass,
    NgIf,
    FormsModule,
    TranslatePipe,
  ],
  templateUrl: './confirmation-dialog.component.html',
  standalone: true
})
export class ConfirmationDialogComponent {
  inputValue: string = '';

  constructor(
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig
  ) {}

  onConfirm() {
    if (this.config.data.showInputField) {
      this.ref.close(this.inputValue.trim());
    } else {
      this.ref.close(true);
    }
  }

  isConfirmDisabled(): boolean {
    if (!this.config.data.showInputField) return false;

    const value = this.inputValue.trim();
    const maxLength = this.config.data.inputMaxLength as number | undefined;
    return value.length === 0 || (maxLength !== undefined && value.length > maxLength);
  }
}
