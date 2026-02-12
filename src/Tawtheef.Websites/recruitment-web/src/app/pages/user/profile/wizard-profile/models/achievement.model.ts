import {dropdownOptionsModel, DropdownOptionVM} from '../../../../../shared/models/dropdown-options.model';
import {UploadedFileRef} from './profile-state.model';
import {GUID} from '../../../../../shared/types/guid.type';

export interface Achievement {
  id?: GUID;
  achievementTypeId: GUID;
  achievementType: DropdownOptionVM | null;
  title: string;
  issuingAuthority: string;
  countryId: GUID;
  country: DropdownOptionVM | null;
  issueDate?: string;
  description?: string;
  fileName?: string | null;
  file?: File | null;
  attachment?: UploadedFileRef | null;
  attachmentId?: GUID | null;
  relatedToSpecialization?: boolean | null;
}
