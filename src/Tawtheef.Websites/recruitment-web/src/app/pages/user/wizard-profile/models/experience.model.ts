import {GUID} from '../../../../shared/types/guid.type';

export interface Experience {
  id?: GUID;
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
