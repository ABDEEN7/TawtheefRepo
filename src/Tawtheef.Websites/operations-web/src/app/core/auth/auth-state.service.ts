import {TokenService} from "./token.service";
import {Injectable} from "@angular/core";
import {BehaviorSubject} from "rxjs";
import {UserService} from "./user.service";
import {HttpClient} from "@angular/common/http";
import {Router} from "@angular/router";
import {EndpointsService} from "../http/endpoints.service";
import {routes} from "../../routes/routes";
import {HttpService} from "../http/http.service";

@Injectable({providedIn: 'root'})
export class AuthStateService {
  readonly routes = routes;
  private isAuthenticatedSubject = new BehaviorSubject<boolean>(false);
  public isAuthenticated$ = this.isAuthenticatedSubject.asObservable();

  constructor(
    private router: Router,
    private http: HttpService,
    private endpoints: EndpointsService,
    private tokenService: TokenService,
    private userService: UserService
  ) {
  }

  setAuthenticated(value: boolean): void {
    this.isAuthenticatedSubject.next(value);
  }

  checkAuthState(allowRedirectLogout: boolean): boolean {
    const token = this.tokenService.getToken();
    const userData = localStorage.getItem('user_data');

    if (token && !this.tokenService.isTokenExpired(token) && userData) {
      try {
        const user = JSON.parse(userData);
        this.userService.updateCurrentUser(user, token);
        this.setAuthenticated(true);
        return true;
      } catch (e) {
        if (allowRedirectLogout) this.logout();
        return false;
      }
    } else {
      if (allowRedirectLogout) this.logout();
      return false;
    }
  }

  logout(request: boolean = true): void {
    if(request) {
      this.http.post(this.endpoints.auth.logout, {}).subscribe({
        next: () => {
          this.tokenService.clearTokens();
          this.userService.clearCurrentUser();
          this.setAuthenticated(false);
          this.router.navigate([this.routes.home]);
        }
      });
    }
    else{
      this.tokenService.clearTokens();
      this.userService.clearCurrentUser();
      this.setAuthenticated(false);
      this.router.navigate([this.routes.home]);
    }
  }
}
