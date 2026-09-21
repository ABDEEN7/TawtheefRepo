using Application.Operation.Features.Employee.Interview.ResultReport.DTOs;
using FluentResults;
using MediatR;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.ResultReport.Queries;

public sealed record ListResultReportsQuery(Guid? JobId, ResultReportStatus? Status) : IRequest<IResult<List<ResultReportListItemDto>>>;
