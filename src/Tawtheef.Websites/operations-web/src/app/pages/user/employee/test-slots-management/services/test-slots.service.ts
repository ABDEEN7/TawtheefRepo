import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { EndpointsService } from '../../../../../core/http/endpoints.service';
import { HttpService } from '../../../../../core/http/http.service';
import { HDR } from '../../../../../core/utils/headers.flags';
import { PaginatedResult } from '../../../../../core/models/paginated-result.model';
import { RoomListItemDto } from '../../rooms-management/models/room-list-item.dto';
import { TestSlotFilters, TestSlotListItemDto } from '../models/test-slot-list-item.dto';
import { CreatedTestSlotDto, CreateTestSlotDto } from '../models/create-test-slot.dto';

@Injectable({ providedIn: 'root' })
export class TestSlotsService {
  private readonly http = inject(HttpService);
  private readonly endpoints = inject(EndpointsService);

  list(filters: TestSlotFilters): Observable<PaginatedResult<TestSlotListItemDto>> {
    return this.http.get<PaginatedResult<TestSlotListItemDto>>(this.endpoints.testSlots.list, filters);
  }

  getAvailableRoomsForTestSlot(language: string): Observable<RoomListItemDto[]> {
    return this.http.get<RoomListItemDto[]>(this.endpoints.testSlots.availableRooms, { language });
  }

  getAvailableRoomsForTestSlotWizard(language: string): Observable<RoomListItemDto[]> {
    return this.http.get<RoomListItemDto[]>(this.endpoints.testSlots.wizardAvailableRooms, { language });
  }

  create(testSlot: CreateTestSlotDto): Observable<CreatedTestSlotDto> {
    return this.http.post<CreatedTestSlotDto>(this.endpoints.testSlots.create, testSlot, undefined, {
      headers: { [HDR.SkipError]: 'true' },
    });
  }
}
