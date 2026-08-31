import { dropdownOptionsModel } from '../../../../../shared/models/dropdown-options.model';

export interface RoomLocationDto {
  id: string;
  nameAr: string;
  nameEn: string | null;
  locationLink: string;
}

export interface RoomListItemDto {
  id: string;
  nameAr: string;
  nameEn: string;
  locationId: string;
  location: RoomLocationDto;
  roomTypeId: string;
  roomType: dropdownOptionsModel;
  capacity: number;
  statusId: string;
  status: dropdownOptionsModel;
  notes: string | null;
  lastUpdated: string;
}

export interface RoomFilters {
  pageNumber: number;
  pageSize: number;
  search?: string;
  locationId?: string;
  roomTypeId?: string;
  statusId?: string;
}
