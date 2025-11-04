import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { LocalizedDirective } from './directives/localized.directive';
import {TranslatePipe} from '@ngx-translate/core';

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    LocalizedDirective,
    TranslatePipe
  ],
  exports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    TranslatePipe,
  ]
})
export class SharedModule {}
