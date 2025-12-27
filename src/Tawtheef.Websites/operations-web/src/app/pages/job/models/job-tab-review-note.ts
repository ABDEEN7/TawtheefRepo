import { GUID } from "../../../shared/types/guid.type";
import { JobTabStatus } from "../enums/job-tab-status";
import { JobTabType } from "../enums/job-tab-type";

export interface JobTabReviewNote {
  jobId?: GUID;
  tab: JobTabType;
  note: string;
  tabStatus: JobTabStatus | null;
}