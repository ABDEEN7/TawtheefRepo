import {CommonModule} from '@angular/common';
import {NgModule} from '@angular/core';
import {RouterModule} from '@angular/router';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {NgbModalModule} from '@ng-bootstrap/ng-bootstrap';
import {TranslatePipe} from '@ngx-translate/core';
import {WizardProfileModule} from './wizard-profile/wizard-profile.module';
import {userRoutes} from './user.routes';
import {I18nNamespaceDirective} from '../../shared/directives/i18n-namespace.directive';
import {Dashboard} from './dashboard/dashboard';

@NgModule({
  imports: [
    RouterModule.forChild(userRoutes),
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    NgbModalModule,
    TranslatePipe,
    WizardProfileModule,
    I18nNamespaceDirective
  ]
})
export class UserModule {}
