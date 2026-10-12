using Application.Operation.Features.Employee.QuestionBankAssignments.Handlers;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.QuestionsBank;

namespace QuestionBankAssignments.Tests;

public class AssignmentAccessTests
{
    [TestCase(0)]
    [TestCase(1)]
    [TestCase(2)]
    public async Task FinalAssignmentsRejectEveryEmployeeHandlerBeforeReturningQuestionContent(int finalState)
    {
        await using var fixture = await AssignmentFixture.Create();
        var item = await fixture.AddChange(QuestionChangeTypeIds.ADD);
        var assignment = await fixture.Db.Set<QuestionBankAssignment>().Include(a => a.QuestionBankRequest).SingleAsync();
        assignment.StatusId = finalState == 1
            ? QuestionBankAssignmentStatusIds.QuestionEntryCompleted : QuestionBankAssignmentStatusIds.Completed;
        assignment.QuestionBankRequest.StatusId = finalState == 0
            ? QuestionBankRequestStatusIds.PendingReview : QuestionBankRequestStatusIds.Issued;
        await fixture.Db.SaveChangesAsync();
        fixture.Db.ChangeTracker.Clear();

        var uow = fixture.Uow.Object;
        var user = fixture.User.Object;
        var sanitizer = fixture.Sanitizer.Object;
        var id = fixture.Assignment.Id;
        IResultBase[] results =
        [
            await new GetQuestionBankAssignmentWorkspaceQueryHandler(uow, user).Handle(new(id), default),
            await new AddQuestionToAssignmentCommandHandler(uow, user, sanitizer).Handle(new(id, AssignmentFixture.Input), default),
            await new EditAssignmentQuestionCommandHandler(uow, user, sanitizer).Handle(new(id, item.Id, AssignmentFixture.Input), default),
            await new RemoveAssignmentQuestionCommandHandler(uow, user).Handle(new(id, item.Id), default),
            await new UpdateBaseVersionQuestionCommandHandler(uow, user, sanitizer).Handle(new(id, fixture.BaseQuestionId, AssignmentFixture.Input), default),
            await new DeleteBaseVersionQuestionCommandHandler(uow, user).Handle(new(id, fixture.BaseQuestionId), default),
            await new FinishQuestionBankAssignmentEntryCommandHandler(uow, user).Handle(new(id), default),
            await new FinishQuestionBankAssignmentModificationCommandHandler(uow, user, sanitizer).Handle(new(id), default)
        ];
        Assert.Multiple(() =>
        {
            foreach (var result in results)
            {
                Assert.That(result.IsFailed, Is.True);
                Assert.That(result.Errors.Single().Metadata["StatusCode"], Is.EqualTo(403));
            }
            Assert.That(fixture.Saves, Is.Zero);
        });
    }

    [Test]
    public async Task ActiveSubmittedAndReturnedAssignmentsRemainAccessibleAndListFlagUsesTheSameSqlRule()
    {
        await using var fixture = await AssignmentFixture.Create();
        var assignment = await fixture.Db.Set<QuestionBankAssignment>().Include(a => a.QuestionBankRequest).SingleAsync();
        foreach (var status in new[]
                 {
                     QuestionBankAssignmentStatusIds.Assigned,
                     QuestionBankAssignmentStatusIds.QuestionEntryInProgress,
                     QuestionBankAssignmentStatusIds.QuestionEntryCompleted,
                     QuestionBankAssignmentStatusIds.ReturnedForModification,
                     QuestionBankAssignmentStatusIds.ModificationCompleted
                 })
        {
            assignment.StatusId = status;
            await fixture.Db.SaveChangesAsync();
            var access = await AssignmentAccess.ValidateAsync(fixture.Uow.Object, assignment.Id, assignment.EmployeeId, default);
            Assert.That(access.IsSuccess, Is.True, status.ToString());
            var accessibleIds = fixture.Db.Set<QuestionBankAssignment>().Where(AssignmentAccess.Accessible).Select(a => a.Id);
            var flags = await fixture.Db.Set<QuestionBankAssignment>().Select(a => accessibleIds.Contains(a.Id)).ToListAsync();
            Assert.That(flags, Is.EqualTo(new[] { true }));
        }
        assignment.QuestionBankRequest.StatusId = QuestionBankRequestStatusIds.Issued;
        await fixture.Db.SaveChangesAsync();
        Assert.That(await fixture.Db.Set<QuestionBankAssignment>().Where(AssignmentAccess.Accessible).AnyAsync(), Is.False);
    }

    [Test]
    public async Task AssignmentOwnershipStillApplies()
    {
        await using var fixture = await AssignmentFixture.Create();
        var access = await AssignmentAccess.ValidateAsync(fixture.Uow.Object, fixture.Assignment.Id, Guid.NewGuid(), default);
        Assert.That(access.IsFailed, Is.True);
        Assert.That(access.Errors.Single().Metadata["StatusCode"], Is.EqualTo(404));
    }
}
