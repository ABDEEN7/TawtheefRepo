using System.Text.Json;
using Application.Operation.Features.Employee.Exams.Commands;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Exams;
using Tawtheef.Domain.Entities.Logger;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Events.Operation.Employee.Exams;

namespace Application.Operation.Features.Employee.Exams.Handlers.Commands;

public sealed class ReturnExamCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    : IRequestHandler<ReturnExamCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(ReturnExamCommand request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Note) || !Guid.TryParse(currentUser.UserId, out var reviewerId))
            return Result.Fail<Unit>(ErrorsCodes.InvalidRequest);

        var exam = await unitOfWork.GetEntityRepository<Exam>().DbSet
            .FirstOrDefaultAsync(x => x.Id == request.ExamId && x.StatusId == ExamStatusIds.PendingApproval, ct);
        if (exam == null)
            return Result.Fail<Unit>(ErrorsCodes.InvalidRequest);

        var note = request.Note.Trim();
        var decisionAt = DateTime.UtcNow;
        exam.StatusId = ExamStatusIds.Returned;
        exam.ApprovedById = reviewerId;
        exam.ApprovedAt = decisionAt;
        exam.DecisionNotes = note;

        await unitOfWork.GetEntityRepository<ActionLog>().AddAsync(new ActionLog
        {
            UserId = reviewerId,
            LogType = ActionLogType.Employee,
            ActionType = "ExamReturnedForEdit",
            Section = "ExamWorkflow",
            EntityId = exam.Id,
            Notes = JsonSerializer.Serialize(new
            {
                examId = exam.Id,
                examNumber = exam.ExamNo,
                previousStatus = nameof(ExamStatusIds.PendingApproval),
                status = nameof(ExamStatusIds.Returned),
                performedBy = reviewerId,
                performedAt = decisionAt,
                returnNote = note,
            }),
        }, ct);

        if (exam.CreatedById.HasValue)
            exam.AddDomainEvent(new ExamReturnedForEditDomainEvent(exam.CreatedById.Value, exam.Id, exam.ExamNo, note));

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Ok(Unit.Value);
    }
}
