import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HomeRoutingModule } from './home-routing.module';
import { SharedModule } from '../../shared/shared.module';
import {RouterLink} from "@angular/router";
import {IndexComponent} from './index/index.component';
import {Navbar} from '../../layouts/public/navbar/navbar';
import {Footer} from '../../layouts/public/footer/footer';

@NgModule({
  declarations: [
    IndexComponent
  ],
  imports: [
    CommonModule,
    SharedModule,
    RouterLink,
    HomeRoutingModule,
    Navbar,
    Footer
  ]
})
export class HomeModule {}
