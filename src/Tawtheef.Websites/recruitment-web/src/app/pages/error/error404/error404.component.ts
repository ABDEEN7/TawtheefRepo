import { Component } from '@angular/core';
import {routes} from "../../../routes/routes";
import {RouterLink} from "@angular/router";
import {TranslatePipe} from "@ngx-translate/core";

@Component({
  selector: 'app-error404',
  templateUrl: './error404.component.html',
  styleUrls: ['./error404.component.scss'],
  imports: [
    TranslatePipe,
    RouterLink
  ],
  standalone: true
})
export class Error404Component  {
  public routes = routes;

}
