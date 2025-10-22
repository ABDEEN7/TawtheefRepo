import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { DateRangePickerComponent } from './components/date-range-picker/date-range-picker.component';
import { ProgressBarComponent } from './components/progress-bar/progress-bar.component';
import { ToastsComponent } from './components/toasts/toasts.component';
import { LocalizedDirective } from './directives/localized.directive';
import { BytesPipe } from './pipes/bytes.pipe';
import { SafeHtmlPipe } from './pipes/safe-html.pipe';
import {DateRangePickerModule} from "./components/date-range-picker/date-range-picker.module";

@NgModule({
  declarations: [
    ProgressBarComponent,
    ToastsComponent,
    BytesPipe,
    SafeHtmlPipe
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    DateRangePickerModule,
    LocalizedDirective
  ],
  exports: [
    DateRangePickerComponent,
    ProgressBarComponent,
    ToastsComponent,
    LocalizedDirective,
    BytesPipe,
    SafeHtmlPipe,
    CommonModule,
    FormsModule,
    ReactiveFormsModule
  ]
})
export class SharedModule {}
