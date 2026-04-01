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

  private readonly version$ = this.http.get<VersionJson>('./version.json', {
    headers: { 'X-Skip-Loading': 'true' }
  }).pipe(
    map(v => String(v?.buildNo ?? v?.commit ?? v?.version ?? Date.now())),
    catchError(() => of(String(Date.now()))),
    shareReplay(1)
  );


  getTranslation(lang: string): Observable<any> {
    return this.version$.pipe(
      switchMap((ver) => {
        const requests = this.resources.map((r) => {
          const res: TranslationResource =
            typeof r === 'string' ? { prefix: r, suffix: '.json' } : r;

          const suffix = res.suffix ?? '.json';
          const url = `${res.prefix}${lang}${suffix}?v=${encodeURIComponent(ver)}`;

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
      })
    );
  }
}
