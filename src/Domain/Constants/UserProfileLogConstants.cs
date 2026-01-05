namespace Tawtheef.Domain.Constants;

public static class UserProfileLogConstants
{
    public static class ActionTypes
    {
        public const string ProfileUnassigned = "ProfileUnassigned";
        public const string ProfileChangeRequested = "ProfileChangeRequested";
        public const string ProfileChangeUpdated = "ProfileChangeUpdated";
    }

    public static class Notes
    {
        public const string ReturnedToDistribution = "Profile change request moved back to distribution";
    }
}
