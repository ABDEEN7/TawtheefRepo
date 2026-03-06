using Application.Operation.Features.Admin.JobTitles.DTOs;
using MediatR;
using FluentResults;

namespace Application.Operation.Features.Admin.JobTitles.Commands;

public sealed record InsertJobTitleCommand(
    string JobNumber,
    string JobNameAr,
    string JobNameEn) : IRequest<IResult<JobTitleAdminDto>>;

