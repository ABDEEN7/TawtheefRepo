import {inject, Injectable} from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import {HttpService} from '../http/http.service';

@Injectable({ providedIn: 'root' })
export class GeoIpService {
  private readonly apiUrl = 'https://ipapi.co/json/';
  http = inject(HttpService);
  getCountryIso2(): Observable<string> {
    return this.http.get<{ country_code: string }>(this.apiUrl).pipe(
      map(res => res.country_code || 'QA')
    );
  }
}
