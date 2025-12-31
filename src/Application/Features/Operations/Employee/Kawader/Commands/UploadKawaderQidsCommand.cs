using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Tawtheef.Application.Features.Operations.Employee.Kawader.DTOs;

namespace Tawtheef.Application.Features.Operations.Employee.Kawader.Commands;

public sealed record UploadKawaderQidsCommand(IFormFile File) : IRequest<IResult<KawaderUploadResultDto>>;
