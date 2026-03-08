import { Injectable, inject } from '@angular/core';
import { HttpHeaders } from '@angular/common/http';
import { Observable, of, race, timer } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { HttpService } from '../http/http.service';
import { HDR } from '../utils/headers.flags';

type GeoAny = {
  country?: string;                 // ipwhois sometimes returns country code, sometimes name
  country_code?: string;            // ipapi often returns country_code
  country_code2?: string;           // some providers
  geoplugin_countryCode?: string;   // geoplugin
};

@Injectable({ providedIn: 'root' })
export class GeoIpService {
  private http = inject(HttpService);

  getCountryIso2(): Observable<string> {
    // Emit a fallback object if all providers are slow
    const timeout$ = timer(1800).pipe(map(() => ({}) as GeoAny));

    const ipapi$ = this.http.get<GeoAny>('https://ipapi.co/json/', undefined).pipe(
      catchError(() => this.http.get<GeoAny>('https://ipwhois.app/json/', undefined)),
      catchError(() => this.http.get<GeoAny>('https://www.geoplugin.net/json.gp', undefined)),
      catchError(() => of({} as GeoAny))
    );

    return race(ipapi$, timeout$).pipe(
      map((data) => {
        const raw =
          data?.country_code ||
          data?.country_code2 ||
          data?.geoplugin_countryCode ||
          data?.country ||
          '';

        const iso2 = String(raw).trim().toUpperCase();

        // If provider returns a full country name (e.g., "Qatar"), this won’t be ISO2.
        // We keep it simple: require length===2, otherwise fallback.
        return iso2.length === 2 ? iso2 : 'QA';
      }),
      catchError(() => of('QA'))
    );
  }
}
