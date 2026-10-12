using Application.Operation.Features.Employee.QuestionBankAssignments.Handlers;
using Application.Operation.Features.Employee.QuestionBankRequests.Handlers.Queries;
using Application.Operation.Features.Employee.QuestionBankRequests.Handlers.Commands;
using Application.Operation.Features.Employee.QuestionBankRequests.Commands;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;
using Tawtheef.Domain.Entities.QuestionsBank;

namespace QuestionBankAssignments.Tests;

internal sealed class WorkflowReadTestContext(DbContextOptions options) : AssignmentTestContext(options)
{
    protected override void AddWorkflowNavigations(Dictionary<Type, string[]> navigations)
    {
        navigations[typeof(QuestionBankAssignment)] = ["QuestionBankRequest", "RequestItems", "Status", "Employee"];
        navigations[typeof(QuestionBankRequest)] = ["Items", "Assignments", "History", "QuestionBank", "Status", "RequestType", "BaseVersion"];
        navigations[typeof(QuestionRevision)] = ["Question", "Options", "SourceRequestItem", "Resource", "QuestionType", "DifficultyLevel"];
        navigations[typeof(QuestionBankRequestItem)] = ["Request", "QuestionBankAssignment", "Question", "OriginalRevision", "CurrentProposedRevision", "Reviews", "Status"];
        navigations[typeof(QuestionBankRequestItemReview)] = ["RequestItem"];
        navigations[typeof(QuestionBank)] = ["QuestionBankType", "Management", "JobTitle", "Versions"];
        navigations[typeof(QuestionBankVersion)] = ["QuestionBank", "Questions"];
        navigations[typeof(QuestionBankVersionQuestion)] = ["Question", "QuestionRevision", "QuestionBankVersion"];
        navigations[typeof(QuestionBankRequestReview)] = ["Request"];
        foreach (var type in new[] { typeof(QuestionBankType), typeof(QuestionBankAssignmentStatus),
                     typeof(QuestionBankRequestStatus), typeof(QuestionBankRequestType), typeof(QuestionBankRequestItemStatus),
                     typeof(QuestionType), typeof(DifficultyLevel), typeof(Management), typeof(JobTitle) })
            navigations[type] = [];
    }

    public static void Seed(AssignmentTestContext db, QuestionBankRequest request)
    {
        SeedLookups(db, typeof(QuestionBankRequestStatus), typeof(QuestionBankRequestStatusIds));
        SeedLookups(db, typeof(QuestionBankRequestType), typeof(QuestionBankRequestTypeIds));
        SeedLookups(db, typeof(QuestionBankRequestItemStatus), typeof(QuestionBankRequestItemStatusIds));
        SeedLookups(db, typeof(QuestionType), typeof(QuestionTypeIds));
        SeedLookups(db, typeof(DifficultyLevel), typeof(DifficultyLevelIds));
        SeedLookups(db, typeof(QuestionBankType), typeof(QuestionBankTypeIds));
        foreach (var field in typeof(QuestionBankAssignmentStatusIds).GetFields())
            db.Add(new QuestionBankAssignmentStatus { Id = (Guid)field.GetValue(null)!, Code = field.Name,
                NameAr = field.Name, NameEn = field.Name });
        request.QuestionBank = new QuestionBank { Id = Guid.NewGuid(), QuestionBankTypeId = QuestionBankTypeIds.Specialized };
        request.QuestionBankId = request.QuestionBank.Id;
        request.BaseVersion = new QuestionBankVersion { Id = request.BaseVersionId!.Value,
            QuestionBank = request.QuestionBank, QuestionBankId = request.QuestionBankId, VersionNo = 1 };
    }

    private static void SeedLookups(DbContext db, Type entityType, Type idsType)
    {
        foreach (var field in idsType.GetFields().Where(f => f.FieldType == typeof(Guid)))
        {
            var lookup = (LookupBase)Activator.CreateInstance(entityType)!;
            lookup.Id = (Guid)field.GetValue(null)!;
            lookup.NameAr = lookup.NameEn = lookup.BackendName = field.Name;
            db.Add(lookup);
        }
    }
}

public class WorkflowReadTests
{
    [TestCase(false)]
    [TestCase(true)]
    public async Task ApprovedPayloadIsSavedAndIssuedOrSingleOverrideReturnsOnlyThatQuestionForCorrection(bool overrideOne)
    {
        await using var fixture = await AssignmentFixture.Create(workflowReads: true);
        fixture.EnableReviewSubmission();
        var first = await fixture.AddChange(QuestionChangeTypeIds.ADD, QuestionBankRequestItemStatusIds.PENDING_REVIEW);
        var second = await fixture.AddChange(QuestionChangeTypeIds.ADD, QuestionBankRequestItemStatusIds.PENDING_REVIEW);
        var request = await fixture.Db.Set<QuestionBankRequest>().Include(r => r.QuestionBank).SingleAsync();
        var submittedAssignment = await fixture.Db.Set<QuestionBankAssignment>().SingleAsync();
        submittedAssignment.StatusId = QuestionBankAssignmentStatusIds.QuestionEntryCompleted;
        request.StatusId = QuestionBankRequestStatusIds.PendingReview;
        request.QuestionBank.CurrentApprovedVersionId = request.BaseVersionId;
        await fixture.Db.SaveChangesAsync();
        QuestionReviewInput[] inputs =
        [
            new(first.Id, first.CurrentProposedRevisionId!.Value,
                overrideOne ? QuestionReviewDecisionIds.NEEDS_MODIFICATION : QuestionReviewDecisionIds.APPROVED,
                overrideOne ? "Change this question" : null),
            new(second.Id, second.CurrentProposedRevisionId!.Value, QuestionReviewDecisionIds.APPROVED, null)
        ];
        var result = await new SubmitQuestionBankRequestReviewCommandHandler(fixture.Uow.Object, fixture.User.Object)
            .Handle(new(request.Id, inputs), default);
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value.Issued, Is.EqualTo(!overrideOne));
        var saved = await fixture.Db.Set<QuestionBankRequestItemReview>().AsNoTracking().ToListAsync();
        Assert.That(saved.Count, Is.EqualTo(2));
        foreach (var input in inputs)
            Assert.That(saved.Single(r => r.RequestItemId == input.RequestItemId).DecisionId, Is.EqualTo(input.DecisionId));
        var assignment = await fixture.Db.Set<QuestionBankAssignment>().AsNoTracking().SingleAsync();
        Assert.That(assignment.StatusId, Is.EqualTo(overrideOne
            ? QuestionBankAssignmentStatusIds.ReturnedForModification : QuestionBankAssignmentStatusIds.Completed));
        var access = await AssignmentAccess.ValidateAsync(fixture.Uow.Object, assignment.Id, assignment.EmployeeId, default);
        Assert.That(access.IsSuccess, Is.EqualTo(overrideOne));
    }

    [Test]
    public async Task ActiveEmployeeCanReadTheWorkspaceButFinalizedListExposesOnlyMetadata()
    {
        await using var fixture = await AssignmentFixture.Create(workflowReads: true);
        await fixture.AddChange(QuestionChangeTypeIds.ADD);
        var workspaceHandler = new GetQuestionBankAssignmentWorkspaceQueryHandler(fixture.Uow.Object, fixture.User.Object);
        var workspace = await workspaceHandler.Handle(new(fixture.Assignment.Id), default);
        Assert.That(workspace.IsSuccess, Is.True);
        Assert.That(workspace.Value.Questions.Single().QuestionTextEn, Is.EqualTo("Question"));
        var listHandler = new ListMyQuestionBankAssignmentsQueryHandler(fixture.Uow.Object, fixture.User.Object);
        var active = await listHandler.Handle(new(), default);
        Assert.That(active.Value.Items.Single().CanAccess, Is.True);

        var assignment = await fixture.Db.Set<QuestionBankAssignment>().Include(a => a.QuestionBankRequest).SingleAsync();
        assignment.StatusId = QuestionBankAssignmentStatusIds.Completed;
        assignment.QuestionBankRequest.StatusId = QuestionBankRequestStatusIds.Issued;
        await fixture.Db.SaveChangesAsync();
        var final = await listHandler.Handle(new(), default);
        Assert.That(final.Value.Items.Single().CanAccess, Is.False);
        var serialized = System.Text.Json.JsonSerializer.Serialize(final.Value);
        Assert.That(serialized, Does.Not.Contain("QuestionText").And.Not.Contain("Options").And.Not.Contain("Explanation"));
        var denied = await workspaceHandler.Handle(new(fixture.Assignment.Id), default);
        Assert.That(denied.IsFailed, Is.True);
        Assert.That(denied.Errors.Single().Metadata["StatusCode"], Is.EqualTo(403));
    }

    [Test]
    public async Task ReviewerApiReturnsLatestSavedDecisionAndNoteForCorrectionRounds()
    {
        await using var fixture = await AssignmentFixture.Create(workflowReads: true);
        var item = await fixture.AddChange(QuestionChangeTypeIds.ADD, QuestionBankRequestItemStatusIds.PENDING_REVIEW);
        var undecided = await fixture.AddChange(QuestionChangeTypeIds.ADD, QuestionBankRequestItemStatusIds.PENDING_REVIEW);
        var request = await fixture.Db.Set<QuestionBankRequest>().SingleAsync();
        request.StatusId = QuestionBankRequestStatusIds.PendingReview;
        request.CurrentReviewRound = 2;
        fixture.Db.AddRange(
            new QuestionBankRequestItemReview { Id = Guid.NewGuid(), RequestItemId = item.Id, ReviewRound = 1,
                DecisionId = QuestionReviewDecisionIds.REJECTED, ReviewNote = "Old rejection" },
            new QuestionBankRequestItemReview { Id = Guid.NewGuid(), RequestItemId = item.Id, ReviewRound = 2,
                DecisionId = QuestionReviewDecisionIds.NEEDS_MODIFICATION, ReviewNote = "Current correction" },
            new QuestionBankRequestItemReview { Id = Guid.NewGuid(), RequestItemId = item.Id, ReviewRound = 3,
                DecisionId = QuestionReviewDecisionIds.APPROVED, ReviewNote = "Deleted review", IsDeleted = true });
        await fixture.Db.SaveChangesAsync();
        var result = await new GetQuestionBankRequestReviewQueryHandler(fixture.Uow.Object, fixture.User.Object)
            .Handle(new(request.Id), default);
        Assert.That(result.IsSuccess, Is.True);
        var saved = result.Value.Items.Single(i => i.RequestItemId == item.Id);
        Assert.That(saved.DecisionId, Is.EqualTo(QuestionReviewDecisionIds.NEEDS_MODIFICATION));
        Assert.That(saved.ReviewNote, Is.EqualTo("Current correction"));
        Assert.That(result.Value.Items.Single(i => i.RequestItemId == undecided.Id).DecisionId, Is.Null);
    }
}
