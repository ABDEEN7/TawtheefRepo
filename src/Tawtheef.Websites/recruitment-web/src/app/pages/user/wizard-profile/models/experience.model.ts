export interface Experience {
  org: string;
  title: string;
  from?: string;
  to?: string;
  tasks?: string;
  fileName?: string;
  file?: File | null;
  attachmentId?: string | null;
  current?: boolean;
}
