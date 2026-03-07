import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/auth/auth.service';
import { TranslatePipe } from '@ngx-translate/core';
import { I18nNamespaceDirective } from '../../../shared/directives/i18n-namespace.directive';
import { routes } from '../../../routes/routes';

@Component({
    selector: 'app-pending-approval',
    standalone: true,
    imports: [TranslatePipe, I18nNamespaceDirective],
    templateUrl: './pending-approval.html',
    styleUrl: './pending-approval.scss'
})
export class PendingApprovalComponent {
    private authService = inject(AuthService);
    private router = inject(Router);

    logout(): void {
        this.authService.logout();
    }
}
