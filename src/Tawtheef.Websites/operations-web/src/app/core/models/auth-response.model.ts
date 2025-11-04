import {UserInfoModel} from "../../shared/models/user-info.model";
import {TokenModel} from "./token.model";

export interface AuthResponse {
  user: UserInfoModel;
  token: TokenModel;
}
