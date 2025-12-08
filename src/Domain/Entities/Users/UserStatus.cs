namespace Tawtheef.Domain.Entities.Users;

public enum UserProfileStatus
{
    InCreation = 0, // قيد الانشاء
    Submitted = 1, // تم تقديم الطلب
    UnderReview = 2, // قيد المراجعه
    RequiresUpdate = 3, // مطلوب التعديل
    Approved = 4, // معتمد
    Rejected = 5, // مرفوض
    Cancelled = 6, // ملغي
    AdminCancelled = 7 // ملغي اداريا
}
