import { Injectable, inject } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { TokenService } from './token.service';
import { UserInfoModel } from '../../shared/models/user-info.model';
import { AUTH_PROVIDER } from '../constants/auth-providers.const';
import { USER_DATA_KEY } from '../constants/user-storage.const';

@Injectable({ providedIn: 'root' })
export class UserService {
  private tokenService = inject(TokenService);
  private currentUserSubject =
    new BehaviorSubject<UserInfoModel | null>(null);

  public currentUser$ =
    this.currentUserSubject.asObservable();

  constructor() { }

  getCurrentUser(): UserInfoModel | null {
    return this.currentUserSubject.value;
  }

  updateCurrentUser(
    user: UserInfoModel,
    accessToken: string
  ): void {
    const minimalUser: UserInfoModel = {
      userId: user.userId,
      email: user.email,
      fullName: user.fullName,
      profilePictureUrl: user.profilePictureUrl ?? null,
      userRoles: this.tokenService.getRolesFromToken(accessToken),
      provider: user.provider ?? AUTH_PROVIDER.LOCAL,
      notifications: user.notifications ?? 0
    };

    localStorage.setItem(
      USER_DATA_KEY,
      JSON.stringify(minimalUser)
    );

    this.currentUserSubject.next(minimalUser);
  }

  clearCurrentUser(): void {
    localStorage.removeItem(USER_DATA_KEY);
    this.currentUserSubject.next(null);
  }
}
