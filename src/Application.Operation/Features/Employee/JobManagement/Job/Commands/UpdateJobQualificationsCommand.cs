using Application.Operation.Features.Employee.JobManagement.Job.DTOs;
using MediatR;
using FluentResults;

namespace Application.Operation.Features.Employee.JobManagement.Job.Commands;

public record UpdateJobQualificationsCommand(Guid JobId, UpdateJobQualificationsDto Data) : IRequest<IResult<Unit>>;
