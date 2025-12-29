export const AUTH_PROVIDER = {
  LOCAL: 'local',
  GOOGLE: 'google',
  FACEBOOK: 'facebook',
} as const;

export type AuthProvider =
  typeof AUTH_PROVIDER[keyof typeof AUTH_PROVIDER];
