export interface Attachment {
  id?: string;
  name: string;
  fileName?: string;
  attachmentId?: string;
  type?: string;
  size?: number;
  file?: File;
  url?: string;
}
