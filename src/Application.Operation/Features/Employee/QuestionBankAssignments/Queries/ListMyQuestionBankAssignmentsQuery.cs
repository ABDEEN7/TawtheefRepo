using Application.Operation.Features.Employee.QuestionBankAssignments.DTOs;
using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Models.Pagination;

namespace Application.Operation.Features.Employee.QuestionBankAssignments.Queries;

public sealed record ListMyQuestionBankAssignmentsQuery : PaginatedRequest,
    IRequest<IResult<PaginatedResult<MyQuestionBankAssignmentDto>>>;
