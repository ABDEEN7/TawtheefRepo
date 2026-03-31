import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HomeRoutingModule } from './home-routing.module';
import { SharedModule } from '../../shared/shared.module';
import {RouterLink} from "@angular/router";
import {IndexComponent} from './index/index.component';
import {Navbar} from '../../layouts/public/navbar/navbar';
import {Footer} from '../../layouts/public/footer/footer';
import {I18nNamespaceDirective} from '../../shared/directives/i18n-namespace.directive';
import { TabsModule } from 'primeng/tabs';
import { CarouselModule } from 'primeng/carousel';
import { SkeletonModule } from 'primeng/skeleton';


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
    Footer,
    I18nNamespaceDirective,
    TabsModule,
    CarouselModule,
    SkeletonModule
  ]
})
export class HomeModule {}
