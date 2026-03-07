using MediatR;
using FluentResults;

namespace Application.Operation.Features.Admin.JobTitles.Commands;

public sealed record DeleteJobTitleCommand(Guid Id) : IRequest<IResult<bool>>;

