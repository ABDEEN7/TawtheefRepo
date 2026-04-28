import { inject, Injectable } from '@angular/core';
import { HttpService } from '../../../../../core/http/http.service';
import { EndpointsService } from '../../../../../core/http/endpoints.service';
import { Observable } from 'rxjs';
import {
  TemplateMetadata,
  SendTestRequest,
  SendTestResponse,
  PreviewRequest,
  PreviewResponse
} from '../models/notification-tester.models';

@Injectable({ providedIn: 'root' })
export class NotificationTesterService {
  private http = inject(HttpService);
  private endpoints = inject(EndpointsService);

  getTemplates(): Observable<TemplateMetadata[]> {
    return this.http.get<TemplateMetadata[]>(this.endpoints.notificationTester.templates);
  }

  sendTest(request: SendTestRequest): Observable<SendTestResponse> {
    return this.http.post<SendTestResponse>(this.endpoints.notificationTester.send, request);
  }

  preview(request: PreviewRequest): Observable<PreviewResponse> {
    return this.http.post<PreviewResponse>(this.endpoints.notificationTester.preview, request);
  }
}
