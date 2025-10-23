import { Component } from '@angular/core';
import {routes} from "../../../routes/routes";
import {RouterLink} from "@angular/router";
import {TranslatePipe} from "@ngx-translate/core";

@Component({
  selector: 'app-error500',
  templateUrl: './error500.component.html',
  styleUrls: ['./error500.component.scss'],
  imports: [
    TranslatePipe,
    RouterLink
  ],
  standalone: true
})
export class Error500Component  {
  public routes = routes;



}
