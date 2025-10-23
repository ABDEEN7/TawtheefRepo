import {UserInfoModel} from "../../../../shared/models/user-info.model";
import {TokenModel} from "./token.model";
import {UnverifiedEmailDataModel} from "./unverified-email-data.model";

export interface LoginResponse {
  user: UserInfoModel;
  token: TokenModel;
}
