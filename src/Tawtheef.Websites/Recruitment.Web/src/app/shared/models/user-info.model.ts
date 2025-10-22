export interface UserInfoModel {
  userId: string;
  email: string;
  firstName: string;
  lastName: string;
  userType: string;
  profilePictureUrl: string | null;
  authProvider: string;
}
