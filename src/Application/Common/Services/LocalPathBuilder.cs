using System;
using System.IO;

namespace Tawtheef.Application.Common.Services;

public static class LocalPathBuilder
{
    public static string UserProfile(Guid userId, string category, Guid fileId, string ext, string hash, bool isPublic)
        => Key(Scope(isPublic), "u", userId.ToString(), "profile", category, FileName(fileId, hash, ext));

    public static string Ticket(Guid userId, Guid ticketId, Guid fileId, string ext, string hash, bool isPublic)
        => Key(Scope(isPublic), "u", userId.ToString(), "tickets", ticketId.ToString(), FileName(fileId, hash, ext));

    public static string Invoice(Guid userId, Guid invoiceId, Guid fileId, string ext, string hash, bool isPublic)
        => Key(Scope(isPublic), "u", userId.ToString(), "invoices", invoiceId.ToString(), FileName(fileId, hash, ext));

    public static string UniversityProfile(Guid uniId, Guid fileId, string ext, string hash, bool isPublic)
        => Key(Scope(isPublic), "uni", uniId.ToString(), "profile", FileName(fileId, hash, ext));

    public static string CourseCover(Guid courseId, Guid fileId, string ext, string hash, bool isPublic)
        => Key(Scope(isPublic), "c", courseId.ToString(), "cover", FileName(fileId, hash, ext));

    public static string LessonFile(Guid courseId, Guid versionLineId, Guid topicId, Guid lessonId,
        Guid fileId, string ext, string hash, bool isPublic)
        => Key(Scope(isPublic), "c", courseId.ToString(), "v", versionLineId.ToString(),
            "topics", topicId.ToString(), "lessons", lessonId.ToString(),
            "files", FileName(fileId, hash, ext));

    private static string Scope(bool isPublic) => isPublic ? "public" : "private";

    private static string FileName(Guid fileId, string hash, string ext)
        => $"{fileId:N}_{hash}.{ext.TrimStart('.')}";

    // Cross-platform join, then normalize to forward slashes for blob keys/URLs
    private static string Key(params string[] parts)
        => Path.Join(parts).Replace('\\', '/');
}
