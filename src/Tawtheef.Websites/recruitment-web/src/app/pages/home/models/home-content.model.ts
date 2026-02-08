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
}

export interface FAQ {
  id: string;
  questionAr: string;
  questionEn: string;
  answerAr: string;
  answerEn: string;
  displayOrder: number;
}

export interface HomeContentResponse {
  successStories: HomeSuccessStory[];
  faqs: FAQ[];
}
