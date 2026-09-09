using Application.Operation.Features.Employee.Exams.DTOs;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Exams.Commands;

public sealed record SaveExamCommand(Guid? DraftId, ExamConfigurationDto Exam, bool Submit)
    : IRequest<IResult<SavedExamDto>>;