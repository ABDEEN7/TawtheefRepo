namespace Application.Recruitment.Features.HomeContent.DTOs;

public sealed record HomeSuccessStoryDto(
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
    int DisplayOrder);

public sealed record FAQDto(
    Guid Id,
    string QuestionAr,
    string QuestionEn,
    string AnswerAr,
    string AnswerEn,
    int DisplayOrder);

public sealed record HomeContentDto(
    IReadOnlyCollection<HomeSuccessStoryDto> SuccessStories,
    IReadOnlyCollection<FAQDto> Faqs);
