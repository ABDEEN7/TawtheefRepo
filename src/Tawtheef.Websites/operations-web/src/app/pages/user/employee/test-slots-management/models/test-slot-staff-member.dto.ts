import { UserDto } from '../../users-management/models/user.dto';

export interface TestSlotStaffMemberDto extends UserDto {
  jobTitle?: string | null;
  departmentName?: string | null;
}
