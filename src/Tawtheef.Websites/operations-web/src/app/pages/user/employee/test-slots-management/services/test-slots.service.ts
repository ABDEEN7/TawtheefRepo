import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { EndpointsService } from '../../../../../core/http/endpoints.service';
import { HttpService } from '../../../../../core/http/http.service';
import { PaginatedResult } from '../../../../../core/models/paginated-result.model';
import { dropdownOptionsModel } from '../../../../../shared/models/dropdown-options.model';
import { TestSlotFilters, TestSlotListItemDto } from '../models/test-slot-list-item.dto';

@Injectable({ providedIn: 'root' })
export class TestSlotsService {
  private readonly http = inject(HttpService);
  private readonly endpoints = inject(EndpointsService);

  list(filters: TestSlotFilters): Observable<PaginatedResult<TestSlotListItemDto>> {
    return this.http.get<PaginatedResult<TestSlotListItemDto>>(this.endpoints.testSlots.list, filters);
  }

  getAvailableRoomsForTestSlot(language: string): Observable<dropdownOptionsModel[]> {
    return this.http.get<dropdownOptionsModel[]>(this.endpoints.testSlots.availableRooms, { language });
  }

  getAvailableRoomsForTestSlotWizard(language: string): Observable<dropdownOptionsModel[]> {
    return this.http.get<dropdownOptionsModel[]>(this.endpoints.testSlots.wizardAvailableRooms, { language });
  }
}
