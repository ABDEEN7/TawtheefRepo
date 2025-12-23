using FluentResults;
using MediatR;

namespace Tawtheef.Application.Features.Recruitment.Profile.Command.DeleteOperation;

public sealed record DeleteProfileEducationCommand(Guid DegreeId): IRequest<IResult<Unit>>;
