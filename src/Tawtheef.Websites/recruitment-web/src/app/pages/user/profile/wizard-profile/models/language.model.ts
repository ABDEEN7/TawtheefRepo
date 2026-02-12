import {dropdownOptionsModel, DropdownOptionVM} from '../../../../../shared/models/dropdown-options.model';
import {GUID} from '../../../../../shared/types/guid.type';

export interface Language {
  id?: GUID;
  langId: string;
  lang: DropdownOptionVM | null | undefined;
  speakingLevelId: string;
  speakingLevel: DropdownOptionVM | null | undefined;
  writingLevelId: string;
  writingLevel: DropdownOptionVM | null | undefined;
  readingLevelId: string;
  readingLevel: DropdownOptionVM | null | undefined;
}
