using System;
using Application.Operation.Features.Employee.Interview.Committee.DTOs;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Interview.Committee.Queries;

public sealed record GetCommitteeByIdQuery(Guid Id) : IRequest<IResult<CommitteeDto>>;
