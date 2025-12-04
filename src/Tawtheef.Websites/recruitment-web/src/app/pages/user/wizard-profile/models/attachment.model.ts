import {UploadedFileRef} from './profile-state.model';

export interface Attachment {
  id?: string;
  name: string;
  fileName?: string;
  attachmentId?: string;
  type?: string;
  size?: number;
  file?: File;
  fileRef?: UploadedFileRef | null;
}
