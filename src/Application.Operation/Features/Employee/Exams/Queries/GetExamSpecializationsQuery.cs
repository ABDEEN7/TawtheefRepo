using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Models;
using Tawtheef.Application.Features.Lookups.Queries;

namespace Application.Operation.Features.Employee.Exams.Queries;

public sealed record GetExamSpecializationsQuery : BaseSearchQuery, IRequest<IResult<List<DropdownOptions>>>;
