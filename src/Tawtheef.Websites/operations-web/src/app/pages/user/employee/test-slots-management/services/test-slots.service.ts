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
import { TestSlotAccessCodeDto, TestSlotCandidateFilters, TestSlotCandidateListItemDto, TestSlotDetailsDto, TestSlotSessionDto } from '../models/test-slot-details.dto';

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

  details(id: string, language: string): Observable<TestSlotDetailsDto> {
    return this.http.get<TestSlotDetailsDto>(this.endpoints.testSlots.details(id), { language });
  }

  sessions(id: string, language: string): Observable<TestSlotSessionDto[]> {
    return this.http.get<TestSlotSessionDto[]>(this.endpoints.testSlots.sessions(id), { language });
  }

  candidates(id: string, filters: TestSlotCandidateFilters): Observable<PaginatedResult<TestSlotCandidateListItemDto>> {
    return this.http.get<PaginatedResult<TestSlotCandidateListItemDto>>(this.endpoints.testSlots.candidates(id), filters);
  }

  accessCode(id: string): Observable<TestSlotAccessCodeDto> {
    return this.http.get<TestSlotAccessCodeDto>(this.endpoints.testSlots.accessCode(id));
  }

  start(id: string): Observable<void> {
    return this.http.post<void>(this.endpoints.testSlots.start(id), {});
  }

  close(id: string): Observable<void> {
    return this.http.post<void>(this.endpoints.testSlots.close(id), {});
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
