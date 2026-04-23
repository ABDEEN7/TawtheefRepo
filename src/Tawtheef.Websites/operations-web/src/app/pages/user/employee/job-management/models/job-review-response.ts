import { JobReviewAttachmentResponse } from "./job-review-attachment-response";
import { JobTabReviewNoteResponse } from "./job-tab-review-note-response";

export interface JobReviewResponse{
    reviewAttachments : JobReviewAttachmentResponse[]
    tabNoteReviews : JobTabReviewNoteResponse[]
}