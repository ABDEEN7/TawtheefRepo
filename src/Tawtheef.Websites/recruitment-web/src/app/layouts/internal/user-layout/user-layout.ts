import { Component } from '@angular/core';
import {RouterOutlet} from '@angular/router';
import {SideNav} from '../side-nav/side-nav';

@Component({
  selector: 'app-user-layout',
  imports: [
    RouterOutlet,
    SideNav
  ],
  templateUrl: './user-layout.html',
  styleUrl: './user-layout.scss',
})
export class UserLayout {

}
