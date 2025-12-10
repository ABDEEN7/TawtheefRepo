export interface RoleSummaryDto {
  id: string;
  nameAr: string;
  nameEn: string;
}

export interface UserRolesResponse {
  userId: string;
  name: string;
  email: string;
  isBlocked: boolean;
  lastLoginDate?: string | null;
  roles: RoleSummaryDto[];
  assignedRoleIds: string[];
}
