import { Component } from '@angular/core';
import {RouterOutlet} from "@angular/router";
import {I18nNamespaceDirective} from '../../shared/directives/i18n-namespace.directive';

@Component({
  selector: 'app-error',
  templateUrl: './error.component.html',
  styleUrls: ['./error.component.scss'],
  imports: [
    RouterOutlet,
    I18nNamespaceDirective
  ],
  standalone: true
})
export class ErrorComponent  {
}
