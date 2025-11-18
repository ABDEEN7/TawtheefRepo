import { Component } from '@angular/core';
import { DynamicDialogRef, DynamicDialogConfig } from 'primeng/dynamicdialog';
import {TranslatePipe} from '@ngx-translate/core';
import {I18nNamespaceDirective} from '../../../../shared/directives/i18n-namespace.directive';

@Component({
  selector: 'app-confirm-apply-modal',
  standalone: true,
  templateUrl: './confirm-apply-modal.component.html',
  imports: [
    TranslatePipe,
    I18nNamespaceDirective
  ],
  styleUrls: ['./confirm-apply-modal.component.scss']
})
export class ConfirmApplyModalComponent {
  agreedToTerms: boolean = false;

  constructor(
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
  ) {}

  onAgreementChange(event: Event): void {
    const target = event.target as HTMLInputElement;
    this.agreedToTerms = target?.checked || false;
  }

  confirmApply(): void {
    if (this.agreedToTerms) {
      this.ref.close(true); // Return true when confirmed
    }
  }

  cancel(): void {
    this.ref.close(false); // Return false when canceled
  }
}
