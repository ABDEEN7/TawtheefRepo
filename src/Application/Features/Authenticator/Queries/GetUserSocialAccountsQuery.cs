using System;
using CSharpFunctionalExtensions;
using MediatR;
using Tawtheef.Application.Features.Authenticator.DTOs;

namespace Tawtheef.Application.Features.Authenticator.Queries;

public record GetUserSocialAccountsQuery(Guid UserId) : IRequest<Result<UserSocialAccounts>>;
