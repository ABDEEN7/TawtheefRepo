import {CommonModule} from '@angular/common';
import {NgModule} from '@angular/core';
import {RouterModule} from '@angular/router';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {NgbModalModule} from '@ng-bootstrap/ng-bootstrap';
import {TranslatePipe} from '@ngx-translate/core';
import {userRoutes} from './user.routes';
import {I18nNamespaceDirective} from '../../shared/directives/i18n-namespace.directive';
import {WizardProfileModule} from './profile/wizard-profile/wizard-profile.module';

@NgModule({
  imports: [
    RouterModule.forChild(userRoutes),
    I18nNamespaceDirective,
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    NgbModalModule,
    TranslatePipe,
    WizardProfileModule
  ]
})
export class UserModule {}
