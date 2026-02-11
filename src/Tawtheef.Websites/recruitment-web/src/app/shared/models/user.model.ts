import {dropdownOptionsModel, DropdownOptionVM} from "./dropdown-options.model";

export interface userModel {
  id: string;
  name: string;
  email: string;
  gender: string;
  role: string;
  accountStatus: DropdownOptionVM;
  registerAt: Date;
  isVerifiedEmail: boolean;
  isVerifiedPhone: boolean;
}
