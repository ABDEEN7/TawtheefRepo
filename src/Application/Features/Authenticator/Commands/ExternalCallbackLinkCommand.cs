using System;
using CSharpFunctionalExtensions;
using MediatR;
using Tawtheef.Application.Features.Authenticator.DTOs;

namespace Tawtheef.Application.Features.Authenticator.Commands;

public record ExternalCallbackLinkCommand(Guid? UserId, string Token, string? ReturnUrl = null, string? RemoteError = null) : IRequest<Result<SocialAccounts>>;