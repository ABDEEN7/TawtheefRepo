import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { EndpointsService } from '../../../../../core/http/endpoints.service';
import { HttpService } from '../../../../../core/http/http.service';
import { PaginatedResult } from '../../../../../core/models/paginated-result.model';
import {
  TestSessionFilters,
  TestSessionListItemDto,
  TestSessionLookupsDto,
} from '../models/test-session-list-item.dto';

@Injectable({ providedIn: 'root' })
export class TestSessionsService {
  private readonly http = inject(HttpService);
  private readonly endpoints = inject(EndpointsService);

  list(filters: TestSessionFilters): Observable<PaginatedResult<TestSessionListItemDto>> {
    return this.http.get<PaginatedResult<TestSessionListItemDto>>(this.endpoints.testSessions.list, filters);
  }

  lookups(language: string, roomId?: string): Observable<TestSessionLookupsDto> {
    return this.http.get<TestSessionLookupsDto>(this.endpoints.testSessions.lookups, { language, roomId });
  }
}
