using Application.Operation.Features.Admin.JobTitles.DTOs;
using MediatR;
using FluentResults;

namespace Application.Operation.Features.Admin.JobTitles.Queries;

public sealed record GetListJobTitlesQuery : IRequest<IResult<List<JobTitleAdminDto>>>;

