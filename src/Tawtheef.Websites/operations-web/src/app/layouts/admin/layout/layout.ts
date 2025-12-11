import { Component } from '@angular/core';
import {RouterOutlet} from '@angular/router';
import {Navbar} from '../../common/navbar/navbar';
import {Footer} from '../../common/footer/footer';
import { I18nNamespaceDirective } from '../../../shared/directives/i18n-namespace.directive';
import {SidebarComponent} from '../sidebar/sidebar.component';

@Component({
  selector: 'app-user-layout',
  imports: [
    RouterOutlet,
    Navbar,
    Footer,
    I18nNamespaceDirective,
    SidebarComponent
],
  templateUrl: './layout.html',
  styleUrl: './layout.scss',
})
export class Layout {
 isSidebarOpen = false;

 toggleSidebar() {
    this.isSidebarOpen = !this.isSidebarOpen;
  }
}
