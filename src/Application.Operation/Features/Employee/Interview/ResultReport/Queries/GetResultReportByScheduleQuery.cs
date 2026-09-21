using Application.Operation.Features.Employee.Interview.ResultReport.DTOs;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Interview.ResultReport.Queries;

public sealed record GetResultReportByScheduleQuery(Guid ScheduleId) : IRequest<IResult<ResultReportDto>>;
