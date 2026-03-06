using Application.Operation.Features.Admin.Languages.DTOs;
using MediatR;
using FluentResults;

namespace Application.Operation.Features.Admin.Languages.Queries;

public sealed record GetLanguageDetailsQuery(Guid Id) : IRequest<IResult<LanguageAdminDto>>;

