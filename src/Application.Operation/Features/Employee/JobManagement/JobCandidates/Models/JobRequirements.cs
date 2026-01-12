namespace Application.Operation.Features.Employee.JobCandidates.Models;

public sealed record JobRequirements(
    Guid? JobMajorId,
    Guid? JobSubMajorId,
    IReadOnlyCollection<Guid> RequiredSkillIds);
