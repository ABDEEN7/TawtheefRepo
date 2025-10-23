import {Inject, Injectable, PLATFORM_ID} from "@angular/core";
import {from, Observable, throwError} from "rxjs";
import {isPlatformBrowser} from "@angular/common";
import {HttpService} from "../http/http.service";

@Injectable({providedIn: 'root'})
export class SocialAuthService {
  constructor(
    private http: HttpService,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {
  }

  signIn(provider: 'google'): Observable<{ token: string }> {
    if (isPlatformBrowser(this.platformId)) {
      return from(this.handleClientSideAuth(provider));
    }
    return throwError(() => 'Social auth only available in browser');
  }

  private async handleClientSideAuth(provider: string): Promise<{ token: string }> {
    throw new Error('Method not implemented.');
  }
}
