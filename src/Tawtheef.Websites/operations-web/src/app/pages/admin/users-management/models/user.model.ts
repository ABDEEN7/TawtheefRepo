export interface UserDto {
  id: string;
  name: string;
  email: string;
  lastLoginDate?: string | null;
  isBlocked: boolean;
  roles: string[];
}

export interface UserFilters {
  pageNumber: number;
  pageSize: number;
  name?: string;
  email?: string;
  isBlocked?: boolean | null;
}
