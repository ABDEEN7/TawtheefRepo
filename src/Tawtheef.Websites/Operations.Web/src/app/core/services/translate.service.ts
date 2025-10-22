import { Injectable } from '@angular/core';
import { TranslateService as NgxTranslateService } from '@ngx-translate/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class TranslateService {
  private currentLang$ = new BehaviorSubject<string>('en');
  currentLang = this.currentLang$.asObservable();

  constructor(private ngxTranslate: NgxTranslateService) {
    // Set default language
    this.ngxTranslate.setDefaultLang('en');
    this.setLanguage('en');
  }

  /**
   * Switch current language
   */
  setLanguage(lang: string): void {
    this.ngxTranslate.use(lang);
    this.currentLang$.next(lang);
    document.documentElement.lang = lang; // optional, sets HTML lang
  }

  /**
   * Get current language
   */
  getLanguage(): string {
    return this.currentLang$.value;
  }

  /**
   * Translate a key synchronously
   */
  instant(key: string, params?: any): string {
    return this.ngxTranslate.instant(key, params);
  }

  /**
   * Translate a key asynchronously (observable)
   */
  get(key: string, params?: any) {
    return this.ngxTranslate.get(key, params);
  }

  /**
   * Translate enum value by prefix
   */
  translateEnum(value: string, prefix: string = 'enum'): string {
    if (!value) return '';
    return this.ngxTranslate.instant(`${prefix}.${value}`);
  }

  /**
   * Get multiple translations at once
   */
  getMultiple(keys: string[], params?: any) {
    return this.ngxTranslate.get(keys, params);
  }
}
