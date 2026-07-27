import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslatePipe } from '@ngx-translate/core';
import {routes} from '../../../routes/routes';
import { environment } from '../../../../environments/environment';

@Component({
  selector: 'app-footer',
  imports: [CommonModule, TranslatePipe],
  templateUrl: './footer.html',
  styleUrl: './footer.scss',
})
export class Footer {  
  routes = routes;
  env = environment;
  getCurrentYear() {
    return new Date().getFullYear();
  }
}
