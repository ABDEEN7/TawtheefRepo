using FluentResults;
using MediatR;
using Tawtheef.Application.Features.Operations.Admin.Languages.DTOs;

namespace Tawtheef.Application.Features.Operations.Admin.Languages.Queries;

public sealed record GetLanguageDetailsQuery(Guid Id) : IRequest<IResult<LanguageAdminDto>>;
