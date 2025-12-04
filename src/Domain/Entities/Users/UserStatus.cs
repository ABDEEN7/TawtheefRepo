namespace Tawtheef.Domain.Entities.Users;

public enum UserStatus
{
    InCreation = 0, // قيد الانشاء
    Submitted = 1, // تم تقديم الطلب
    UnderReview = 2, // قيد المراجعه
    RequiresUpdate = 3, // مطلوب التعديل
    Approved = 4, // معتمد
    Rejected = 5, // مرفوض
    Blocked = 6, // محظولا
    Cancelled = 7, // ملغي
    AdminCancelled = 8 // ملغي اداريا
}

public static class UserStatusExtensions
{
    public static bool BlocksLogin(this UserStatus status)
    {
        return status is UserStatus.Blocked
            or UserStatus.Cancelled
            or UserStatus.AdminCancelled
            or UserStatus.Rejected;
    }
}
