using Application.Operation.Features.Admin.JobTitles.DTOs;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Application.Operation.Features.Admin.JobTitles.Commands;

public sealed record InsertJobTitleCommand(
    string JobNumber,
    string JobNameAr,
    string JobNameEn) : ICommand<IResult<JobTitleAdminDto>>;
