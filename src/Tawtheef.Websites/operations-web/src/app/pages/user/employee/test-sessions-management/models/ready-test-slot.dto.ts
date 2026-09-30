import { PaginatedResult } from '../../../../../core/models/paginated-result.model';

export interface ReadyTestSlotListItemDto {
  testSlotId: string;
  slotName: string;
  slotDate: string;
  startTime: string;
  endTime: string;
  roomId: string;
  roomName: string;
  roomCapacity: number;
  currentReservations: number;
  existingSessionCount: number;
  existingExamSessionCount: number;
  remainingCapacity: number;
  status: string;
}

export interface LocalTestSession {
  slot: ReadyTestSlotListItemDto;
  startTime: string;
  endTime: string;
  availableCapacity: number;
}

export interface ReadyTestSlotsSummaryDto {
  existingExamSessionCount: number;
  existingExamCandidateCount: number;
  availableCapacity: number;
}

export interface ReadyTestSlotsDto {
  slots: PaginatedResult<ReadyTestSlotListItemDto>;
  summary: ReadyTestSlotsSummaryDto;
}

export interface ReadyTestSlotFilters {
  examId: string;
  language: string;
  pageNumber: number;
  pageSize: number;
  selectedCandidateCount: number;
  searchText?: string;
  roomId?: string;
  date?: string;
}
