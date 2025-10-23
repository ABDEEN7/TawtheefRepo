import {Injectable} from "@angular/core";
import {HttpClient} from "@angular/common/http";
import {Observable} from "rxjs";
import {EndpointsService} from "../http/endpoints.service";
import {HttpService} from "../http/http.service";

@Injectable({ providedIn: 'root' })
export class VerificationActionsService {
  constructor(private http: HttpService,
              private endpoints: EndpointsService
  ) {}

  verifyOtp(email: string, otp: string): Observable<any> {
    return this.http.post(this.endpoints.auth.verifyOtp, { email, otp });
  }

  resendOtp(email: string): Observable<any> {
    return this.http.post(this.endpoints.auth.resendOtp, { email });
  }
}
