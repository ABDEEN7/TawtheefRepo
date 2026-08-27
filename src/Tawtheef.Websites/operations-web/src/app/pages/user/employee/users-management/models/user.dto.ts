import {RoleSummaryDto} from './role-summary.dto';

export interface UserDto {
  id: string;
  name: string;
  email: string;
  section?: string | null;
  lastLoginDate?: string | null;
  isBlocked: boolean;
  roles?: RoleSummaryDto[];
  roleNames?: string[];
}
