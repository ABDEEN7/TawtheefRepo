using Application.Operation.Features.Employee.MinisterOffice.DTOs;
using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Models.Pagination;

namespace Application.Operation.Features.Employee.MinisterOffice.Queries;

public sealed record GetMinisterOfficeCandidateAuditLogQuery(Guid CandidateId)
    : PaginatedRequest,
        IRequest<IResult<PaginatedResult<MinisterOfficeCandidateAuditLogDto>>>;
