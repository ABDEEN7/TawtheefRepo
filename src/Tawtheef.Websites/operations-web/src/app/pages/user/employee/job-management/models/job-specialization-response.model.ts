import { dropdownOptionsModel } from "../../../../../shared/models/dropdown-options.model";
import { GUID } from "../../../../../shared/types/guid.type";

export interface JobSpecializationResponse {
  id: GUID;
  major: dropdownOptionsModel;
  subMajor?: dropdownOptionsModel;
}
