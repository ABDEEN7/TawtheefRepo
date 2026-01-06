import {inject, Injectable} from "@angular/core";
import {BehaviorSubject} from "rxjs";
import {TokenService} from "./token.service";
import {UserInfoModel} from "../../shared/models/user-info.model";
import {PrefillData} from '../models/auth/auth-response.model';
import {PREFILL_DATA_KEY, USER_DATA_KEY} from '../constants/user-storage.const';
import {AUTH_PROVIDER} from '../constants/auth-providers.const';
import {VERIFIED_PHONE_KEY} from '../constants/wizard-keys.const';

@Injectable({providedIn: 'root'})
export class UserService {
  public tokenService = inject(TokenService);
  private currentUserSubject = new BehaviorSubject<UserInfoModel | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();

  getCurrentUser(): UserInfoModel | null {
    return this.currentUserSubject.value;
  }

  updateCurrentUser(user: UserInfoModel, accessToken: string): void {
    const minimalUser = {
      userId: user.userId,
      email: user.email,
      fullName: user.fullName,
      profilePictureUrl: user.profilePictureUrl,
      agreedToTerms: !!user.agreedToTerms,
      userType: this.tokenService.getRoleFromToken(accessToken),
      provider: user.provider || AUTH_PROVIDER.LOCAL,
      notifications : user.notifications || 0
    } as UserInfoModel;

    localStorage.setItem(USER_DATA_KEY, JSON.stringify(minimalUser));
    if(user.prefill || user.prefill === null) localStorage.setItem(PREFILL_DATA_KEY, JSON.stringify(user.prefill));
    this.currentUserSubject.next(minimalUser);
  }

  getPrefill(): PrefillData | null {
    return JSON.parse(localStorage.getItem(PREFILL_DATA_KEY) ?? 'null');
  }

  clearCurrentUser(): void {
    localStorage.removeItem(USER_DATA_KEY);
    localStorage.removeItem(PREFILL_DATA_KEY);
    localStorage.removeItem(VERIFIED_PHONE_KEY);
    this.currentUserSubject.next(null);
  }

  markTermsAsAgreed(): void {
    const current = this.currentUserSubject.value;
    if (!current) return;

    const updatedUser: UserInfoModel = {
      ...current,
      agreedToTerms: true
    };

    localStorage.setItem(USER_DATA_KEY, JSON.stringify(updatedUser));
    this.currentUserSubject.next(updatedUser);
  }
}
