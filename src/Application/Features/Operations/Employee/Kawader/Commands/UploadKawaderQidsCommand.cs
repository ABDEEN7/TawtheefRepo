using Cortex.Mediator.Commands;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Tawtheef.Application.Features.Operations.Employee.Kawader.DTOs;

namespace Tawtheef.Application.Features.Operations.Employee.Kawader.Commands;

public sealed record UploadKawaderQidsCommand(IFormFile File) : ICommand<IResult<KawaderUploadResultDto>>;
