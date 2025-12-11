export interface UserDto {
  id: string;
  name: string;
  email: string;
  lastLoginDate?: string | null;
  isBlocked: boolean;
}
