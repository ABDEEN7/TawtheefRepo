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
  displayOrder: number;
  isActive: boolean;
}

export type HomeSuccessStoryPayload = Omit<HomeSuccessStory, 'id'>;
