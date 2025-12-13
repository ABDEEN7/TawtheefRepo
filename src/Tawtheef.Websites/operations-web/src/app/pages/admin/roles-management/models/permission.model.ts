export interface RoleDto {
  id: string;
  nameAr: string;
  nameEn: string;
  descriptionAr?: string;
  descriptionEn?: string;
  permissions: string[]; // Permission IDs
}
