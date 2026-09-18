import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { EndpointsService } from '../../../../../core/http/endpoints.service';
import { HttpService } from '../../../../../core/http/http.service';
import { HDR } from '../../../../../core/utils/headers.flags';
import { PaginatedResult } from '../../../../../core/models/paginated-result.model';
import { RoomListItemDto } from '../../rooms-management/models/room-list-item.dto';
import { UserFilters } from '../../users-management/models/user-filters.dto';
import { TestSlotStaffMemberDto } from '../models/test-slot-staff-member.dto';
import { TestSlotFilters, TestSlotListItemDto } from '../models/test-slot-list-item.dto';
import { CreateTestSlotDto, TestSlotConfigurationDto } from '../models/create-test-slot.dto';

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

  getStaffMembers(filters: UserFilters): Observable<PaginatedResult<TestSlotStaffMemberDto>> {
    return this.http.get<PaginatedResult<TestSlotStaffMemberDto>>(this.endpoints.testSlots.wizardStaffMembers, filters);
  }

  configuration(id: string, language: string): Observable<TestSlotConfigurationDto> {
    return this.http.get<TestSlotConfigurationDto>(this.endpoints.testSlots.configuration(id), { language });
  }

  save(testSlot: CreateTestSlotDto, id: string | null): Observable<void> {
    const options = {
      headers: { [HDR.SkipError]: 'true' },
    };
    return id
      ? this.http.put<void>(this.endpoints.testSlots.update(id), testSlot, undefined, options)
      : this.http.post<void>(this.endpoints.testSlots.create, testSlot, undefined, options);
  }
}
