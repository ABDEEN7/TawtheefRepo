import {Injectable} from "@angular/core";
import {BehaviorSubject} from "rxjs";
import {TokenService} from "./token.service";
import {UserInfoModel} from "../../shared/models/user-info.model";

@Injectable({providedIn: 'root'})
export class UserService {
  private currentUserSubject = new BehaviorSubject<UserInfoModel | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();

  constructor(private tokenService: TokenService) {
  }

  getCurrentUser(): UserInfoModel | null {
    return this.currentUserSubject.value;
  }

  updateCurrentUser(user: UserInfoModel, accessToken: string): void {
    const minimalUser = {
      userId: user.userId,
      email: user.email,
      firstName: user.firstName ?? this.tokenService.getClaim(accessToken, 'given_name'),
      lastName: user.lastName ?? this.tokenService.getClaim(accessToken, 'family_name'),
      profilePictureUrl: user.profilePictureUrl ?? this.tokenService.getClaim(accessToken, 'picture') ?? null,
      userType: this.tokenService.getRoleFromToken(accessToken),
      authProvider: user.authProvider || this.tokenService.getClaim(accessToken, 'auth_provider') || 'local',
    };

    localStorage.setItem('user_data', JSON.stringify(minimalUser));
    this.currentUserSubject.next(minimalUser);
  }

  clearCurrentUser(): void {
    localStorage.removeItem('user_data');
    this.currentUserSubject.next(null);
  }

  isPasswordlessUser(): boolean {
    const user = this.getCurrentUser();
    if (!user?.authProvider) return false;
    return ['google', 'facebook'].includes(user.authProvider);
  }
}
