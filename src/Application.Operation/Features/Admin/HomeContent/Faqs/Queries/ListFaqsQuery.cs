using Application.Operation.Features.Admin.HomeContent.Faqs.DTOs;
using Cortex.Mediator.Queries;
using FluentResults;

namespace Application.Operation.Features.Admin.HomeContent.Faqs.Queries;

public sealed record ListFaqsQuery : IQuery<IResult<IReadOnlyCollection<FAQAdminDto>>>;
