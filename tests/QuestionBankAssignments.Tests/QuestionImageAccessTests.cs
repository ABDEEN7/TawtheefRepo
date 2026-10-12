using System.Security.Claims;
using Application.Operation.Features.Employee.QuestionBankAssignments.Handlers;
using Application.Operation.Features.Employee.QuestionBankAssignments.Queries;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Tawtheef.Application.Common.Security;
using Tawtheef.Domain.Entities;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.QuestionsBank;

namespace QuestionBankAssignments.Tests;

public class QuestionImageAccessTests
{
    [TestCase("private/operation/question-bank/employee/images/a.png")]
    [TestCase("private//operation/question-bank/employee/images/a.png")]
    [TestCase("private/operation/other/../question-bank/employee/images/a.png")]
    [TestCase("private\\operation\\question-bank\\employee\\images\\a.png")]
    public void AlternateQuestionImagePathsUseTheSameAuthorizationAndNoStoreClassification(string key)
    {
        var image = new CheckQuestionBankImageAccessQuery(key);
        Assert.That(image.IsQuestionBankImage, Is.True);
        Assert.That(image.NormalizedBlobKey, Is.EqualTo("private/operation/question-bank/employee/images/a.png"));
    }

    private static IHttpContextAccessor Context(params string[] permissions) => new HttpContextAccessor
    {
        HttpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(permissions.Select(p => new Claim("permission", p)), "test"))
        }
    };

    [Test]
    public async Task DirectImageAccessIsAllowedWhileActiveAndRejectedAfterFinalApprovalEvenWithAlternatePaths()
    {
        await using var fixture = await AssignmentFixture.Create();
        var key = $"private/operation/question-bank/{fixture.Assignment.EmployeeId}/images/question.png";
        var image = new Resource { Id = Guid.NewGuid(), Key = Guid.NewGuid().ToString(), Name = "question.png", Url = key, Type = "image/png", Size = 100 };
        fixture.Db.Add(image);
        var item = await fixture.AddChange(QuestionChangeTypeIds.ADD);
        var revision = await fixture.Db.Set<QuestionRevision>().SingleAsync(r => r.Id == item.CurrentProposedRevisionId);
        revision.ResourceId = image.Id;
        await fixture.Db.SaveChangesAsync();
        var handler = new CheckQuestionBankImageAccessQueryHandler(fixture.Uow.Object, fixture.User.Object,
            Context(PermissionKeys.QuestionBankAssignments.Manage));
        Assert.That((await handler.Handle(new(key), default)).IsSuccess, Is.True);

        var assignment = await fixture.Db.Set<QuestionBankAssignment>().Include(a => a.QuestionBankRequest).SingleAsync();
        assignment.StatusId = QuestionBankAssignmentStatusIds.Completed;
        assignment.QuestionBankRequest.StatusId = QuestionBankRequestStatusIds.Issued;
        await fixture.Db.SaveChangesAsync();
        foreach (var path in new[] { key, key.Replace("/operation/", "//operation/"), key.Replace("/question-bank/", "/other/../question-bank/") })
        {
            var result = await handler.Handle(new(path), default);
            Assert.That(result.IsFailed, Is.True);
            Assert.That(result.Errors.Single().Metadata["StatusCode"], Is.EqualTo(403));
        }
    }

    [TestCase(PermissionKeys.QuestionBankRequests.Review)]
    [TestCase(PermissionKeys.Exams.ViewAttemptDetails)]
    public async Task AdministrativeImageAccessRemainsAvailable(string permission)
    {
        await using var fixture = await AssignmentFixture.Create();
        var assignment = await fixture.Db.Set<QuestionBankAssignment>().Include(a => a.QuestionBankRequest).SingleAsync();
        assignment.StatusId = QuestionBankAssignmentStatusIds.Completed;
        assignment.QuestionBankRequest.StatusId = QuestionBankRequestStatusIds.Issued;
        await fixture.Db.SaveChangesAsync();
        var handler = new CheckQuestionBankImageAccessQueryHandler(fixture.Uow.Object, fixture.User.Object, Context(permission));
        var result = await handler.Handle(new("private/operation/question-bank/employee/images/question.png"), default);
        Assert.That(result.IsSuccess, Is.True);
    }

    [TestCase(PermissionKeys.Exams.View)]
    [TestCase(PermissionKeys.QuestionBanks.View)]
    public async Task CatalogPermissionDoesNotBypassFinalEmployeeImageAccess(string catalogPermission)
    {
        await using var fixture = await AssignmentFixture.Create();
        var item = await fixture.AddChange(QuestionChangeTypeIds.ADD);
        var key = $"private/operation/question-bank/{fixture.Assignment.EmployeeId}/images/final.png";
        var image = new Resource
        {
            Id = Guid.NewGuid(), Key = Guid.NewGuid().ToString(), Name = "final.png",
            Url = key, Type = "image/png", Size = 100
        };
        fixture.Db.Add(image);
        var revision = await fixture.Db.Set<QuestionRevision>().SingleAsync(r => r.Id == item.CurrentProposedRevisionId);
        revision.ResourceId = image.Id;
        var assignment = await fixture.Db.Set<QuestionBankAssignment>().Include(a => a.QuestionBankRequest).SingleAsync();
        assignment.StatusId = QuestionBankAssignmentStatusIds.Completed;
        assignment.QuestionBankRequest.StatusId = QuestionBankRequestStatusIds.Issued;
        await fixture.Db.SaveChangesAsync();
        var handler = new CheckQuestionBankImageAccessQueryHandler(fixture.Uow.Object, fixture.User.Object,
            Context(PermissionKeys.QuestionBankAssignments.Manage, catalogPermission));
        var result = await handler.Handle(new(key), default);
        Assert.That(result.IsFailed, Is.True);
        Assert.That(result.Errors.Single().Metadata["StatusCode"], Is.EqualTo(403));
    }

    [Test]
    public async Task SoftDeletedRevisionCannotTurnAFinalizedImageIntoAnUnattachedUploaderPreview()
    {
        await using var fixture = await AssignmentFixture.Create();
        var item = await fixture.AddChange(QuestionChangeTypeIds.ADD);
        var key = $"private/operation/question-bank/{fixture.Assignment.EmployeeId}/images/deleted.png";
        var image = new Resource
        {
            Id = Guid.NewGuid(), Key = Guid.NewGuid().ToString(), Name = "deleted.png",
            Url = key, Type = "image/png", Size = 100
        };
        fixture.Db.Add(image);
        var revision = await fixture.Db.Set<QuestionRevision>().SingleAsync(r => r.Id == item.CurrentProposedRevisionId);
        revision.ResourceId = image.Id;
        revision.IsDeleted = true;
        var assignment = await fixture.Db.Set<QuestionBankAssignment>().Include(a => a.QuestionBankRequest).SingleAsync();
        assignment.StatusId = QuestionBankAssignmentStatusIds.Completed;
        assignment.QuestionBankRequest.StatusId = QuestionBankRequestStatusIds.Issued;
        await fixture.Db.SaveChangesAsync();
        var handler = new CheckQuestionBankImageAccessQueryHandler(fixture.Uow.Object, fixture.User.Object,
            Context(PermissionKeys.QuestionBankAssignments.Manage));
        Assert.That((await handler.Handle(new(key), default)).IsFailed, Is.True);
    }

    [Test]
    public async Task UnauthenticatedClaimsDoNotAuthorizeQuestionImages()
    {
        await using var fixture = await AssignmentFixture.Create();
        var context = Context(PermissionKeys.QuestionBankRequests.Review);
        context.HttpContext!.User = new ClaimsPrincipal(new ClaimsIdentity(
            [new Claim("permission", PermissionKeys.QuestionBankRequests.Review)]));
        var handler = new CheckQuestionBankImageAccessQueryHandler(fixture.Uow.Object, fixture.User.Object, context);
        Assert.That((await handler.Handle(new("private/operation/question-bank/employee/images/question.png"), default)).IsFailed, Is.True);
    }

    [Test]
    public async Task RetainedFinalImageIdCannotBeReattachedThroughAnyEmployeeQuestionWriteCommand()
    {
        await using var fixture = await AssignmentFixture.Create();
        var sourceItem = await fixture.AddChange(QuestionChangeTypeIds.ADD);
        var image = new Resource
        {
            Id = Guid.NewGuid(), Key = Guid.NewGuid().ToString(), Name = "final.png", Type = "image/png", Size = 100,
            Url = $"private/operation/question-bank/{fixture.Assignment.EmployeeId}/images/final.png"
        };
        fixture.Db.Add(image);
        var sourceRevision = await fixture.Db.Set<QuestionRevision>().SingleAsync(r => r.Id == sourceItem.CurrentProposedRevisionId);
        sourceRevision.ResourceId = image.Id;
        var sourceAssignment = await fixture.Db.Set<QuestionBankAssignment>().Include(a => a.QuestionBankRequest).SingleAsync();
        sourceAssignment.StatusId = QuestionBankAssignmentStatusIds.Completed;
        sourceAssignment.QuestionBankRequest.StatusId = QuestionBankRequestStatusIds.Issued;
        var activeRequest = new QuestionBankRequest
        {
            Id = Guid.NewGuid(), RequestTypeId = QuestionBankRequestTypeIds.MAINTENANCE,
            StatusId = QuestionBankRequestStatusIds.QuestionEntryInProgress,
            BaseVersionId = sourceAssignment.QuestionBankRequest.BaseVersionId
        };
        var activeAssignment = new QuestionBankAssignment
        {
            Id = Guid.NewGuid(), EmployeeId = sourceAssignment.EmployeeId,
            QuestionBankRequest = activeRequest, QuestionBankRequestId = activeRequest.Id,
            StatusId = QuestionBankAssignmentStatusIds.Assigned
        };
        fixture.Db.Add(activeAssignment);
        var targetItem = await fixture.AddChange(QuestionChangeTypeIds.ADD);
        targetItem.QuestionBankAssignmentId = activeAssignment.Id;
        targetItem.RequestId = activeRequest.Id;
        await fixture.Db.SaveChangesAsync();
        fixture.Db.ChangeTracker.Clear();

        var input = AssignmentFixture.Input with { ResourceId = image.Id };
        IResultBase[] results =
        [
            await new AddQuestionToAssignmentCommandHandler(fixture.Uow.Object, fixture.User.Object, fixture.Sanitizer.Object)
                .Handle(new(activeAssignment.Id, input), default),
            await new EditAssignmentQuestionCommandHandler(fixture.Uow.Object, fixture.User.Object, fixture.Sanitizer.Object)
                .Handle(new(activeAssignment.Id, targetItem.Id, input), default),
            await new UpdateBaseVersionQuestionCommandHandler(fixture.Uow.Object, fixture.User.Object, fixture.Sanitizer.Object)
                .Handle(new(activeAssignment.Id, fixture.BaseQuestionId, input), default)
        ];
        foreach (var result in results)
        {
            Assert.That(result.IsFailed, Is.True);
            Assert.That(result.Errors.Single().Metadata["StatusCode"], Is.EqualTo(403));
        }
        Assert.That(fixture.Saves, Is.Zero);
    }

    [Test]
    public async Task EmployeeCanAttachAnOwnNewUploadAndRetainItWhenEditingAnActiveQuestion()
    {
        await using var fixture = await AssignmentFixture.Create();
        var image = new Resource
        {
            Id = Guid.NewGuid(), Key = Guid.NewGuid().ToString(), Name = "new.png", Type = "image/png", Size = 100,
            Url = $"private/operation/question-bank/{fixture.Assignment.EmployeeId}/images/new.png"
        };
        fixture.Db.Add(image);
        await fixture.Db.SaveChangesAsync();
        var input = AssignmentFixture.Input with { ResourceId = image.Id };
        var added = await new AddQuestionToAssignmentCommandHandler(fixture.Uow.Object, fixture.User.Object, fixture.Sanitizer.Object)
            .Handle(new(fixture.Assignment.Id, input), default);
        Assert.That(added.IsSuccess, Is.True);
        var edited = await new EditAssignmentQuestionCommandHandler(fixture.Uow.Object, fixture.User.Object, fixture.Sanitizer.Object)
            .Handle(new(fixture.Assignment.Id, added.Value, input), default);
        Assert.That(edited.IsSuccess, Is.True);
    }

    [Test]
    public async Task EmployeeCanSeeMaintenanceBaseImagesOnlyWhileTheirAssignmentIsAccessible()
    {
        await using var fixture = await AssignmentFixture.Create();
        var key = "private/operation/question-bank/another-uploader/images/base.png";
        var image = new Resource { Id = Guid.NewGuid(), Key = Guid.NewGuid().ToString(), Name = "base.png", Url = key, Type = "image/png", Size = 100 };
        fixture.Db.Add(image);
        var revision = await fixture.Db.Set<QuestionRevision>().SingleAsync(r => r.Id == fixture.BaseRevisionId);
        revision.ResourceId = image.Id;
        await fixture.Db.SaveChangesAsync();
        var handler = new CheckQuestionBankImageAccessQueryHandler(fixture.Uow.Object, fixture.User.Object,
            Context(PermissionKeys.QuestionBankAssignments.Manage));
        Assert.That((await handler.Handle(new(key), default)).IsSuccess, Is.True);
        var assignment = await fixture.Db.Set<QuestionBankAssignment>().SingleAsync();
        assignment.StatusId = QuestionBankAssignmentStatusIds.Completed;
        await fixture.Db.SaveChangesAsync();
        Assert.That((await handler.Handle(new(key), default)).IsFailed, Is.True);
    }

    [Test]
    public async Task UnrelatedResourcesRemainUnchangedAndUnattachedUploadsAreLimitedToTheirUploader()
    {
        await using var fixture = await AssignmentFixture.Create();
        var handler = new CheckQuestionBankImageAccessQueryHandler(fixture.Uow.Object, fixture.User.Object,
            Context(PermissionKeys.QuestionBankAssignments.Manage));
        Assert.That((await handler.Handle(new("private/recruitment/profile/document.pdf"), default)).IsSuccess, Is.True);
        var ownUpload = $"private/operation/question-bank/{fixture.Assignment.EmployeeId}/images/new.png";
        Assert.That((await handler.Handle(new(ownUpload), default)).IsSuccess, Is.True);
        Assert.That((await handler.Handle(new(ownUpload.Replace(fixture.Assignment.EmployeeId.ToString(), Guid.NewGuid().ToString())), default)).IsFailed, Is.True);
    }
}
