using Application.Operation.Features.Employee.JobManagement.JobOperations.DTOs;
using MediatR;
using FluentResults;

namespace Application.Operation.Features.Employee.JobManagement.JobOperations.Commands;

public record UpdateJobSkillsCommand(Guid JobId, UpdateJobSkillsDto Data) : IRequest<IResult<Unit>>;
