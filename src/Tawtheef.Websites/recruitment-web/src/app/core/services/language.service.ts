import {Injectable, Inject, inject} from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { TranslateService } from '@ngx-translate/core';
import { DOCUMENT } from '@angular/common';
import {PRIME_NG_CONFIG, PrimeNG} from 'primeng/config';
import {APP_LANGUAGE_KEY} from '../constants/website-storage.const';

export type Lang = 'ar' | 'en';

const DEFAULT_LANG: Lang = 'ar';
const SUPPORTED_LANGS: Lang[] = ['ar', 'en'];

@Injectable({ providedIn: 'root' })
export class LanguageService {
  private primengConfig = inject(PrimeNG);
  /** Emits current language immediately and on every change */
  readonly current$ = new BehaviorSubject<Lang>(DEFAULT_LANG);

  /** Emits RTL state immediately and on every change */
  private isRtlSubj = new BehaviorSubject<boolean>(false);
  readonly isRtl$ = this.isRtlSubj.asObservable();

  constructor(
    private translate: TranslateService,
    @Inject(DOCUMENT) private doc: Document
  ) {
    // Constructor stays light; no .use() here.
    this.translate.addLangs(SUPPORTED_LANGS);
  }

  /** Call once at app start (from provideAppInitializer). */
  async init(): Promise<void> {
    const initial = this.resolveInitialLang();
    if (initial !== DEFAULT_LANG) {
      this.translate.setFallbackLang(DEFAULT_LANG);
    }
    await this.apply(initial, { emit: true, persist: true });
  }

  /** Current language sync getter */
  get(): Lang { return this.current$.value; }
  getPrev(): Lang { return this.current$.value === 'ar' ? 'en' : 'ar'; }
  get isRtl() { return this.get() === 'ar'; }

  /** Set language and update TranslateService + <html dir/lang> + storage */
  async set(lang: Lang): Promise<void> {
    const safe = SUPPORTED_LANGS.includes(lang) ? lang : DEFAULT_LANG;
    if (this.current$.value === safe) return;
    await this.apply(safe, { emit: true, persist: true });
  }

  /** Toggle between ar/en */
  async toggle(): Promise<void> {
    await this.set(this.get() === 'ar' ? 'en' : 'ar');
  }

  // ---- internals ----
  private resolveInitialLang(): Lang {
    const stored = LanguageService.safeGet(APP_LANGUAGE_KEY)?.toLowerCase();
    if (stored === 'ar' || stored === 'en') return stored as Lang;

    const nav = (navigator?.language || (navigator as any)?.languages?.[0] || DEFAULT_LANG).toLowerCase();
    return nav.startsWith('ar') ? 'ar' : 'en';
  }

  private async apply(lang: Lang, opts: { emit: boolean; persist: boolean }): Promise<void> {
    // Switch translations first (Angular will await this in app initializer)
    await this.translate.use(lang).toPromise();
    this.translate.get('primeng').subscribe(res => this.primengConfig.setTranslation(res));

    const isRtl = lang === 'ar';

    // <html> attributes
    const html = this.doc.documentElement;
    html.setAttribute('lang', lang);
    html.setAttribute('dir', isRtl ? 'rtl' : 'ltr');

    // <body> helper classes
    this.doc.body.classList.toggle('rtl', isRtl);
    this.doc.body.classList.toggle('ltr', !isRtl);

    // ===== Bootstrap CSS handling (two patterns) =====
    const bs = this.doc.getElementById('bs') as HTMLLinkElement | null;
    if (bs) {
      bs.href = isRtl
        ? 'assets/styles/bootstrap/bootstrap.rtl.min.css'
        : 'assets/styles/bootstrap/bootstrap.min.css';
    }
    else {
      const ltr = this.doc.getElementById('bs-ltr') as HTMLLinkElement | null;
      const rtl = this.doc.getElementById('bs-rtl') as HTMLLinkElement | null;
      if (ltr && rtl) {
        ltr.media = isRtl ? 'not all' : 'all';
        rtl.media = isRtl ? 'all' : 'not all';
      }
    }

    if (opts.persist) this.safeSet(APP_LANGUAGE_KEY, lang);

    this.isRtlSubj.next(isRtl);
    if (opts.emit) this.current$.next(lang);
  }

  public static safeGet(key: string): string | null {
    try { return localStorage.getItem(key); } catch { return null; }
  }
  private safeSet(key: string, val: string): void {
    try { localStorage.setItem(key, val); } catch { /* ignore */ }
  }
}
