using MediatR;
using FluentResults;

namespace Application.Operation.Features.Admin.HomeContent.Faqs.Commands;

public sealed record UpdateFaqStatusCommand(Guid FaqId, bool IsActive) : IRequest<IResult<Unit>>;

