export const TestSlotStaffRoleIds = {
  hallSupervisor: 'f8535d6c-a02e-46d8-a124-db986e51d5fc',
  monitor: '7020e361-a374-4ebd-ab69-dd95d1de8789',
} as const;

export type TestSlotStaffRoleId =
  (typeof TestSlotStaffRoleIds)[keyof typeof TestSlotStaffRoleIds];

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
