import { InjectionToken } from '@angular/core';

export type ProfileWriteMode = 'create' | 'change-request' | 'review-edit';

export const PROFILE_WRITE_MODE = new InjectionToken<ProfileWriteMode>('PROFILE_WRITE_MODE');
