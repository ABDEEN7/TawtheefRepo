using Application.Operation.Features.Employee.JobManagement.Job.DTOs;
using MediatR;
using FluentResults;

namespace Application.Operation.Features.Employee.JobManagement.Job.Commands;

public record UpdateJobBenefitsCommand(Guid JobId, UpdateJobBenefitsDto Data) : IRequest<IResult<Unit>>;
