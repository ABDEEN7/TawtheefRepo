using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Models;

namespace Tawtheef.Application.Features.Lookups.Queries;

public sealed record GetQuestionBankRequestStatusesQuery : BaseSearchQuery, IRequest<IResult<List<DropdownOptions>>>;
