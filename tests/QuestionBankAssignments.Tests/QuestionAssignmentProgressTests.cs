using Application.Operation.Features.Employee.QuestionBankAssignments.Commands;
using Application.Operation.Features.Employee.QuestionBankAssignments.Handlers;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.QuestionsBank;

namespace QuestionBankAssignments.Tests;

public class QuestionAssignmentProgressTests
{
    [TestCase(2, 0, 2, false)]
    [TestCase(2, 2, 4, false)]
    [TestCase(3, 2, 5, true)]
    public async Task CountAndFinishExcludeDelete(int adds, int updates, int expected, bool canFinish)
    {
        await using var fixture = await AssignmentFixture.Create();
        for (var i = 0; i < adds; i++) await fixture.AddChange(QuestionChangeTypeIds.ADD);
        for (var i = 0; i < updates; i++) await fixture.AddChange(QuestionChangeTypeIds.UPDATE);
        var deletion = await fixture.AddChange(QuestionChangeTypeIds.DELETE);
        fixture.Db.ChangeTracker.Clear();
        var countable = fixture.Db.Set<QuestionBankRequestItem>().Where(QuestionAssignmentProgress.Countable);
        var current = await countable.CountAsync(x => x.QuestionBankAssignmentId == fixture.Assignment.Id &&
                                                     x.RequestId == fixture.Assignment.QuestionBankRequestId);
        Assert.That(current, Is.EqualTo(expected));
        Assert.That(5 - current, Is.EqualTo(5 - expected));
        Assert.That(100 * current / 5, Is.EqualTo(expected * 20));
        var result = await new FinishQuestionBankAssignmentEntryCommandHandler(fixture.Uow.Object, fixture.User.Object)
            .Handle(new(fixture.Assignment.Id), default);
        Assert.That(result.IsSuccess, Is.EqualTo(canFinish));
        if (!canFinish)
            Assert.That(result.Errors.Single().Message, Is.EqualTo(ErrorsCodes.QuestionBankMinimumQuestionCountNotMet));
        Assert.That(await fixture.Db.Set<QuestionBankRequestItem>().CountAsync(), Is.EqualTo(adds + updates + 1));
        var persistedDelete = await fixture.Db.Set<QuestionBankRequestItem>().AsNoTracking().SingleAsync(x => x.Id == deletion.Id);
        Assert.That(persistedDelete.StatusId, Is.EqualTo(canFinish
            ? QuestionBankRequestItemStatusIds.PENDING_REVIEW : QuestionBankRequestItemStatusIds.DRAFT));
    }

    [TestCase(4, false)]
    [TestCase(5, true)]
    public async Task FinishModificationsAlsoExcludesDelete(int usable, bool canFinish)
    {
        await using var fixture = await AssignmentFixture.Create();
        var assignment = await fixture.Db.Set<QuestionBankAssignment>().Include(x => x.QuestionBankRequest).SingleAsync();
        assignment.StatusId = QuestionBankAssignmentStatusIds.ReturnedForModification;
        assignment.QuestionBankRequest.StatusId = QuestionBankRequestStatusIds.ModificationInProgress;
        for (var i = 0; i < usable; i++) await fixture.AddChange(QuestionChangeTypeIds.ADD);
        await fixture.AddChange(QuestionChangeTypeIds.DELETE);
        fixture.Db.ChangeTracker.Clear();
        var result = await new FinishQuestionBankAssignmentModificationCommandHandler(
            fixture.Uow.Object, fixture.User.Object, fixture.Sanitizer.Object).Handle(new(fixture.Assignment.Id), default);
        Assert.That(result.IsSuccess, Is.EqualTo(canFinish));
        if (!canFinish)
            Assert.That(result.Errors.Single().Message, Is.EqualTo(ErrorsCodes.QuestionBankMinimumQuestionCountNotMet));
    }

    [Test]
    public async Task CountRuleScopesAssignmentAndRequestAndTranslatesToCorrelatedSql()
    {
        await using var fixture = await AssignmentFixture.Create();
        var valid = await fixture.AddChange(QuestionChangeTypeIds.ADD);
        var deleted = await fixture.AddChange(QuestionChangeTypeIds.UPDATE);
        deleted.IsDeleted = true;
        await fixture.AddChange(QuestionChangeTypeIds.ADD, QuestionBankRequestItemStatusIds.REJECTED);
        await fixture.AddChange(QuestionChangeTypeIds.UPDATE, QuestionBankRequestItemStatusIds.REMOVED_FROM_REQUEST);
        await fixture.AddChange(QuestionChangeTypeIds.DELETE);
        await fixture.Db.SaveChangesAsync();
        var items = await fixture.Db.Set<QuestionBankRequestItem>().AsNoTracking().ToListAsync();
        items.Add(new() { QuestionBankAssignmentId = Guid.NewGuid(), RequestId = valid.RequestId,
            ChangeTypeId = QuestionChangeTypeIds.ADD, StatusId = QuestionBankRequestItemStatusIds.DRAFT });
        items.Add(new() { QuestionBankAssignmentId = valid.QuestionBankAssignmentId, RequestId = Guid.NewGuid(),
            ChangeTypeId = QuestionChangeTypeIds.ADD, StatusId = QuestionBankRequestItemStatusIds.DRAFT });
        Assert.That(QuestionAssignmentProgress.Count(items, fixture.Assignment.Id, valid.RequestId), Is.EqualTo(1));
        var countable = fixture.Db.Set<QuestionBankRequestItem>().Where(QuestionAssignmentProgress.Countable);
        var counts = await fixture.Db.Set<QuestionBankAssignment>()
            .OrderByDescending(a => countable.Count(i => i.QuestionBankAssignmentId == a.Id && i.RequestId == a.QuestionBankRequestId))
            .Select(a => countable.Count(i => i.QuestionBankAssignmentId == a.Id && i.RequestId == a.QuestionBankRequestId))
            .ToListAsync();
        Assert.That(counts, Is.EqualTo(new[] { 1 }));
    }
}
