using Application.Operation.Features.Employee.Exams.DTOs;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Exams.Queries;

public sealed record GetExamBanksQuery(Guid JobId) : IRequest<IResult<List<ExamBankDto>>>;
