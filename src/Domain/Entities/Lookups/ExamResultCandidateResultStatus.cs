using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Lookups;

public static class ExamResultCandidateResultStatusIds
{
    public static readonly Guid Passed = Guid.Parse("76055057-d4b7-4791-a7df-8ab5c80e116d");
    public static readonly Guid Failed = Guid.Parse("24bd3d0e-7980-494c-a84d-167a81419030");
    public static readonly Guid NoShow = Guid.Parse("385ff451-2293-4023-a33d-235f8d5fdb3f");
    public static readonly Guid NotCompleted = Guid.Parse("fb22f73d-0ab6-4031-a4bf-a219bbb07cd5");
}

[Table(nameof(ExamResultCandidateResultStatus), Schema = Schemas.Lookup)]
public class ExamResultCandidateResultStatus : LookupBase;

