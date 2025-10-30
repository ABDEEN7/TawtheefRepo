import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { UserProfile } from './user-profile.model';


const STORAGE_KEY = 'profile_data_v1';


@Injectable({ providedIn: 'root' })
export class ProfileService {
  private _profile = new BehaviorSubject<UserProfile>(this.load());
  profile$ = this._profile.asObservable();


  private load(): UserProfile {
    try {
      return JSON.parse(localStorage.getItem(STORAGE_KEY) || '{}');
    } catch {
      return {};
    }
  }


  private save() {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(this._profile.value));
  }


  update(partial: Partial<UserProfile>) {
    const updated = { ...this._profile.value, ...partial };
    this._profile.next(updated);
    this.save();
  }


  clear() {
    localStorage.removeItem(STORAGE_KEY);
    this._profile.next({});
  }
}
