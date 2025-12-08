import { Component } from '@angular/core';
import {RouterOutlet} from '@angular/router';
import {Navbar} from '../navbar/navbar';
import {Footer} from '../footer/footer';
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
  templateUrl: './user-layout.html',
  styleUrl: './user-layout.scss',
})
export class UserLayout {
 isSidebarOpen = false;

 toggleSidebar() {
    this.isSidebarOpen = !this.isSidebarOpen;
  }
}
