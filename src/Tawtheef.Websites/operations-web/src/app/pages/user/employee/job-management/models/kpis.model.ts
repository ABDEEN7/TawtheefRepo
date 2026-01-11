import { GUID } from "../../../../../shared/types/guid.type";

export interface KPIs {
  jobId : GUID | null;
  total: number;
  applied: number;
  declined: number;
  viewed: number;
  unseen: number;
  appliedPct: number;
  declinedPct: number;
  viewedPct: number;
  unseenPct: number;
}
