import { GUID } from "../../../shared/types/guid.type";

//TODO :: Will Removed later
export interface Application {
  jobId: GUID;
  profileId: GUID;
  appliedAt: string;
  status: string;
}
