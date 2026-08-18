using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Models.Export;

namespace Application.Operation.Features.Employee.JobManagement.JobOperations.Queries;

public sealed record ExportJobsQuery(
    JobQueryFilter? Filter,
    string? SortBy,
    string? SortDirection) : IRequest<IResult<FileExportResult>>;
