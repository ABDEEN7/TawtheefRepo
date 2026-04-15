using Application.Operation.Features.Employee.JobManagement.Job.DTOs;
using MediatR;
using FluentResults;

namespace Application.Operation.Features.Employee.JobManagement.Job.Commands;

public record UpdateJobAttachmentsCommand(Guid JobId, UpdateJobAttachmentsDto Data) : IRequest<IResult<Unit>>;
