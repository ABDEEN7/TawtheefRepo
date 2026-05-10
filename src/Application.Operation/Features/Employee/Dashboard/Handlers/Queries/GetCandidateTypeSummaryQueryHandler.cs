using Application.Operation.Features.Employee.Dashboard.DTOs;
using Application.Operation.Features.Employee.Dashboard.Queries;
using Application.Operation.Features.Employee.Dashboard.Services;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Dashboard.Handlers.Queries;

public sealed class GetCandidateTypeSummaryQueryHandler(IDashboardReadService dashboardReadService)
    : IRequestHandler<GetCandidateTypeSummaryQuery, Result<CandidateTypeSummaryDto>>
{
    public Task<Result<CandidateTypeSummaryDto>> Handle(
        GetCandidateTypeSummaryQuery request,
        CancellationToken ct) => dashboardReadService.GetCandidateTypesAsync(request, ct);
}
