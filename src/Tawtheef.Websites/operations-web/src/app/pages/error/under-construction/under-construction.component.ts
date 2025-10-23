import { Component } from '@angular/core';
import {routes} from "../../../routes/routes";
import {RouterLink} from "@angular/router";
import {TranslatePipe} from "@ngx-translate/core";

@Component({
  selector: 'app-under-construction',
  templateUrl: './under-construction.component.html',
  styleUrls: ['./under-construction.component.scss'],
  imports: [
    TranslatePipe,
    RouterLink
  ],
  standalone: true
})
export class UnderConstructionComponent  {
  public routes = routes;


}
