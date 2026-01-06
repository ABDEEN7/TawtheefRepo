import {Component, inject} from '@angular/core';
import {NgIf} from '@angular/common';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import {ButtonDirective} from 'primeng/button';
import {DynamicDialogRef} from 'primeng/dynamicdialog';
import {finalize} from 'rxjs/operators';
import {TermsService} from '../../../core/auth/terms.service';
import {NotificationService} from '../../../core/services/notification.service';
import {I18nNamespaceDirective} from '../../directives/i18n-namespace.directive';

@Component({
  selector: 'app-terms-agreement-dialog',
  standalone: true,
  imports: [
    ButtonDirective,
    TranslatePipe,
    I18nNamespaceDirective,
    NgIf
  ],
  templateUrl: './terms-agreement-dialog.component.html',
  styleUrl: './terms-agreement-dialog.component.scss'
})
export class TermsAgreementDialogComponent {
  private termsService = inject(TermsService);
  private notifier = inject(NotificationService);
  private translate = inject(TranslateService);
  private ref = inject(DynamicDialogRef);

  loading = false;

  accept(): void {
    this.loading = true;
    this.termsService.agreeToTerms()
      .pipe(finalize(() => this.loading = false))
      .subscribe({
        next: () => {
          this.notifier.success(
            this.translate.instant('layout.internal.termsDialog.accepted.detail'),
            this.translate.instant('layout.internal.termsDialog.accepted.summary')
          );
          this.ref.close(true);
        },
        error: () => {
          this.notifier.error(
            this.translate.instant('common.errorGeneric'),
            this.translate.instant('layout.internal.termsDialog.title')
          );
        }
      });
  }

  close(): void {
    this.ref.close(false);
  }
}
