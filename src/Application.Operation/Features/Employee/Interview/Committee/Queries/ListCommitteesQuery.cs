using System;
using Application.Operation.Features.Employee.Interview.Committee.DTOs;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Interview.Committee.Queries;

public sealed record ListCommitteesQuery(Guid? JobId) : IRequest<IResult<List<CommitteeDto>>>;
