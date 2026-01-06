using Cortex.Mediator.Queries;
using FluentResults;
using Tawtheef.Application.Features.Operations.Admin.Languages.DTOs;

namespace Tawtheef.Application.Features.Operations.Admin.Languages.Queries;

public sealed record GetLanguageDetailsQuery(Guid Id) : IQuery<IResult<LanguageAdminDto>>;
