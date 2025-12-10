import {RoleSummaryDto} from './role-summary.dto';

export interface UserRolesResponse {
  userId: string;
  name: string;
  email: string;
  isBlocked: boolean;
  lastLoginDate?: string | null;
  roles: RoleSummaryDto[];
  assignedRoleIds: string[];
}
