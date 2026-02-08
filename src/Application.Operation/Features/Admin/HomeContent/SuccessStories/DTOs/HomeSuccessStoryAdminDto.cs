namespace Application.Operation.Features.Admin.HomeContent.SuccessStories.DTOs;

public sealed record HomeSuccessStoryAdminDto(
    Guid Id,
    string NameAr,
    string NameEn,
    string RoleAr,
    string RoleEn,
    string MetricTitleAr,
    string MetricTitleEn,
    string MetricDescriptionAr,
    string MetricDescriptionEn,
    string ImageUrl,
    int DisplayOrder,
    bool IsActive);
