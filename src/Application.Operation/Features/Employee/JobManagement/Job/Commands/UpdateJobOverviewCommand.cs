using Application.Operation.Features.Employee.JobManagement.Job.DTOs;
using MediatR;
using FluentResults;

namespace Application.Operation.Features.Employee.JobManagement.Job.Commands;

public record UpdateJobOverviewCommand(Guid JobId, UpdateJobOverviewDto Data) : IRequest<IResult<Unit>>;
