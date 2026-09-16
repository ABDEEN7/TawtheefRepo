using Application.Operation.Features.Employee.Exams.DTOs;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Exams.Queries;

public sealed record GetExamLookupsQuery : IRequest<IResult<ExamLookupsDto>>;
