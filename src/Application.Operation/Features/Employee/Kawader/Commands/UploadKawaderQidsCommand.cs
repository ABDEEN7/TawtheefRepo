using Application.Operation.Features.Employee.Kawader.DTOs;
using Cortex.Mediator.Commands;
using FluentResults;
using Microsoft.AspNetCore.Http;

namespace Application.Operation.Features.Employee.Kawader.Commands;

public sealed record UploadKawaderQidsCommand(IFormFile File) : ICommand<IResult<KawaderUploadResultDto>>;
