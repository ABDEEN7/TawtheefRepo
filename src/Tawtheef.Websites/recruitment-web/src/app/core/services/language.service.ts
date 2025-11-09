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

  /** Emits RTL state immediately and on every change */
  private isRtlSubj = new BehaviorSubject<boolean>(false);
  readonly isRtl$ = this.isRtlSubj.asObservable();

  constructor(
    private translate: TranslateService,
    @Inject(DOCUMENT) private doc: Document
  ) {
    this.translate.addLangs(['ar', 'en']);
    this.translate.setFallbackLang(DEFAULT_LANG);

    const initial = this.resolveInitialLang();
    // Apply without re-emitting current$ (we push once below)
    this.apply(initial, { emit: false });
    this.current$.next(initial);
    this.isRtlSubj.next(initial === 'ar');
  }

  /** Current language sync getter */
  get(): Lang { return this.current$.value; }
  getPrev(): Lang { return this.current$.value === 'ar' ? 'en' : 'ar'; }


  /** Is current language RTL */
  get isRtl() { return this.get() === 'ar'; }
  /** Set language and update TranslateService + <html dir/lang> + storage */
  set(lang: Lang): void {
    if (lang !== 'ar' && lang !== 'en') lang = DEFAULT_LANG;
    if (this.current$.value === lang) return;
    this.apply(lang, { emit: true });
  }

  /** Toggle between ar/en */
  toggle(): void { this.set(this.get() === 'ar' ? 'en' : 'ar'); }

  // ---- internals ----
  private resolveInitialLang(): Lang {
    const stored = (localStorage.getItem(STORAGE_KEY) || '').toLowerCase();
    if (stored === 'ar' || stored === 'en') return stored as Lang;

    const nav = (navigator.language || navigator.languages?.[0] || 'ar').toLowerCase();
    return nav.startsWith('ar') ? 'ar' : 'en';
  }

  private apply(lang: Lang, opts: { emit: boolean }): void {
    const isRtl = lang === 'ar';

    // i18n
    this.translate.use(lang);

    // <html> attributes
    const html = this.doc.documentElement;
    html.setAttribute('lang', lang);
    html.setAttribute('dir', isRtl ? 'rtl' : 'ltr');

    // <body> helper classes
    this.doc.body.classList.toggle('rtl', isRtl);
    this.doc.body.classList.toggle('ltr', !isRtl);

    // ===== Bootstrap CSS handling (supports two patterns) =====
    // Pattern A: one link with id="bs" → swap href
    const bs = this.doc.getElementById('bs') as HTMLLinkElement | null;
    if (bs) {
      bs.href = isRtl
        ? 'assets/styles/bootstrap/bootstrap.rtl.min.css'
        : 'assets/styles/bootstrap/bootstrap.min.css';
    } else {
      // Pattern B: two links with stable ids → flip media
      const ltr = this.doc.getElementById('bs-ltr') as HTMLLinkElement | null;
      const rtl = this.doc.getElementById('bs-rtl') as HTMLLinkElement | null;
      if (ltr && rtl) {
        ltr.media = isRtl ? 'not all' : 'all';
        rtl.media = isRtl ? 'all' : 'not all';
      }
      // If neither exists, we silently continue (no crash).
    }

    // persist
    try { localStorage.setItem(STORAGE_KEY, lang); } catch { /* ignore */ }

    // notify observers
    this.isRtlSubj.next(isRtl);
    if (opts.emit) this.current$.next(lang);
  }
}
