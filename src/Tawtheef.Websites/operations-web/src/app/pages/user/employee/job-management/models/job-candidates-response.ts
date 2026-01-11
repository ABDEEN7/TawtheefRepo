import { PaginatedResult } from "../../../../../core/models/paginated-result.model";
import { JobCandidateListItem } from "./job-candidate.model";
import { JobCandidatesOverview } from "./job-candidates-overview.model";

export interface JobCandidatesResponse {
  list: PaginatedResult<JobCandidateListItem>;
  overview: JobCandidatesOverview
}
