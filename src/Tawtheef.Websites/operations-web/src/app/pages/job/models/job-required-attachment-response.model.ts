import { GUID } from "../../../shared/types/guid.type";

export interface JobRequiredAttachmentResponseDto {
  id: GUID;
  title: string;
  isMandatory: boolean;
}