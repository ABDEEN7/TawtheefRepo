import {dropdownOptionsModel} from '../../../../../shared/models/dropdown-options.model';
import {GUID} from '../../../../../shared/types/guid.type';

export interface Language {
  id?: GUID;
  langId: string;
  lang: dropdownOptionsModel | null | undefined;
  speakingLevelId: string;
  speakingLevel: dropdownOptionsModel | null | undefined;
  writingLevelId: string;
  writingLevel: dropdownOptionsModel | null | undefined;
  readingLevelId: string;
  readingLevel: dropdownOptionsModel | null | undefined;
}
