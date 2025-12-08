import { dropdownOptionsModel } from "../../../shared/models/dropdown-options.model";
import { GUID } from "../../../shared/types/guid.type";

export interface ResidentBreakdownResponse {
  id: GUID;
  jobQuotaId: GUID;
  nationalityId: GUID;
  nationality: dropdownOptionsModel;
  percentage: number;
  createdDate: Date;
  lastModifiedDate?: Date;
}