export interface RoleDto {
  id: string;
  nameAr: string;
  nameEn: string;
  descriptionAr?: string;
  descriptionEn?: string;
  systemName: string;
  isSystemRole: boolean;
  permissions: string[]; // Permission IDs
  permissionNames?: string[];
}
