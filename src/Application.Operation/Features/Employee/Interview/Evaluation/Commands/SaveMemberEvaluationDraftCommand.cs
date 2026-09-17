using Application.Operation.Features.Employee.Interview.Evaluation.DTOs;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Interview.Evaluation.Commands;

public sealed record SaveMemberEvaluationDraftCommand(
    Guid AppointmentId,
    IReadOnlyCollection<CriterionScoreInputDto> Scores,
    string? GeneralNotes) : IRequest<IResult<Guid>>;
