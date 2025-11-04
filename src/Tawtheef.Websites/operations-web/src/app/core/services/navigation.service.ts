import {Injectable} from "@angular/core";
import {Router} from "@angular/router";
import {MessageService} from "primeng/api";
import {TranslateService} from "@ngx-translate/core";
import {routes} from "../../routes/routes";
import {UserType} from "../../shared/models/user-type";

@Injectable({providedIn: 'root'})
export class NavigationService {
  public redirectUrl: string | null = null;

  constructor(
    private router: Router,
    private messageService: MessageService,
    private translate: TranslateService
  ) {
  }

  navigateAfterLogin(userType: string): void {
    const returnUrl = this.getReturnUrl();
    if (returnUrl) {
      this.router.navigateByUrl(returnUrl);
    } else {
      this.redirectBasedOnRole(userType);
    }
  }

  private getReturnUrl(): string | null {
    const tree = this.router.parseUrl(this.router.url);
    return tree.queryParams['returnUrl'] || null;
  }

  redirectBasedOnRole(userType: string): void {
    if (this.isValidRole(userType)) {
      this.router.navigate([routes.dashboard(userType)]);
    } else {
      this.messageService.add({
        severity: 'error',
        summary: this.translate.instant('app.common.error'),
        detail: this.translate.instant('app.common.youDontHavePermission')
      });
      throw new Error('Invalid userModel role');
    }
  }

  private isValidRole(userType: string): boolean {
    return [UserType.Admin]
      .map(role => role.toString().toLowerCase())
      .includes(userType.toLowerCase());
  }
}
