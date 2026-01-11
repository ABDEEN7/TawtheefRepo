import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import {EndpointsService} from '../../../../../core/http/endpoints.service';
import {HttpService} from '../../../../../core/http/http.service';
@Injectable({ providedIn: 'root' })
export class ContactVerificationService {
  private http = inject(HttpService);
  private endpoints = inject(EndpointsService);

  requestPhoneCode(payload: { phoneE164: string }): Observable<void> {
    return this.http.post<void>(this.endpoints.user.verify.phone.request, payload);
  }

  verifyPhoneCode(payload: { phoneE164: string; code: string }): Observable<void> {
    return this.http.post<void>(this.endpoints.user.verify.phone.confirm, payload);
  }

  requestEmailVerification(payload: { email: string }): Observable<void> {
    return this.http.post<void>(this.endpoints.user.verify.email.request, payload);
  }

  verifyEmailCode(payload: { email: string; code: string }): Observable<void> {
    return this.http.post<void>(this.endpoints.user.verify.email.confirm, payload);
  }

  updatePhone(payload: { phoneE164: string; }): Observable<void> {
    return this.http.post<void>(this.endpoints.user.verify.phone.update, payload);
  }
}
