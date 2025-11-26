import {PrefillData} from '../../core/models/auth/auth-response.model';

export interface UserInfoModel {
  userId: string;
  email: string;
  firstName: string;
  lastName: string;
  userType: string;
  profilePictureUrl: string | null;
  authProvider: string;

  missingFields?: string[];
  prefill?:  PrefillData | null;
}
