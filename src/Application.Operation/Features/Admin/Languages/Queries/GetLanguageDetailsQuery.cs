using Application.Operation.Features.Admin.Languages.DTOs;
using Cortex.Mediator.Queries;
using FluentResults;

namespace Application.Operation.Features.Admin.Languages.Queries;

public sealed record GetLanguageDetailsQuery(Guid Id) : IQuery<IResult<LanguageAdminDto>>;
