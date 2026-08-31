export interface CreateRoomRequest {
  nameAr: string;
  nameEn: string;
  roomTypeId: string;
  capacity: number;
  locationId: string;
  statusId: string;
  notes: string | null;
}
