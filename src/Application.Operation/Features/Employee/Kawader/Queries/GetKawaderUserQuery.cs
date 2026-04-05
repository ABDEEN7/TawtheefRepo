using Application.Operation.Features.Employee.Kawader.DTOs;
using MediatR;
using Tawtheef.Application.Common.Models.Pagination;
namespace Application.Operation.Features.Employee.Kawader.Queries;

public record GetKawaderUserQuery(string? SearchTerm) 
    : PaginatedRequest, IRequest<PaginatedResult<KawaderUserDto>>;
