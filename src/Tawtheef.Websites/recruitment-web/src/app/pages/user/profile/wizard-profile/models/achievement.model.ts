import {dropdownOptionsModel} from '../../../../../shared/models/dropdown-options.model';
import {UploadedFileRef} from './profile-state.model';
import {GUID} from '../../../../../shared/types/guid.type';

export interface Achievement {
  id?: GUID;
  achievementType: dropdownOptionsModel | null;
  title: string;
  issuingAuthority: string;
  country: dropdownOptionsModel | null;
  issueDate?: string;
  description?: string;
  fileName?: string | null;
  file?: File | null;
  attachment?: UploadedFileRef | null;
  attachmentId?: string | null;
  relatedToSpecialization?: boolean | null;
}
