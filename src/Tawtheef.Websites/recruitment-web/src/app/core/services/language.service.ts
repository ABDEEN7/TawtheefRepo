import { Injectable, Inject } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { TranslateService } from '@ngx-translate/core';
import { DOCUMENT } from '@angular/common';

type Lang = 'ar' | 'en';

const STORAGE_KEY = 'app.lang';
const DEFAULT_LANG: Lang = 'ar';

@Injectable({ providedIn: 'root' })
export class LanguageService {
  /** Emits current language immediately and on every change */
  readonly current$ = new BehaviorSubject<Lang>(DEFAULT_LANG);

  constructor(
    private translate: TranslateService,
    @Inject(DOCUMENT) private doc: Document
  ) {
    // configure translate
    this.translate.addLangs(['ar', 'en']);
    const initial = this.resolveInitialLang();
    this.apply(initial, { emit: false }); // don’t double-emit on startup
    this.current$.next(initial);
  }

  /** Get current language value synchronously */
  get(): Lang {
    return this.current$.value;
  }

  /** Set language and update TranslateService + <html dir/lang> + storage */
  set(lang: Lang): void {
    if (lang !== 'ar' && lang !== 'en') lang = DEFAULT_LANG;
    if (this.current$.value === lang) return;
    this.apply(lang, { emit: true });
  }

  /** Toggle between ar/en */
  toggle(): void {
    this.set(this.get() === 'ar' ? 'en' : 'ar');
  }

  // ---- internals ----
  private resolveInitialLang(): Lang {
    // localStorage
    const stored = (localStorage.getItem(STORAGE_KEY) || '').toLowerCase();
    if (stored === 'ar' || stored === 'en') return stored as Lang;

    // browser language
    const nav = (navigator.language || navigator.languages?.[0] || 'ar').toLowerCase();
    return nav.startsWith('ar') ? 'ar' : 'en';
  }

  private apply(lang: Lang, opts: { emit: boolean }): void {
    // ngx-translate
    this.translate.setFallbackLang(DEFAULT_LANG);
    this.translate.use(lang);

    // html attributes
    const html = this.doc.documentElement;
    html.setAttribute('lang', lang);
    html.setAttribute('dir', lang === 'ar' ? 'rtl' : 'ltr');

    // body class (optional, handy for CSS overrides)
    this.doc.body.classList.toggle('rtl', lang === 'ar');
    this.doc.body.classList.toggle('ltr', lang === 'en');

    // persist
    try { localStorage.setItem(STORAGE_KEY, lang); } catch { /* ignore */ }

    if (opts.emit) this.current$.next(lang);
  }
}
