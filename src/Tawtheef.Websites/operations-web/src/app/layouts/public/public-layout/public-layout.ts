import { Component } from '@angular/core';
import {RouterOutlet} from '@angular/router';
import {Navbar} from '../navbar/navbar';
import {TranslatePipe} from '@ngx-translate/core';
import {Footer} from '../footer/footer';
import {I18nNamespaceDirective} from '../../../shared/directives/i18n-namespace.directive';
@Component({
  selector: 'app-public-layout',
  imports: [
    RouterOutlet,
    Navbar,
    Footer,
    I18nNamespaceDirective
  ],
  templateUrl: './public-layout.html',
  styleUrl: './public-layout.scss',
})
export class PublicLayout {

}
