import { NgModule, Optional, SkipSelf } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthService } from './auth/auth.service';
import { HttpService } from './http/http.service';
import { EndpointsService } from './http/endpoints.service';
import { LoggerService } from './services/logger.service';
import { NotificationService } from './services/notification.service';
import { LanguageService } from './services/language.service';
import { VersionService } from './services/version.service';
import {MessageService} from "primeng/api";

@NgModule({
  imports: [CommonModule,],
  providers: [
    AuthService,
    HttpService,
    EndpointsService,
    LoggerService,
    NotificationService,
    LanguageService,
    VersionService,
    MessageService
  ]
})
export class CoreModule {
  constructor(@Optional() @SkipSelf() parentModule: CoreModule) {
    if (parentModule) {
      throw new Error('CoreModule is already loaded. Import only in AppModule.');
    }
  }
}
