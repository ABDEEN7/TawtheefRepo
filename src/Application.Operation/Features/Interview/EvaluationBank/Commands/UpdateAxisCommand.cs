using FluentResults;
using MediatR;
namespace Application.Operation.Features.Interview.EvaluationBank.Commands;

public sealed record UpdateAxisCommand(
    Guid Id,
    string NameAr,
    string? NameEn,
    string? DescriptionAr,
    string? DescriptionEn,
    bool IsActive) : IRequest<IResult<Unit>>;
