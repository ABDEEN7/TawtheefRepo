export interface FAQ {
  id: string;
  questionAr: string;
  questionEn: string;
  answerAr: string;
  answerEn: string;
  displayOrder: number;
  isActive: boolean;
}

export type FaqPayload = Omit<FAQ, 'id'>;
