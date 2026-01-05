using Cortex.Mediator.Queries;
using FluentResults;

using Tawtheef.Application.Features.Resources.DTOs;

namespace Tawtheef.Application.Features.Resources.Queries;

public record GetSignedBlobQuery(string B, long Exp, string Sig) : IQuery<Result<FileResponse>>;
