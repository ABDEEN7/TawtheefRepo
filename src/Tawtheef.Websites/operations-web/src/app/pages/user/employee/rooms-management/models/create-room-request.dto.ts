import { RoomStatus, RoomType } from '../../../../../core/enums/lookups.enum';

export interface CreateRoomRequest {
  nameAr: string;
  nameEn: string;
  roomType: RoomType;
  capacity: number;
  location: string | null;
  status: RoomStatus;
  notes: string | null;
}
