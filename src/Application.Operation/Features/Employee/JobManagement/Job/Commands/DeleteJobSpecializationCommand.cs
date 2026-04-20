using MediatR;
using FluentResults;

namespace Application.Operation.Features.Employee.JobManagement.Job.Commands;

public record DeleteJobSpecializationCommand(Guid SpecializationId) : IRequest<IResult<Unit>>;
