import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HomeRoutingModule } from './home-routing.module';
import { SharedModule } from '../../shared/shared.module';
import {RouterLink} from "@angular/router";

@NgModule({
  declarations: [],
  imports: [CommonModule, SharedModule,
    RouterLink, HomeRoutingModule]
})
export class HomeModule {}
