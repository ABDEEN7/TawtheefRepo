namespace Tawtheef.Application.Common.Services;

public static class LocalPathBuilder
{
    #region Recruitment
    
    public static string UserProfile(Guid userId, string category, Guid fileId, string ext, string hash, bool isPublic)
        => Key(Scope(isPublic), "recruitment", "u", userId.ToString(), "profile", category, FileName(fileId, hash, ext));

    #endregion

    #region Operation
    
    public static string JobReview( Guid jobId, Guid fileId, string ext, string hash, bool isPublic)
        => Key( Scope(isPublic), "operation", "j", jobId.ToString(), "reviews", FileName(fileId, hash, ext));

    public static string UniversityLogo(Guid universityId, string logoType, Guid fileId, string ext, string hash, bool isPublic)
        => Key(Scope(isPublic), "operation", "universities", universityId.ToString(), "logos", logoType, FileName(fileId, hash, ext));

    #endregion

    private static string Scope(bool isPublic) => isPublic ? "public" : "private";

    private static string FileName(Guid fileId, string hash, string ext)
        => $"{fileId:N}_{hash}.{ext.TrimStart('.')}";

    // Cross-platform join, then normalize to forward slashes for blob keys/URLs
    private static string Key(params string[] parts)
        => Path.Join(parts).Replace('\\', '/');

}
