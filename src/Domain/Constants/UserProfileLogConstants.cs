namespace Tawtheef.Domain.Constants;

public static class UserProfileLogConstants
{
    public static class ActionTypes
    {
        public const string ProfileUnassigned = "ProfileUnassigned";
        public const string ProfileAssigned = "ProfileAssigned";
        public const string ProfileReviewStarted = "ProfileReviewStarted";
        public const string ProfileReviewFinalized = "ProfileReviewFinalized";
        public const string OpenProfile = "OpenProfile";
        public const string OpenProfileChangeReview = "OpenProfileChangeReview";
        public const string ReviewItemDecision = "ReviewItemDecision";
        public const string ReviewSectionDecision = "ReviewSectionDecision";
        public const string ProfileChangeRequested = "ProfileChangeRequested";
        public const string ProfileChangeUpdated = "ProfileChangeUpdated";
    }

    public static class Notes
    {
        public const string ReturnedToDistribution = "Profile change request moved back to distribution";
        public const string ProfileResubmittedToDistribution = "Profile resubmitted and returned to distribution";
        public const string ProfileAssignedAutomatically = "Profile automatically assigned to reviewer";
        public const string ProfileAssignedManually = "Profile manually assigned to reviewer";
        public const string ProfileReassignedManually = "Profile reassigned to reviewer (manual)";
        public const string ProfileReviewStarted = "Profile moved to under review";
        public const string ProfileReviewFinalizedWithCorrections = "Profile review finalized with corrections requested";
        public const string ProfileReviewFinalizedApproved = "Profile review finalized as approved";
        public const string ProfileOpenedForReview = "Profile opened for review";
        public const string ProfileOpenedForChangeReview = "Profile opened for change requests review";
        public const string AssignmentClosed = "Assignment closed when review finalized";
        public const string AssignmentDeactivatedBeforeReassignment = "Existing assignment deactivated before reassignment";
        public const string AssignmentDeactivatedBeforeManualReassignment = "Existing assignment deactivated before manual reassignment";
    }

    public static class Sections
    {
        public const string Assignment = "Assignment";
    }
}
