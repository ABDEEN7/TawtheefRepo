import { Component } from '@angular/core';
import {RouterOutlet} from '@angular/router';
import {Navbar} from '../navbar/navbar';
import {TranslatePipe} from '@ngx-translate/core';
import {Footer} from '../footer/footer';

@Component({
  selector: 'app-public-layout',
  imports: [
    RouterOutlet,
    Navbar,
    TranslatePipe,
    Footer
  ],
  templateUrl: './public-layout.html',
  styleUrl: './public-layout.scss',
})
export class PublicLayout {

}
