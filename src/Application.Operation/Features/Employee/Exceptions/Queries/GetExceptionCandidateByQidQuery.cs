using Application.Operation.Features.Employee.Exceptions.DTOs;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Exceptions.Queries;

public sealed record GetExceptionCandidateByQidQuery(string Qid)
    : IRequest<IResult<ExceptionCandidateLookupDto>>;
