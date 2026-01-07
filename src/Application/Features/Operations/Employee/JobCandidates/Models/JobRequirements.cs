namespace Tawtheef.Application.Features.Operations.Employee.JobCandidates.Models;

public sealed record JobRequirements(
    Guid? JobMajorId,
    Guid? JobSubMajorId,
    IReadOnlyCollection<Guid> RequiredSkillIds);
