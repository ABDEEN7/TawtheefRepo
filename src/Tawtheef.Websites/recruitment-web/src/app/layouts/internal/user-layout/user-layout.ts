import { Component } from '@angular/core';
import {RouterOutlet} from '@angular/router';
import {Navbar} from '../navbar/navbar';
import {Footer} from '../footer/footer';
import {I18nNamespaceDirective} from '../../../shared/directives/i18n-namespace.directive';

@Component({
  selector: 'app-user-layout',
  imports: [
    RouterOutlet,
    Navbar,
    Footer,
    I18nNamespaceDirective
  ],
  templateUrl: './user-layout.html',
  styleUrl: './user-layout.scss',
})
export class UserLayout {

}
