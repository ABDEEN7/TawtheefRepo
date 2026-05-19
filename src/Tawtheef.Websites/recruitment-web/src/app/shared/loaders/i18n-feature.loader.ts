import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { TranslateService } from '@ngx-translate/core';
import { firstValueFrom, of } from 'rxjs';
import { catchError } from 'rxjs/operators';

type BundleKey = `${string}:${string}`;

@Injectable({ providedIn: 'root' })
export class I18nFeatureLoader {
  private http = inject(HttpClient);
  private translate = inject(TranslateService);

  // ✅ Promise resolves to Record<string, any> only
  private cache = new Map<BundleKey, Promise<Record<string, any>>>();

  async ensureLoaded(namespace: string, lang?: string): Promise<void> {
    const current = lang ?? this.translate.currentLang ?? this.translate.getDefaultLang() ?? 'ar';
    const key: BundleKey = `${namespace}:${current}`;
    const yymmddhh = new Date()
      .toLocaleString("sv-SE", {
        year: "2-digit",
        month: "2-digit",
        day: "2-digit",
        hour: "2-digit",
        hour12: false,
      })
      .replace(/\D/g, "");
    const url = `/i18n/${namespace}/${current}.json?v=${encodeURIComponent(yymmddhh)}`;

    if (!this.cache.has(key)) {
      // ✅ firstValueFrom + catchError -> never undefined
      const p = firstValueFrom(
        this.http.get<Record<string, any>>(url, {
          headers: { 'X-Skip-Loading': 'true' }
        }).pipe(
          catchError(() => of({} as Record<string, any>))
        )
      );
      this.cache.set(key, p);
    }

    const bundle = await this.cache.get(key)!; // bundle is guaranteed typed
    this.translate.setTranslation(current, bundle, true);
  }

  clear(namespace: string, lang?: string) {
    const current = lang ?? this.translate.currentLang ?? 'ar';
    this.cache.delete(`${namespace}:${current}`);
  }
}
