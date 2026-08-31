import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { EndpointsService } from '../../../../../core/http/endpoints.service';
import { HttpService } from '../../../../../core/http/http.service';
import { PaginatedResult } from '../../../../../core/models/paginated-result.model';
import { CreateRoomRequest } from '../models/create-room-request.dto';
import { RoomFilters, RoomListItemDto } from '../models/room-list-item.dto';
import { RoomLocationDto } from '../models/room-list-item.dto';
import { dropdownOptionsModel } from '../../../../../shared/models/dropdown-options.model';

@Injectable({ providedIn: 'root' })
export class RoomsService {
  private readonly http = inject(HttpService);
  private readonly endpoints = inject(EndpointsService);

  create(request: CreateRoomRequest): Observable<string> {
    return this.http.post<string>(this.endpoints.rooms.create, request);
  }

  update(id: string, request: CreateRoomRequest): Observable<void> {
    return this.http.put<void>(this.endpoints.rooms.update(id), request);
  }

  list(filters: RoomFilters): Observable<PaginatedResult<RoomListItemDto>> {
    return this.http.get<PaginatedResult<RoomListItemDto>>(this.endpoints.rooms.list, filters);
  }

  getRoomTypes(): Observable<dropdownOptionsModel[]> {
    return this.http.get<dropdownOptionsModel[]>(this.endpoints.rooms.lookups.roomTypes);
  }

  getRoomStatuses(): Observable<dropdownOptionsModel[]> {
    return this.http.get<dropdownOptionsModel[]>(this.endpoints.rooms.lookups.roomStatuses);
  }

  getLocations(): Observable<RoomLocationDto[]> {
    return this.http.get<RoomLocationDto[]>(this.endpoints.rooms.lookups.locations);
  }
}
