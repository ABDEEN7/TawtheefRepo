using Application.Operation.Features.Employee.Interview.OperationalIssue.DTOs;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Interview.OperationalIssue.Queries;

public sealed record ListAppointmentOperationalIssuesQuery(Guid AppointmentId) : IRequest<IResult<List<OperationalIssueDto>>>;
