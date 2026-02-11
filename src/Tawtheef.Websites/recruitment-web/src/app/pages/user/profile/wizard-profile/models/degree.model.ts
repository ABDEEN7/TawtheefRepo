import {DropdownOptionVM} from '../../../../../shared/models/dropdown-options.model';
import {UploadedFileRef} from './profile-state.model';
import {GUID} from '../../../../../shared/types/guid.type';

export interface Degree {
  id?: GUID;
  degree: DropdownOptionVM | null | undefined;
  gradCountry: DropdownOptionVM | null | undefined;
  university: DropdownOptionVM | null | undefined;
  major: DropdownOptionVM | null | undefined;
  subMajor: DropdownOptionVM | null | undefined;
  gradYear: number;
  studySystem: DropdownOptionVM | null | undefined;
  gpa: number;
  grade: DropdownOptionVM | null | undefined;
  certificate?: UploadedFileRef | null;
  file?: File | null;
  fileName?: string | null;
  attachmentId?: string | null;
}
