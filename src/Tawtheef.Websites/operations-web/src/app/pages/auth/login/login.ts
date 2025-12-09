import { Component, OnInit, OnDestroy, inject } from '@angular/core';
import {LanguageService} from '../../../core/services/language.service';
import {ExternalLoginService} from '../../../core/auth/external-login';
import {TranslatePipe} from '@ngx-translate/core';
import {Subscription} from 'rxjs';
import {I18nNamespaceDirective} from '../../../shared/directives/i18n-namespace.directive';

@Component({
  selector: 'app-login',
  templateUrl: './login.html',
  styleUrls: ['./login.scss'],
  imports: [
    TranslatePipe,
    I18nNamespaceDirective,
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

  startLogin(kind: 'azure' | 'google'): void {
    switch (kind) {
      case 'azure':
        this.auth.loginUsingAzure();
        break;
      case 'google':
        this.auth.loginUsingGoogle();
        break;
    }
  }
}
