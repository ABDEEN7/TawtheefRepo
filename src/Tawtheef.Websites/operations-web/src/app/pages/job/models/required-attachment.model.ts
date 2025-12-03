import { GUID } from "../../../shared/types/guid.type";

export interface RequiredAttachment {
  id?: GUID;
  title: string;
  isMandatory: boolean;
}