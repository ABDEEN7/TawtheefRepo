import { GUID } from "../../../shared/types/guid.type";
import { Job } from "./job.model";

export interface UpdateJobRequest extends Job {
  id: GUID;
  jobStatusId?: GUID;
  publishAt?: Date | null;
  cancelledAt?: Date | null;
}