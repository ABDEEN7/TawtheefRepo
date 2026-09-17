using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.QuestionBankRequests.Commands;

public sealed record AssignmentInput(Guid EmployeeId, int MinimumQuestionCount, string? Notes);
public sealed record AssignQuestionBankEmployeesCommand(Guid RequestId, IReadOnlyCollection<AssignmentInput> Assignments) : IRequest<IResult>;
