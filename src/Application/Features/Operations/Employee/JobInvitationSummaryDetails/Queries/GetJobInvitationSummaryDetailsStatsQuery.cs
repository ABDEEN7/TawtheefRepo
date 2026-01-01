using FluentResults;
using MediatR;
using Tawtheef.Application.Features.Operations.Employee.JobInvitationSummaryDetails.DTOs;

namespace Tawtheef.Application.Features.Operations.Employee.JobInvitationSummaryDetails.Queries;

public sealed record GetJobInvitationSummaryDetailsStatsQuery(Guid JobId, string? AcademicYear)
    : IRequest<IResult<JobInvitationSummaryDetailsStatsDto>>;
