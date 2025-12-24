export interface UserInfoModel {
  userId: string;
  email: string;
  fullName: string;
  profilePictureUrl: string | null;
  notifications: number;
  provider: string;
}
