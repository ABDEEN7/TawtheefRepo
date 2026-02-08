namespace Application.Operation.Features.Admin.HomeContent.Faqs.DTOs;

public sealed record FAQAdminDto(
    Guid Id,
    string QuestionAr,
    string QuestionEn,
    string AnswerAr,
    string AnswerEn,
    int DisplayOrder,
    bool IsActive);
