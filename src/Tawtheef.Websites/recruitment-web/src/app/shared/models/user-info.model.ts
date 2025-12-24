import {PrefillData} from '../../core/models/auth/auth-response.model';

export interface UserInfoModel {
  userId: string;
  email: string;
  fullName: string;
  profilePictureUrl: string | null;
  notifications: number;
  provider: string;
  missingFields?: string[];
  prefill?:  PrefillData | null;
}
