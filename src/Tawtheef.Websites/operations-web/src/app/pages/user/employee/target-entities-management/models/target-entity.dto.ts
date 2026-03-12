export interface TargetEntityDto {
  id: string | null;
  nameAr: string;
  nameEn: string;
  descriptionAr?: string | null;
  descriptionEn?: string | null;
  isActive: boolean;
  backendName?: string;
  displayOrder?: number;
}
