using System.Collections;
using Application.Operation.Features.Employee.QuestionBankAssignments.Commands;
using Application.Operation.Features.Employee.QuestionBankAssignments.Handlers;
using Application.Operation.Features.Employee.QuestionBankAssignments.Services;
using FluentResults;
using MediatR;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using Tawtheef.Application.Common.Interfaces;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.QuestionsBank;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Infrastructure.Configurations.Entities.QuestionsBank;

namespace QuestionBankAssignments.Tests;

// A relational slice using the production revision/item configurations. Unrelated
// user, bank, and lookup relationships are excluded only from this test model.
internal sealed class AssignmentTestContext(DbContextOptions<AssignmentTestContext> options)
    : DbContext(options), ITawtheefDbContext
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new QuestionRevisionConfiguration());
        builder.ApplyConfiguration(new QuestionBankRequestItemConfiguration());
        builder.Entity<EmployeeUser>().HasBaseType((Type?)null);
        var navigations = new Dictionary<Type, string[]>
        {
            [typeof(Question)] = ["Revisions", "RequestItems", "BankVersionQuestions"],
            [typeof(QuestionRevision)] = ["Question", "Options", "SourceRequestItem"],
            [typeof(QuestionRevisionOption)] = ["QuestionRevision"],
            [typeof(QuestionBankRequestItem)] = ["Request", "QuestionBankAssignment", "Question", "OriginalRevision", "CurrentProposedRevision"],
            [typeof(QuestionBankAssignment)] = ["QuestionBankRequest", "RequestItems"],
            [typeof(QuestionBankRequest)] = ["Items", "Assignments", "History"],
            [typeof(QuestionBankVersionQuestion)] = ["Question", "QuestionRevision"],
            [typeof(QuestionBankRequestHistory)] = ["Request"],
            [typeof(EmployeeUser)] = ["EmployeeProfile"],
            [typeof(EmployeeProfile)] = []
        };
        foreach (var (type, allowed) in navigations)
        {
            var entity = builder.Entity(type);
            foreach (var property in type.GetProperties())
            {
                var propertyType = property.PropertyType;
                var navigation = propertyType.Namespace?.StartsWith("Tawtheef.Domain", StringComparison.Ordinal) == true ||
                    (propertyType.IsGenericType && typeof(IEnumerable).IsAssignableFrom(propertyType));
                if (navigation && !allowed.Contains(property.Name)) entity.Ignore(property.Name);
            }
        }
        foreach (var entity in builder.Model.GetEntityTypes().ToList())
            if (!navigations.ContainsKey(entity.ClrType)) builder.Ignore(entity.ClrType);
        builder.Entity<QuestionBankAssignment>().HasOne(x => x.QuestionBankRequest)
            .WithMany(x => x.Assignments).HasForeignKey(x => x.QuestionBankRequestId);
    }
}

internal sealed class AssignmentFixture : IAsyncDisposable
{
    private readonly SqliteConnection connection = new("Data Source=:memory:");
    public AssignmentTestContext Db { get; private set; } = null!;
    public Mock<IUnitOfWork> Uow { get; } = new();
    public Mock<ICurrentUserService> User { get; } = new();
    public Mock<IRichTextSanitizer> Sanitizer { get; } = new();
    public QuestionBankAssignment Assignment { get; private set; } = null!;
    public Guid BaseQuestionId { get; private set; }
    public Guid BaseRevisionId { get; private set; }
    public int Saves { get; private set; }
    public int? FailOnSave { get; set; }
    public Func<int, CancellationToken, Task>? BeforeSave { get; set; }
    public Func<int, CancellationToken, Task>? AfterSave { get; set; }

    public static readonly QuestionInput Input = new(QuestionTypeIds.MULTIPLE_CHOICE,
        DifficultyLevelIds.EASY, "سؤال", "Question", null, null, null,
        [new("أول", "First", true, 1), new("ثاني", "Second", false, 2)]);

    public static async Task<AssignmentFixture> Create()
    {
        var fixture = new AssignmentFixture();
        await fixture.connection.OpenAsync();
        fixture.Db = new(new DbContextOptionsBuilder<AssignmentTestContext>()
            .UseSqlite(fixture.connection).Options);
        await fixture.Db.Database.EnsureCreatedAsync();
        var employee = new EmployeeUser
        {
            Id = Guid.NewGuid(), FullNameAr = "موظف", FullNameEn = "Employee",
            EmployeeProfile = new EmployeeProfile { Id = Guid.NewGuid() }
        };
        var request = new QuestionBankRequest
        {
            Id = Guid.NewGuid(), BaseVersionId = Guid.NewGuid(),
            RequestTypeId = QuestionBankRequestTypeIds.MAINTENANCE,
            StatusId = QuestionBankRequestStatusIds.QuestionEntryInProgress
        };
        fixture.Assignment = new()
        {
            Id = Guid.NewGuid(), EmployeeId = employee.Id, QuestionBankRequest = request,
            QuestionBankRequestId = request.Id, StatusId = QuestionBankAssignmentStatusIds.Assigned,
            MinimumQuestionCount = 5
        };
        fixture.Db.AddRange(employee, fixture.Assignment);
        var baseQuestion = await fixture.CreateBaseQuestion();
        fixture.BaseQuestionId = baseQuestion.QuestionId;
        fixture.BaseRevisionId = baseQuestion.QuestionRevisionId;
        await fixture.Db.SaveChangesAsync();
        fixture.Db.ChangeTracker.Clear();
        fixture.User.SetupGet(x => x.UserId).Returns(employee.Id.ToString());
        fixture.Sanitizer.Setup(x => x.Sanitize(It.IsAny<string?>())).Returns((string? text) => text);
        fixture.Sanitizer.Setup(x => x.HasMeaningfulContent(It.IsAny<string?>()))
            .Returns((string? text) => !string.IsNullOrWhiteSpace(text));
        fixture.Uow.SetupGet(x => x.Context).Returns(fixture.Db);
        fixture.Repository<QuestionBankAssignment>();
        fixture.Repository<QuestionBankRequestItem>();
        fixture.Repository<QuestionRevision>();
        fixture.Repository<QuestionBankVersionQuestion>();
        fixture.Repository<QuestionBankRequestHistory>();
        fixture.Uow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(async (CancellationToken ct) =>
            {
                fixture.Saves++;
                if (fixture.BeforeSave is not null) await fixture.BeforeSave(fixture.Saves, ct);
                if (fixture.FailOnSave == fixture.Saves) throw new InvalidOperationException("Simulated persistence failure");
                var saved = await fixture.Db.SaveChangesAsync(ct);
                if (fixture.AfterSave is not null) await fixture.AfterSave(fixture.Saves, ct);
                return saved;
            });
        fixture.Transaction<IResult<Guid>>();
        fixture.Transaction<IResult<Unit>>();
        return fixture;
    }

    private void Repository<T>() where T : EventEntity
    {
        var repository = new Mock<IGenericRepository<T>>();
        repository.SetupGet(x => x.DbSet).Returns(Db.Set<T>());
        repository.Setup(x => x.AddAsync(It.IsAny<T>(), It.IsAny<CancellationToken>()))
            .Returns(async (T entity, CancellationToken ct) =>
            {
                await Db.AddAsync(entity, ct);
                return Result.Ok(entity);
            });
        Uow.Setup(x => x.GetEntityRepository<T>()).Returns(repository.Object);
    }

    private void Transaction<TResult>() where TResult : IResultBase
    {
        Uow.Setup(x => x.ExecuteInTransactionAsync(It.IsAny<Func<CancellationToken, Task<TResult>>>(), It.IsAny<CancellationToken>()))
            .Returns(async (Func<CancellationToken, Task<TResult>> operation, CancellationToken ct) =>
            {
                await using var transaction = await Db.Database.BeginTransactionAsync(ct);
                try
                {
                    var result = await operation(ct);
                    if (result.IsFailed) await transaction.RollbackAsync(ct);
                    else await transaction.CommitAsync(ct);
                    return result;
                }
                catch
                {
                    await transaction.RollbackAsync(ct);
                    throw;
                }
            });
    }

    private async Task<QuestionBankVersionQuestion> CreateBaseQuestion()
    {
        var question = new Question { Id = Guid.NewGuid() };
        var revision = QuestionEntryRules.Revision(question.Id, 1, null, Input);
        var versionQuestion = new QuestionBankVersionQuestion
        {
            Id = Guid.NewGuid(), QuestionBankVersionId = Assignment.QuestionBankRequest.BaseVersionId!.Value,
            Question = question, QuestionId = question.Id, QuestionRevision = revision, QuestionRevisionId = revision.Id
        };
        await Db.AddAsync(versionQuestion);
        return versionQuestion;
    }

    public async Task<QuestionBankRequestItem> AddChange(Guid changeType, Guid? status = null)
    {
        var baseQuestion = await CreateBaseQuestion();
        var proposed = changeType == QuestionChangeTypeIds.DELETE ? null :
            QuestionEntryRules.Revision(baseQuestion.QuestionId, 2, null, Input);
        if (proposed is not null) Db.Add(proposed);
        var item = new QuestionBankRequestItem
        {
            Id = Guid.NewGuid(), RequestId = Assignment.QuestionBankRequestId,
            QuestionBankAssignmentId = Assignment.Id, QuestionId = baseQuestion.QuestionId,
            ChangeTypeId = changeType, StatusId = status ?? QuestionBankRequestItemStatusIds.DRAFT,
            OriginalRevisionId = changeType == QuestionChangeTypeIds.ADD ? null : baseQuestion.QuestionRevisionId,
            CurrentProposedRevisionId = proposed?.Id
        };
        Db.Add(item);
        await Db.SaveChangesAsync();
        return item;
    }

    public async ValueTask DisposeAsync()
    {
        await Db.DisposeAsync();
        await connection.DisposeAsync();
    }
}
