import {Component, inject} from '@angular/core';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import {ButtonDirective} from 'primeng/button';
import {DynamicDialogRef} from 'primeng/dynamicdialog';
import {finalize} from 'rxjs/operators';
import {TermsService} from '../../../core/auth/terms.service';
import {NotificationService} from '../../../core/services/notification.service';

@Component({
  selector: 'app-terms-agreement-dialog',
  standalone: true,
  imports: [
    ButtonDirective,
    TranslatePipe
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
        }
      });
  }

  close(): void {
    this.ref.close(false);
  }
}
