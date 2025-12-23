import { AutoAssignRequest, ManualAssignRequest } from './profile-distribution.models';

export type DistributionDialogResult =
  | { kind: 'manual'; payload: ManualAssignRequest }
  | { kind: 'auto'; payload: AutoAssignRequest }
  | null;
