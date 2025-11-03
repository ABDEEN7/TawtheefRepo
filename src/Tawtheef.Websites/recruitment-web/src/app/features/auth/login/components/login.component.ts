import {Component} from '@angular/core';
import {ReactiveFormsModule} from '@angular/forms';
import {NgIf} from "@angular/common";
import {TranslatePipe} from "@ngx-translate/core";
import {ExternalLoginService} from '../../external-login';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
  imports: [
    NgIf,
    ReactiveFormsModule,
    TranslatePipe,
  ],
  standalone: true
})
export class LoginComponent {
  loading = false;
  errorMessage: string | null = null;

  constructor(private external: ExternalLoginService) {}
  loginWithProvider(provider: 'google' | 'qatar-pass') {
    switch (provider) {
      case 'google':
        this.external.loginUsingGoogle('Google');
        break;
      case 'qatar-pass':
        this.external.loginUsingQatarPass();
        break;
    }
  }
}
