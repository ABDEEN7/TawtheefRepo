using Application.Operation.Features.Employee.JobManagement.Job.DTOs;
using MediatR;
using FluentResults;

namespace Application.Operation.Features.Employee.JobManagement.Job.Commands;

public record UpdateJobSkillsCommand(Guid JobId, UpdateJobSkillsDto Data) : IRequest<IResult<Unit>>;
