using Application.Operation.Features.Employee.Interview.ResultReport.DTOs;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Interview.ResultReport.Commands;

public sealed record ApproveInterviewResultReportCommand(
    Guid ReportId, IReadOnlyCollection<CandidateDecisionInputDto> Decisions) : IRequest<IResult<Unit>>;
