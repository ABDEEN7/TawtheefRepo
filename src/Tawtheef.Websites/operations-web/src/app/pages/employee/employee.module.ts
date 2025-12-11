import {NgModule} from '@angular/core';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {NgbModalModule} from '@ng-bootstrap/ng-bootstrap';
import {I18nNamespaceDirective} from '../../shared/directives/i18n-namespace.directive';
import {TranslatePipe} from '@ngx-translate/core';
import {CommonModule} from '@angular/common';
import {RouterModule} from '@angular/router';
import {employeeRoutes} from './employee.routes';

@NgModule({
  imports:[
    RouterModule.forChild(employeeRoutes),
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    NgbModalModule,
    TranslatePipe,
    I18nNamespaceDirective
  ]
})
export class EmployeeModule {
}
