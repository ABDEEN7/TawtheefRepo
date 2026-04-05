using Application.Operation.Features.Employee.Kawader.DTOs;
using MediatR;
using FluentResults;
using Microsoft.AspNetCore.Http;

namespace Application.Operation.Features.Employee.Kawader.Commands;

public sealed record UploadKawaderUserCommand(IFormFile File) 
    : IRequest<IResult<KawaderUploadResultDto>>;

