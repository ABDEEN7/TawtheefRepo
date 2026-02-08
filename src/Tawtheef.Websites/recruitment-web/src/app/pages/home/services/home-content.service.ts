import {inject, Injectable} from '@angular/core';
import {HttpService} from '../../../core/http/http.service';
import {EndpointsService} from '../../../core/http/endpoints.service';
import {HomeContentResponse} from '../models/home-content.model';

@Injectable({providedIn: 'root'})
export class HomeContentService {
  private http = inject(HttpService);
  private endpoints = inject(EndpointsService);

  getHomeContent() {
    return this.http.get<HomeContentResponse>(this.endpoints.homeContent.content);
  }
}
