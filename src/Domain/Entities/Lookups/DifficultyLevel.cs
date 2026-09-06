using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.QuestionsBank;

namespace Tawtheef.Domain.Entities.Lookups;

public static class DifficultyLevelIds
{
    public static Guid EASY = Guid.Parse("59e7808a-916b-44c4-92a9-41dd1c6e24e4");
    public static Guid MEDIUM = Guid.Parse("580deb45-9304-4fcb-905e-868f5db3848b");
    public static Guid HARD = Guid.Parse("9bbee09c-e2a3-463b-9901-4b646200074d");
}

[Table(nameof(DifficultyLevel), Schema = Schemas.Lookup)]
public class DifficultyLevel : LookupBase
{
    public ICollection<QuestionRevision> QuestionRevisions { get; set; } = new List<QuestionRevision>();
}
