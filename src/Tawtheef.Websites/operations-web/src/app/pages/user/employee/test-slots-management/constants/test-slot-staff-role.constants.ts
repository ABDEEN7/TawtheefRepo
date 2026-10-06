export const TestSlotStaffRoleIds = {
  hallSupervisor: 'f8535d6c-a02e-46d8-a124-db986e51d5fc',
  monitor: '7020e361-a374-4ebd-ab69-dd95d1de8789',
} as const;

export type TestSlotStaffRoleId =
  (typeof TestSlotStaffRoleIds)[keyof typeof TestSlotStaffRoleIds];
