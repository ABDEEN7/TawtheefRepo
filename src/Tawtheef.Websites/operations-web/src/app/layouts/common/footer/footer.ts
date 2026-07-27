import { Component } from '@angular/core';
import {TranslatePipe} from '@ngx-translate/core';
import {routes} from '../../../routes/routes';
import { environment } from '../../../../environments/environment';

@Component({
  selector: 'app-footer',
  imports: [
    TranslatePipe,
  ],
  templateUrl: './footer.html',
  styleUrl: './footer.scss',
})
export class Footer {
  currentYear: number = new Date().getFullYear();
  routes = routes;
  env = environment;
}
