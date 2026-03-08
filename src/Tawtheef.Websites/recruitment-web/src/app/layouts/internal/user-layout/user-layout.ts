import { Component, OnDestroy, OnInit, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Navbar } from '../navbar/navbar';
import { Footer } from '../footer/footer';
import { I18nNamespaceDirective } from '../../../shared/directives/i18n-namespace.directive';
import { AuthService } from '../../../core/auth/auth.service';
import { DialogService, DynamicDialogModule } from 'primeng/dynamicdialog';
import { TranslateService } from '@ngx-translate/core';
import { TermsAgreementDialogComponent } from '../../../shared/dialogs/terms-agreement-dialog/terms-agreement-dialog.component';
import { take } from 'rxjs/operators';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-user-layout',
  imports: [
    RouterOutlet,
    Navbar,
    Footer,
    I18nNamespaceDirective,
    DynamicDialogModule
  ],
  templateUrl: './user-layout.html',
  styleUrl: './user-layout.scss',
  providers: [DialogService]
})
export class UserLayout implements OnInit, OnDestroy {
  private auth = inject(AuthService);
  private dialog = inject(DialogService);
  private translate = inject(TranslateService);

  private termsDialogOpen = false;
  private bootstrapSubscription: Subscription | null = null;

  ngOnInit(): void {
    const user = this.auth.getCurrentUser();
    if (user && user.agreedToTerms === false) {
      this.openTermsDialog();
    }
  }

  ngOnDestroy(): void {
    this.bootstrapSubscription?.unsubscribe();
  }

  private openTermsDialog(): void {
    if (this.termsDialogOpen) return;
    this.termsDialogOpen = true;
    this.dialog.open(TermsAgreementDialogComponent, {
      header: this.translate.instant('layout.internal.termsDialog.title'),
      width: '520px',
      contentStyle: { 'border-radius': '12px' },
      dismissableMask: false,
      draggable: false,
      closable: false
    })?.onClose
      .pipe(take(1))
      .subscribe(() => {
        this.termsDialogOpen = false;
      });
  }
}
