import { dropdownOptionsModel } from "../../../../../shared/models/dropdown-options.model";

export interface TestSlotListItemDto {
  id: string;
  title: string;
  roomId: string;
  roomName: string;
  slotDate: string;
  startTime: string;
  endTime: string;
  candidateCount: number;
  hallSupervisorId?: string;
  hallSupervisorName?: string;
  status: dropdownOptionsModel;
}

export interface TestSlotFilters {
  pageNumber: number;
  pageSize: number;
  language: string;
  searchTerm?: string;
  roomId?: string;
  dateFrom?: string;
  dateTo?: string;
}
