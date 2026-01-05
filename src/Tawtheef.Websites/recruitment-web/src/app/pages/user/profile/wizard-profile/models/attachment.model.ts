import {UploadedFileRef} from './profile-state.model';

export interface Attachment {
  id?: string;
  title: string;
  fileName?: string;
  attachmentId?: string;
  type?: string;
  size?: number;
  file?: File;
  fileRef?: UploadedFileRef | null;
}
