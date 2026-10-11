import { TestSlotStaffRoleId } from '../constants/test-slot-staff-role.constants';

export interface CreateTestSlotStaffDto {
  staffUserId: string;
  roleId: TestSlotStaffRoleId;
  isActive: boolean;
}

export interface CreateTestSlotDto {
  titleAr: string;
  titleEn: string;
  roomId: string;
  slotDate: string;
  startTime: string;
  endTime: string;
  staff: CreateTestSlotStaffDto[];
}

export interface UpdateTestSlotAssignmentsDto {
  staff: CreateTestSlotStaffDto[];
}

export interface TestSlotConfigurationDto extends Omit<CreateTestSlotDto, 'staff'> {
  id: string;
  staff: TestSlotConfigurationStaffDto[];
}

export interface TestSlotConfigurationStaffDto {
  staffUserId: string;
  name: string;
  roleId: string;
}

export interface TestSlotStaffConflictDto {
  staffUserId: string;
  staffDisplayName: string;
  slotTitle: string;
  slotDate: string;
  startTime: string;
  endTime: string;
}
