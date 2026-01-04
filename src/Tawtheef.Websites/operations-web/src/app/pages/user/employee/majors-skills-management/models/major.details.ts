import {dropdownOptionsModel} from "../../../../../shared/models/dropdown-options.model";

export interface majorDetails extends dropdownOptionsModel {
  parentId?: string | null;
  parent?: dropdownOptionsModel | null;
}
