using Application.Operation.Features.Employee.Dashboard.DTOs;
using Application.Operation.Features.Employee.Dashboard.Queries;
using Application.Operation.Features.Employee.Dashboard.Services;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Dashboard.Handlers.Queries;

public sealed class GetCandidateStatusSummaryQueryHandler(IDashboardReadService dashboardReadService)
    : IRequestHandler<GetCandidateStatusSummaryQuery, Result<CandidateStatusSummaryDto>>
{
    public Task<Result<CandidateStatusSummaryDto>> Handle(
        GetCandidateStatusSummaryQuery request,
        CancellationToken ct) => dashboardReadService.GetCandidateStatusAsync(request, ct);
}
