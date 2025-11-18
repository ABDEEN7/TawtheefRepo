//TODO :: Need Refactor.
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, shareReplay } from 'rxjs';
import { ILookups } from '../models/lookups.model';

@Injectable({
  providedIn: 'root'
})
export class LookupService {
  private apiUrl = 'api/lookups';
  private cache = new Map<string, Observable<ILookups[]>>();

  constructor(private http: HttpClient) {}

  getLookups(lookupType: string): Observable<ILookups[]> {
    if (!this.cache.has(lookupType)) {
      const lookup$ = this.http.get<ILookups[]>(`${this.apiUrl}/${lookupType}`)
        .pipe(shareReplay(1));
      
      this.cache.set(lookupType, lookup$);
    }
    return this.cache.get(lookupType)!;
  }

  // Convenience methods
  getDepartments(): Observable<ILookups[]> {
    return this.getLookups('departments');
  }

  getMajors(): Observable<ILookups[]> {
    return this.getLookups('majors');
  }
  getDegrees(): Observable<ILookups[]> {
    return this.getLookups('dergrees');
  }
}