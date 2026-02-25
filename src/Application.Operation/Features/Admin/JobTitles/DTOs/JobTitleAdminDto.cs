namespace Application.Operation.Features.Admin.JobTitles.DTOs;

public sealed record JobTitleAdminDto(
    Guid Id,
    string JobNumber,
    string JobNameAr,
    string JobNameEn,
    bool IsActive);
