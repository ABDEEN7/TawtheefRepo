import { inject, Injectable } from '@angular/core';
import { HttpService } from '../../../../../core/http/http.service';
import { EndpointsService } from '../../../../../core/http/endpoints.service';
import { KawaderUploadResult } from '../models/kawader-upload.model';
import { GetKawaderQidsRequest, KawaderQidDto } from '../models/kawader-list.model';
import { PaginatedResult } from '../../../../../core/models/paginated-result.model';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class KawaderService {
  private http = inject(HttpService);
  private endpoints = inject(EndpointsService);

  upload(file: File) {
    const form = new FormData();
    form.append('file', file);

    return this.http.post<KawaderUploadResult>(this.endpoints.kawader.upload, form);
  }

  getList(request: GetKawaderQidsRequest): Observable<PaginatedResult<KawaderQidDto>> {
    return this.http.get<PaginatedResult<KawaderQidDto>>(this.endpoints.kawader.list, request);
  }
}

