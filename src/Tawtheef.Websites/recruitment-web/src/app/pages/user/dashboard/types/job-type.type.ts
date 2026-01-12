import { JOB_TYPES } from "../constants/constants";

export type JobType = typeof JOB_TYPES[keyof typeof JOB_TYPES];