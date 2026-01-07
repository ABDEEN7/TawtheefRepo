import {dropdownOptionsModel} from '../../../../../shared/models/dropdown-options.model';
import {UploadedFileRef} from './profile-state.model';
import {GUID} from '../../../../../shared/types/guid.type';

export interface Degree {
  id?: GUID;
  degree: dropdownOptionsModel | null | undefined;
  gradCountry: dropdownOptionsModel | null | undefined;
  university: dropdownOptionsModel | null | undefined;
  major: dropdownOptionsModel | null | undefined;
  subMajor: dropdownOptionsModel | null | undefined;
  gradYear: number;
  studySystem: dropdownOptionsModel | null | undefined;
  gpa: number;
  grade: dropdownOptionsModel | null | undefined;
  certificate?: UploadedFileRef | null;
  file?: File | null;
  fileName?: string | null;
  attachmentId?: string | null;
}
