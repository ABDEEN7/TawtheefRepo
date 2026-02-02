import {GUID} from '../types/guid.type';

export interface dropdownOptionsModel {
  id: GUID;
  backendName: string;
  name: string;
  description: string;
  additionalData?: {
    nameAr?: string;
    nameEn?: string;
    [key: string]: unknown;
  } | null | undefined;
}
