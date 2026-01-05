import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { EndpointsService } from '../http/endpoints.service';
import { AuthResponse } from '../models/auth/auth-response.model';
import {DateOnly} from '../../shared/types/dateOnly.type';

@Injectable({ providedIn: 'root' })
export class QatarResidentOtpService {
  private http = inject(HttpClient);
  private endpoints = inject(EndpointsService);

  requestOtp(qid: string, phoneNumber: string, qidExpiry: DateOnly) {
    return this.http.post<void>(this.endpoints.auth.qatarResident.requestOtp, { qid, phoneNumber, qidExpiry });
  }

  verifyOtp(qid: string, phoneNumber: string, otp: string, qidExpiry: DateOnly) {
    return this.http.post<AuthResponse>(this.endpoints.auth.qatarResident.verifyOtp, {
      qid,
      phoneNumber,
      qidExpiry,
      otp
    });
  }
}
