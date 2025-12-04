export interface Profile {
  id: string;
  name: string;
  major: string;
  entity: string;
  sent: string;
  status: string;
  avatar: string;
}

export interface ProfileListState {
  query: string;
  status: string;
  entity: string;
  page: number;
  size: number;
}
