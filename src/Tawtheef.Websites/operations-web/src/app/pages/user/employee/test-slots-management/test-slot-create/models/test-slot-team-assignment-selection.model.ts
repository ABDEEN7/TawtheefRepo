import { UserDto } from '../../../users-management/models/user.dto';

export interface TestSlotTeamAssignmentSelection {
  roomHead: UserDto | null;
  selectedStaff: UserDto[];
}
