export interface HomeSuccessStory {
  id: string;
  nameAr: string;
  nameEn: string;
  roleAr: string;
  roleEn: string;
  metricTitleAr: string;
  metricTitleEn: string;
  metricDescriptionAr: string;
  metricDescriptionEn: string;
  imageUrl: string;
  imagePreviewUrl: string;
  displayOrder: number;
  isActive: boolean;
}

export type HomeSuccessStoryPayload = Omit<HomeSuccessStory, 'id' | 'imagePreviewUrl'> & {
  imageFile?: File | null;
};
