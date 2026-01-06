import {inject, Injectable} from '@angular/core';
import {Observable} from 'rxjs';
import {tap} from 'rxjs/operators';
import {HttpService} from '../http/http.service';
import {EndpointsService} from '../http/endpoints.service';
import {UserService} from './user.service';
import {AuthStateService} from './auth-state.service';

@Injectable({providedIn: 'root'})
export class TermsService {
  private http = inject(HttpService);
  private endpoints = inject(EndpointsService);
  private userService = inject(UserService);
  private authState = inject(AuthStateService);

  agreeToTerms(): Observable<void> {
    return this.http.post<void>(this.endpoints.user.agreeToTerms, {}).pipe(
      tap(() => {
        this.userService.markTermsAsAgreed();
        this.authState.resetBootstrap();
      })
    );
  }
}
