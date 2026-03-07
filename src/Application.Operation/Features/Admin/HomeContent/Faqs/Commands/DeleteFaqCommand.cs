using MediatR;
using FluentResults;

namespace Application.Operation.Features.Admin.HomeContent.Faqs.Commands;

public sealed record DeleteFaqCommand(Guid FaqId) : IRequest<IResult<Unit>>;

