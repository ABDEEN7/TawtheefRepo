import {GUID} from '../../../../../shared/types/guid.type';
import {dropdownOptionsModel} from '../../../../../shared/models/dropdown-options.model';
import {UploadedFileRef} from './profile-state.model';

export interface Experience {
  id?: GUID;
  employerName: string;
  jobTitle: string;
  from?: string;
  to?: string;
  country: dropdownOptionsModel | null | undefined;
  description?: string;
  fileName?: string;
  file?: File | null;
  attachmentId?: string | null;
  attachment?: UploadedFileRef | null;
  current?: boolean;
  qualificationId?: GUID | null;
  qualificationName?: string | null;
}
export interface TrainingCourse {
  id?: GUID;
  title: string;
  provider: string;
  country: dropdownOptionsModel | null | undefined;
  from?: string;
  to?: string;
  description?: string;

  fileName?: string;
  file?: File | null;
  attachmentId?: string | null;
  attachment?: UploadedFileRef | null;
}
