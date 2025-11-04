import { Component, OnInit, OnDestroy, inject } from '@angular/core';
import {LanguageService} from '../../../core/services/language.service';
import {EndpointsService} from '../../../core/http/endpoints.service';
import {AuthService} from '../../../core/auth/auth.service';
import {ExternalLoginService} from '../../../core/auth/external-login';
import {TranslatePipe} from '@ngx-translate/core';
import {Subscription} from 'rxjs';

@Component({
  selector: 'app-login',
  templateUrl: './login.html',
  styleUrls: ['./login.scss'],
  imports: [
    TranslatePipe
  ]
})
export class Login implements OnInit, OnDestroy {
  private auth = inject(ExternalLoginService);
  private lang = inject(LanguageService);

  currentLang: 'ar' | 'en' = 'ar';
  private subs: Subscription[] = [];

  ngOnInit(): void {
    const s = this.lang.current$.subscribe(code => {
      this.currentLang = (code as 'ar' | 'en') || 'ar';
    });
    this.subs.push(s);
  }
  ngOnDestroy(): void {
    this.subs.forEach(s => s.unsubscribe());
  }

  toggleLang(): void {
    this.lang.toggle();
    this.currentLang = this.lang.get();
  }

  startAzure(): void {
    this.auth.loginUsingAzure();
  }
}
