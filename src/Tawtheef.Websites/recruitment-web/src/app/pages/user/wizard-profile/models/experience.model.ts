import {GUID} from '../../../../shared/types/guid.type';

export interface Experience {
  id?: GUID;
  org: string;
  name: string;
  from?: string;
  to?: string;
  description?: string;
  fileName?: string;
  file?: File | null;
  attachmentId?: string | null;
  current?: boolean;
}
