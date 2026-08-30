import { RoomStatus, RoomType } from '../../../../../core/enums/lookups.enum';

export interface RoomListItemDto {
  id: string;
  nameAr: string;
  nameEn: string;
  location: string | null;
  roomType: RoomType;
  capacity: number;
  status: RoomStatus;
  notes: string | null;
  lastUpdated: string;
}

export interface RoomFilters {
  pageNumber: number;
  pageSize: number;
  search?: string;
  location?: string;
  roomType?: RoomType;
  status?: RoomStatus;
}
