import {dropdownOptionsModel} from '../../../../shared/models/dropdown-options.model';
import {UploadedFileRef} from './profile-state.model';

export interface Degree {
  degree: dropdownOptionsModel | undefined;
  gradCountry: dropdownOptionsModel | undefined;
  university: dropdownOptionsModel | undefined;
  major: dropdownOptionsModel | undefined;
  subMajor: dropdownOptionsModel | undefined;
  gradYear: number;
  studySystem: dropdownOptionsModel | undefined;
  gpa: number;
  grade: dropdownOptionsModel | undefined;
  certificateName?: string;
  certificate?: UploadedFileRef | null;
  file?: File | null;
  attachmentId?: string | null;
}
