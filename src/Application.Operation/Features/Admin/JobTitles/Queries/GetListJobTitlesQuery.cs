using Application.Operation.Features.Admin.JobTitles.DTOs;
using Cortex.Mediator.Queries;
using FluentResults;

namespace Application.Operation.Features.Admin.JobTitles.Queries;

public sealed record GetListJobTitlesQuery : IQuery<IResult<List<JobTitleAdminDto>>>;
