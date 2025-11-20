import { Component } from '@angular/core';
import {RouterLink} from '@angular/router';
import {TranslatePipe} from '@ngx-translate/core';
import {routes} from '../../../routes/routes';

@Component({
  selector: 'app-footer',
  imports: [
    RouterLink,
    TranslatePipe
  ],
  templateUrl: './footer.html',
  styleUrl: './footer.scss',
})
export class Footer {
  currentYear: number = new Date().getFullYear();
  routes = routes;
}
