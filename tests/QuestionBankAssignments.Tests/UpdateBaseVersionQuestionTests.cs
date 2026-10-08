using Application.Operation.Features.Employee.QuestionBankAssignments.Handlers;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.QuestionsBank;

namespace QuestionBankAssignments.Tests;

public class UpdateBaseVersionQuestionTests
{
    [TestCase(false)]
    [TestCase(true)]
    public async Task PersistsBothForeignKeysForNewAndReclaimedItems(bool reclaim)
    {
        await using var fixture = await AssignmentFixture.Create();
        var existing = reclaim ? await SeedRemovedItem(fixture) : null;
        var initialRevisionCount = reclaim ? 2 : 1;
        fixture.BeforeSave = (stage, _) =>
        {
            if (stage == 1)
            {
                Assert.That(fixture.Db.ChangeTracker.Entries<QuestionRevision>(), Is.Empty);
                fixture.Uow.Verify(x => x.GetEntityRepository<QuestionRevision>(), Times.Never);
                var claim = fixture.Db.ChangeTracker.Entries<QuestionBankRequestItem>().Single();
                Assert.That(claim.State, Is.EqualTo(reclaim ? EntityState.Modified : EntityState.Added));
                Assert.That(claim.Entity.CurrentProposedRevisionId, Is.Null);
            }
            return Task.CompletedTask;
        };
        fixture.AfterSave = async (stage, ct) =>
        {
            if (stage != 1) return;
            // Read the actual first-stage rows inside the still-open transaction.
            var claim = await fixture.Db.Set<QuestionBankRequestItem>().AsNoTracking().SingleAsync(ct);
            Assert.That(claim.CurrentProposedRevisionId, Is.Null);
            Assert.That(claim.QuestionBankAssignmentId, Is.EqualTo(fixture.Assignment.Id));
            Assert.That(claim.OriginalRevisionId, Is.EqualTo(fixture.BaseRevisionId));
            Assert.That(claim.ChangeTypeId, Is.EqualTo(QuestionChangeTypeIds.UPDATE));
            Assert.That(claim.StatusId, Is.EqualTo(QuestionBankRequestItemStatusIds.DRAFT));
            Assert.That(claim.RemovedById, Is.Null);
            Assert.That(claim.RemovedAt, Is.Null);
            Assert.That(claim.RemovalNote, Is.Null);
            Assert.That(await fixture.Db.Set<QuestionRevision>().CountAsync(ct), Is.EqualTo(initialRevisionCount));
            var assignment = await fixture.Db.Set<QuestionBankAssignment>().AsNoTracking()
                .SingleAsync(x => x.Id == fixture.Assignment.Id, ct);
            Assert.That(assignment.StatusId, Is.EqualTo(QuestionBankAssignmentStatusIds.QuestionEntryInProgress));
            Assert.That(assignment.QuestionEntryStartedAt, Is.Not.Null);
        };
        var result = await new UpdateBaseVersionQuestionCommandHandler(fixture.Uow.Object, fixture.User.Object, fixture.Sanitizer.Object)
            .Handle(new(fixture.Assignment.Id, fixture.BaseQuestionId, AssignmentFixture.Input), default);
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(fixture.Saves, Is.EqualTo(2));
        fixture.Db.ChangeTracker.Clear();
        var persisted = await fixture.Db.Set<QuestionBankRequestItem>().Include(x => x.CurrentProposedRevision).SingleAsync();
        Assert.That(persisted.Id, Is.EqualTo(existing?.Id ?? result.Value));
        Assert.That(result.Value, Is.EqualTo(persisted.Id));
        Assert.That(persisted.QuestionBankAssignmentId, Is.EqualTo(fixture.Assignment.Id));
        Assert.That(persisted.OriginalRevisionId, Is.EqualTo(fixture.BaseRevisionId));
        Assert.That(persisted.CurrentProposedRevisionId, Is.Not.EqualTo(fixture.BaseRevisionId));
        Assert.That(persisted.CurrentProposedRevision!.SourceRequestItemId, Is.EqualTo(persisted.Id));
        Assert.That(persisted.CurrentProposedRevision.QuestionTypeId, Is.EqualTo(AssignmentFixture.Input.QuestionTypeId));
        Assert.That(persisted.CurrentProposedRevision.RevisionNo, Is.EqualTo(initialRevisionCount + 1));
        Assert.That(persisted.ChangeTypeId, Is.EqualTo(QuestionChangeTypeIds.UPDATE));
        Assert.That(persisted.StatusId, Is.EqualTo(QuestionBankRequestItemStatusIds.DRAFT));
        Assert.That(persisted.RemovedAt, Is.Null);
        Assert.That(persisted.RemovalNote, Is.Null);
        if (existing is not null)
            Assert.That((await fixture.Db.Set<QuestionRevision>().SingleAsync(x => x.RevisionNo == 2))
                .SourceRequestItemId, Is.EqualTo(existing.Id));
    }

    [TestCase(false)]
    [TestCase(true)]
    public async Task RollsBackFirstSaveWhenSecondSaveFails(bool reclaim)
    {
        await using var fixture = await AssignmentFixture.Create();
        var existing = reclaim ? await SeedRemovedItem(fixture) : null;
        fixture.FailOnSave = 2;
        var handler = new UpdateBaseVersionQuestionCommandHandler(fixture.Uow.Object, fixture.User.Object, fixture.Sanitizer.Object);
        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await handler.Handle(new(fixture.Assignment.Id, fixture.BaseQuestionId, AssignmentFixture.Input), default));
        fixture.Db.ChangeTracker.Clear();
        Assert.That(fixture.Saves, Is.EqualTo(2));
        Assert.That(await fixture.Db.Set<QuestionBankRequestItem>().CountAsync(), Is.EqualTo(reclaim ? 1 : 0));
        Assert.That(await fixture.Db.Set<QuestionRevision>().CountAsync(), Is.EqualTo(reclaim ? 2 : 1));
        var assignment = await fixture.Db.Set<QuestionBankAssignment>().SingleAsync(x => x.Id == fixture.Assignment.Id);
        Assert.That(assignment.StatusId, Is.EqualTo(QuestionBankAssignmentStatusIds.Assigned));
        Assert.That(assignment.QuestionEntryStartedAt, Is.Null);
        if (existing is not null)
        {
            var restored = await fixture.Db.Set<QuestionBankRequestItem>().SingleAsync();
            Assert.That(restored.Id, Is.EqualTo(existing.Id));
            Assert.That(restored.QuestionBankAssignmentId, Is.EqualTo(existing.QuestionBankAssignmentId));
            Assert.That(restored.CurrentProposedRevisionId, Is.EqualTo(existing.CurrentProposedRevisionId));
            Assert.That(restored.OriginalRevisionId, Is.EqualTo(existing.OriginalRevisionId));
            Assert.That(restored.ChangeTypeId, Is.EqualTo(existing.ChangeTypeId));
            Assert.That(restored.StatusId, Is.EqualTo(existing.StatusId));
            Assert.That(restored.RemovedById, Is.EqualTo(existing.RemovedById));
            Assert.That(restored.RemovedAt, Is.EqualTo(existing.RemovedAt));
            Assert.That(restored.RemovalNote, Is.EqualTo(existing.RemovalNote));
            Assert.That((await fixture.Db.Set<QuestionRevision>().SingleAsync(x => x.RevisionNo == 2))
                .SourceRequestItemId, Is.EqualTo(existing.Id));
        }
    }

    [TestCase(false)]
    [TestCase(true)]
    public async Task FirstSaveClaimConflictReturnsMaintenanceErrorBeforeAllocatingRevision(bool reclaim)
    {
        await using var fixture = await AssignmentFixture.Create();
        var existing = reclaim ? await SeedRemovedItem(fixture) : null;
        fixture.BeforeSave = (_, _) => throw (reclaim
            ? new DbUpdateConcurrencyException("Competing reclaim updated the row version")
            : new DbUpdateException("Claim insert failed", new Exception(
                "Cannot insert duplicate key row with unique index 'IX_QuestionBankRequestItem_RequestId_QuestionId'")));
        var result = await new UpdateBaseVersionQuestionCommandHandler(fixture.Uow.Object, fixture.User.Object, fixture.Sanitizer.Object)
            .Handle(new(fixture.Assignment.Id, fixture.BaseQuestionId, AssignmentFixture.Input), default);
        Assert.That(result.Errors.Single().Message, Is.EqualTo(ErrorsCodes.QuestionBankQuestionAlreadyAssignedForMaintenance));
        Assert.That(fixture.Saves, Is.EqualTo(1));
        fixture.Uow.Verify(x => x.GetEntityRepository<QuestionRevision>(), Times.Never);
        fixture.Db.ChangeTracker.Clear();
        Assert.That(await fixture.Db.Set<QuestionRevision>().CountAsync(), Is.EqualTo(reclaim ? 2 : 1));
        Assert.That(await fixture.Db.Set<QuestionBankRequestItem>().CountAsync(), Is.EqualTo(reclaim ? 1 : 0));
        if (existing is not null)
            Assert.That((await fixture.Db.Set<QuestionBankRequestItem>().SingleAsync()).StatusId,
                Is.EqualTo(QuestionBankRequestItemStatusIds.REMOVED_FROM_REQUEST));
    }

    [Test]
    public async Task UnrelatedRevisionConstraintFailureIsNotReportedAsClaimConflict()
    {
        await using var fixture = await AssignmentFixture.Create();
        var failure = new DbUpdateException("Revision insert failed", new Exception(
            "Cannot insert duplicate key row with unique index 'IX_QuestionRevision_QuestionId_RevisionNo'"));
        fixture.BeforeSave = (stage, _) => stage == 2 ? throw failure : Task.CompletedTask;
        var handler = new UpdateBaseVersionQuestionCommandHandler(fixture.Uow.Object, fixture.User.Object, fixture.Sanitizer.Object);
        var error = Assert.ThrowsAsync<DbUpdateException>(async () =>
            await handler.Handle(new(fixture.Assignment.Id, fixture.BaseQuestionId, AssignmentFixture.Input), default));
        Assert.That(error, Is.SameAs(failure));
        fixture.Db.ChangeTracker.Clear();
        Assert.That(await fixture.Db.Set<QuestionBankRequestItem>().CountAsync(), Is.Zero);
        Assert.That(await fixture.Db.Set<QuestionRevision>().CountAsync(), Is.EqualTo(1));
    }

    [Test]
    public async Task ExistingClaimStillFailsWithoutSaving()
    {
        await using var fixture = await AssignmentFixture.Create();
        fixture.Db.Add(new QuestionBankRequestItem
        {
            Id = Guid.NewGuid(), RequestId = fixture.Assignment.QuestionBankRequestId,
            QuestionBankAssignmentId = fixture.Assignment.Id, QuestionId = fixture.BaseQuestionId,
            ChangeTypeId = QuestionChangeTypeIds.DELETE, StatusId = QuestionBankRequestItemStatusIds.DRAFT,
            OriginalRevisionId = fixture.BaseRevisionId
        });
        await fixture.Db.SaveChangesAsync();
        fixture.Db.ChangeTracker.Clear();
        var result = await new UpdateBaseVersionQuestionCommandHandler(fixture.Uow.Object, fixture.User.Object, fixture.Sanitizer.Object)
            .Handle(new(fixture.Assignment.Id, fixture.BaseQuestionId, AssignmentFixture.Input), default);
        Assert.That(result.Errors.Single().Message, Is.EqualTo(ErrorsCodes.QuestionBankQuestionAlreadyAssignedForMaintenance));
        Assert.That(fixture.Saves, Is.Zero);
    }

    [Test]
    public async Task BaseQuestionTypeCannotChange()
    {
        await using var fixture = await AssignmentFixture.Create();
        var input = AssignmentFixture.Input with
        {
            QuestionTypeId = QuestionTypeIds.TRUE_FALSE,
            Options = [new("صح", "True", true, 1), new("خطأ", "False", false, 2)]
        };
        var result = await new UpdateBaseVersionQuestionCommandHandler(fixture.Uow.Object, fixture.User.Object, fixture.Sanitizer.Object)
            .Handle(new(fixture.Assignment.Id, fixture.BaseQuestionId, input), default);
        Assert.That(result.Errors.Single().Message, Is.EqualTo(ErrorsCodes.InvalidQuestionBankMaintenanceChange));
        Assert.That(fixture.Saves, Is.Zero);
    }

    [Test]
    public async Task RelationalModelReproducesTheOriginalInsertCycle()
    {
        await using var fixture = await AssignmentFixture.Create();
        var item = new QuestionBankRequestItem
        {
            Id = Guid.NewGuid(), RequestId = fixture.Assignment.QuestionBankRequestId,
            QuestionBankAssignmentId = fixture.Assignment.Id, QuestionId = fixture.BaseQuestionId,
            ChangeTypeId = QuestionChangeTypeIds.UPDATE, StatusId = QuestionBankRequestItemStatusIds.DRAFT,
            OriginalRevisionId = fixture.BaseRevisionId
        };
        var revision = QuestionEntryRules.Revision(fixture.BaseQuestionId, 2, item.Id, AssignmentFixture.Input);
        item.CurrentProposedRevisionId = revision.Id;
        fixture.Db.AddRange(item, revision);
        var error = Assert.ThrowsAsync<InvalidOperationException>(async () => await fixture.Db.SaveChangesAsync());
        Assert.That(error!.Message, Does.Contain("circular dependency"));
    }

    private static async Task<QuestionBankRequestItem> SeedRemovedItem(AssignmentFixture fixture)
    {
        var priorAssignment = new QuestionBankAssignment
        {
            Id = Guid.NewGuid(), EmployeeId = Guid.NewGuid(),
            QuestionBankRequestId = fixture.Assignment.QuestionBankRequestId,
            StatusId = QuestionBankAssignmentStatusIds.QuestionEntryInProgress
        };
        var priorRevision = QuestionEntryRules.Revision(fixture.BaseQuestionId, 2, null, AssignmentFixture.Input);
        var item = new QuestionBankRequestItem
        {
            Id = Guid.NewGuid(), RequestId = fixture.Assignment.QuestionBankRequestId,
            QuestionBankAssignmentId = priorAssignment.Id, QuestionId = fixture.BaseQuestionId,
            ChangeTypeId = QuestionChangeTypeIds.UPDATE, StatusId = QuestionBankRequestItemStatusIds.REMOVED_FROM_REQUEST,
            OriginalRevisionId = fixture.BaseRevisionId, CurrentProposedRevisionId = priorRevision.Id,
            RemovedById = fixture.Assignment.EmployeeId, RemovedAt = DateTime.UtcNow, RemovalNote = "Removed"
        };
        fixture.Db.AddRange(priorAssignment, priorRevision, item);
        await fixture.Db.SaveChangesAsync();
        priorRevision.SourceRequestItemId = item.Id;
        await fixture.Db.SaveChangesAsync();
        fixture.Db.ChangeTracker.Clear();
        return item;
    }
}
