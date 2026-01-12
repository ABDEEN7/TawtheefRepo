import { JOB_INVITATION_STATUSES } from "../constants/constants";

export type JobStatus = typeof JOB_INVITATION_STATUSES[keyof typeof JOB_INVITATION_STATUSES];
