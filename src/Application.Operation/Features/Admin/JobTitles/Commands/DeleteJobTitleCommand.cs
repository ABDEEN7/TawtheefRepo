using Cortex.Mediator.Commands;
using FluentResults;

namespace Application.Operation.Features.Admin.JobTitles.Commands;

public sealed record DeleteJobTitleCommand(Guid Id) : ICommand<IResult<bool>>;
