import { dropdownOptionsModel } from '../../../../../shared/models/dropdown-options.model';
import { LocationDto } from '../../locations-management/models/location.dto';

export interface RoomListItemDto {
  id: string;
  nameAr: string;
  nameEn: string;
  locationId: string;
  location: LocationDto;
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
