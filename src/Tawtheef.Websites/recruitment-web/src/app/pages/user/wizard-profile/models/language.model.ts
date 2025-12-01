import {dropdownOptionsModel} from '../../../../shared/models/dropdown-options.model';
import {GUID} from '../../../../shared/types/guid.type';

export interface Language {
  id?: GUID;
  langId: string;
  lang: dropdownOptionsModel | null | undefined;
  levelId: string;
  level: dropdownOptionsModel | null | undefined;
}
