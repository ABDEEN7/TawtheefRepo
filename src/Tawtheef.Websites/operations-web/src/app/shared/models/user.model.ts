import {dropdownOptionsModel} from "./dropdown-options.model";

export interface userModel {
  id: string;
  name: string;
  email: string;
  gender: string;
  role: string;
  accountStatus: dropdownOptionsModel;
  registerAt: Date;
  isVerifiedEmail: boolean;
  isVerifiedPhone: boolean;
}
