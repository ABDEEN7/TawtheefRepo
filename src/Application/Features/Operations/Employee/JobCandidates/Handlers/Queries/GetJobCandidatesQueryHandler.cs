using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Extensions;
using Tawtheef.Application.Features.Operations.Employee.JobCandidates.DTOs;
using Tawtheef.Application.Features.Operations.Employee.JobCandidates.Queries;

namespace Tawtheef.Application.Features.Operations.Employee.JobCandidates.Handlers.Queries;

public sealed class GetJobCandidatesQueryHandler(
    IUnitOfWork unitOfWork,
    ILocalizationService localizationService)
    : IRequestHandler<GetJobCandidatesQuery, IResult<PaginatedResult<JobCandidateListItemDto>>>
{
    public async Task<IResult<PaginatedResult<JobCandidateListItemDto>>> Handle(
        GetJobCandidatesQuery request,
        CancellationToken cancellationToken)
    {
        var query = JobCandidatesQueryBuilder.Build(unitOfWork, request.JobId, request.Filter);

        var paginated = await query.ToPaginationListAsync(request.Pagination, cancellationToken);

        var items = paginated.Items.Select(candidate => new JobCandidateListItemDto
        {
            InvitationId = candidate.InvitationId,
            CandidateId = candidate.ApplicantId,
            CandidateName = localizationService.GetLocalizedFullName(candidate.Applicant),
            Department = localizationService.GetLocalizedName(candidate.Job?.Department),
            JobCategory = localizationService.GetLocalizedName(candidate.Job?.JobCategory),
            CandidateCategory = localizationService.GetLocalizedName(candidate.Profile?.CandidateType),
            CandidateMajor = localizationService.GetLocalizedName(candidate.Major),
            CandidateGender = localizationService.GetLocalizedName(candidate.Profile?.Gender),
            Points = candidate.Points
        }).ToList();

        var result = new PaginatedResult<JobCandidateListItemDto>(
            items,
            paginated.Metadata.TotalCount,
            paginated.Metadata.CurrentPage,
            paginated.Metadata.PageSize);

        return Result.Ok(result);
    }
}
