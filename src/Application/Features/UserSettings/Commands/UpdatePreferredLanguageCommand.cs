using MediatR;
using FluentResults;

namespace Tawtheef.Application.Features.UserSettings.Commands;

public record UpdatePreferredLanguageCommand(Guid UserId, string PreferredLanguage) : IRequest<IResult<Unit>>;
