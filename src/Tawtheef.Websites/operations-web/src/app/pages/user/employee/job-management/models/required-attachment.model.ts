import { GUID } from "../../../../../shared/types/guid.type";

export interface JobRequiredAttachment {
  id?: GUID;
  titleAr: string;
  titleEn: string;
  isMandatory: boolean;
}
