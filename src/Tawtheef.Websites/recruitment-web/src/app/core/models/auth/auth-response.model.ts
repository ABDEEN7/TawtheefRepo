import {UserInfoModel} from "../../../shared/models/user-info.model";
import {TokenModel} from "./token.model";

export interface AuthResponse {
  requiresProfileCompletion: boolean;
  user: UserInfoModel;
  token: TokenModel;
  missingFields: string[];
  prefill: PrefillData;
}

export interface AuthBootstrap {
  requiresProfileCompletion: boolean;
  missingFields: string[];
  prefill:  PrefillData | null;
}

export interface PrefillData
{
  email?: string | null;
  givenNameEn?: string | null;
  familyNameEn?: string | null;
  avatar?: string | null;
  phoneE164?: string | null;
  nationality?: string | null;
  passportNo?: string | null;
  qid?: string | null;
  locale?: string | null;
  provider?: string | null;
}
