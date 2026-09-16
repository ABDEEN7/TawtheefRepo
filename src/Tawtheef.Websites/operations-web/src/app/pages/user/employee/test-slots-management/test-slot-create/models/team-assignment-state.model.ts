import { UserDto } from '../../../users-management/models/user.dto';

export interface TeamAssignmentState {
  roomHead: UserDto | null;
  selectedStaff: UserDto[];
}

export const initialTeamAssignmentState: TeamAssignmentState = {
  roomHead: null,
  selectedStaff: [],
};
