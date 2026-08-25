using System.ComponentModel.DataAnnotations;
using Application.Operation.Features.Employee.Exceptions.DTOs;
using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Domain.Entities.Recruitment;

namespace Application.Operation.Features.Employee.Exceptions.Queries;

public sealed record GetInvitationExceptionsQuery
    : PaginatedRequest,
        IRequest<IResult<PaginatedResult<InvitationExceptionListItemDto>>>
{
    [MaxLength(200)]
    public string? Search { get; init; }

    public InvitationExceptionStatus? Status { get; init; }
}
