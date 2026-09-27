using Tawtheef.Application.Features.Authenticator.DTOs.Responses;

namespace Application.Recruitment.Features.CandidateExams.DTOs;

public sealed record CandidateExamQuestionOptionDto(Guid OptionId, string OptionText, int DisplayOrder,
    FileRefDto? Resource);
