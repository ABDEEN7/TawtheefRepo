import { AutoAssignRequest, ManualAssignRequest } from './profile-distribution-assignment.model';

export type DistributionDialogResult =
  | { kind: 'manual'; payload: ManualAssignRequest }
  | { kind: 'auto'; payload: AutoAssignRequest }
  | null;
