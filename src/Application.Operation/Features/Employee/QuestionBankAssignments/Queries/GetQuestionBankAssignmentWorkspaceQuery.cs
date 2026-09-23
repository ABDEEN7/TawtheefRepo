using Application.Operation.Features.Employee.QuestionBankAssignments.DTOs;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.QuestionBankAssignments.Queries;

public sealed record GetQuestionBankAssignmentWorkspaceQuery(Guid AssignmentId)
    : IRequest<IResult<QuestionBankAssignmentWorkspaceDto>>;
