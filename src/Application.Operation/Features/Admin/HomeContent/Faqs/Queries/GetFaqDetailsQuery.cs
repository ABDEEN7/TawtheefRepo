using Application.Operation.Features.Admin.HomeContent.Faqs.DTOs;
using MediatR;
using FluentResults;

namespace Application.Operation.Features.Admin.HomeContent.Faqs.Queries;

public sealed record GetFaqDetailsQuery(Guid FaqId) : IRequest<IResult<FAQAdminDto>>;

