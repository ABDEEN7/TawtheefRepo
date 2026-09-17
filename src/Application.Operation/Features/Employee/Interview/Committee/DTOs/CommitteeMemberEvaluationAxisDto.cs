using System;

namespace Application.Operation.Features.Employee.Interview.Committee.DTOs;

public sealed record CommitteeMemberEvaluationAxisDto(
    Guid Id,
    Guid InterviewTemplateEvaluationAxisId,
    string AxisNameAr,
    string? AxisNameEn);
