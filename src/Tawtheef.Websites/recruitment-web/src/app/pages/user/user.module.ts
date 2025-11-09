import {CommonModule} from '@angular/common';
import {NgModule} from '@angular/core';
import {RouterModule} from '@angular/router';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {NgbModalModule} from '@ng-bootstrap/ng-bootstrap';
import {TranslatePipe} from '@ngx-translate/core';
import {WizardProfileModule} from './wizard-profile/wizard-profile.module';
import {userRoutes} from './user.routes';

@NgModule({
  declarations: [

  ],
  imports: [
    RouterModule.forChild(userRoutes),
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    NgbModalModule,
    TranslatePipe,
    WizardProfileModule,
  ]
})
export class UserModule {}
