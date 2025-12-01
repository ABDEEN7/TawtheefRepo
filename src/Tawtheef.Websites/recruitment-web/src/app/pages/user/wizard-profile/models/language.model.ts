import {dropdownOptionsModel} from '../../../../shared/models/dropdown-options.model';

export interface Language {
  langId: string;
  lang: dropdownOptionsModel | null | undefined;
  levelId: string;
  level: dropdownOptionsModel | null | undefined;
}
