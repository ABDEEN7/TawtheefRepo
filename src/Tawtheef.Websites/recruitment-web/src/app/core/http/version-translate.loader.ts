import { HttpBackend, HttpClient } from '@angular/common/http';
import { TranslateLoader } from '@ngx-translate/core';
import { Observable, forkJoin, of } from 'rxjs';
import { catchError, map, shareReplay, switchMap } from 'rxjs/operators';
import { inject } from '@angular/core';

export interface TranslationResource {
  prefix: string;
  suffix?: string;
  optional?: boolean;
}

type VersionJson = { buildNo?: number; commit?: string; version?: string };

export class VersionedMultiTranslateLoader implements TranslateLoader {
  http = inject(HttpClient);
  constructor(private resources: (string | TranslationResource)[]) {

  }

  getTranslation(lang: string): Observable<any> {
    const requests = this.resources.map((r) => {
      const res: TranslationResource =
        typeof r === 'string' ? { prefix: r, suffix: '.json' } : r;
      const yymmddhh = new Date()
        .toLocaleString("sv-SE", {
          year: "2-digit",
          month: "2-digit",
          day: "2-digit",
          hour: "2-digit",
          hour12: false,
        })
        .replace(/\D/g, "");
      const suffix = res.suffix ?? '.json';
      const url = `${res.prefix}${lang}${suffix}?v=${encodeURIComponent(yymmddhh)}`;

      return this.http.get<Record<string, any>>(url, {
        headers: { 'X-Skip-Loading': 'true' }
      }).pipe(
        catchError((err) => (res.optional ? of({}) : (() => { throw err; })() as any))
      );
    });

    return forkJoin(requests).pipe(
      map((parts) =>
        parts.reduce((acc: any, cur: any) => ({ ...acc, ...cur }), {}))
    );
  }
}
