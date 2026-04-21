using Application.Operation.Features.Employee.JobManagement.JobOperations.DTOs;
using MediatR;
using FluentResults;

namespace Application.Operation.Features.Employee.JobManagement.JobOperations.Commands;

public record UpdateJobOverviewCommand(Guid JobId, UpdateJobOverviewDto Data) : IRequest<IResult<Unit>>;
