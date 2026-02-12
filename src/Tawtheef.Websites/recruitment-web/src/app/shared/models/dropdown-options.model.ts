import {APP_LANGUAGE_KEY} from '../../core/constants/website-storage.const';
import {GUID} from '../types/guid.type';
import {LanguageService} from '../../core/services/language.service';

export interface dropdownOptionsModel {
  id: GUID;
  backendName: string;
  name: string;
  description: string;
  additionalData?: {
    nameAr?: string;
    nameEn?: string;
    [key: string]: unknown;
  } | null;
}

export class DropdownOptionVM {
  id: GUID;
  backendName: string;
  description: string;
  additionalData?: dropdownOptionsModel['additionalData'];

  private baseName: string;

  constructor(dto: dropdownOptionsModel) {
    this.id = dto.id;
    this.backendName = dto.backendName;
    this.description = dto.description;
    this.additionalData = dto.additionalData ?? null;
    this.baseName = dto.name; // ✅ مهم
  }

  get name(): string {
    const l = (LanguageService.safeGet(APP_LANGUAGE_KEY)?.toLowerCase() ?? 'ar');
    const key = l === 'ar' ? 'nameAr' : 'nameEn';
    const v = this.additionalData?.[key];

    return (typeof v === 'string' && v.trim())
      ? v.trim()
      : this.baseName;
  }
}
