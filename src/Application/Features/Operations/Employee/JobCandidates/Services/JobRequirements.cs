namespace Tawtheef.Application.Features.Operations.Employee.JobCandidates.Services;

internal sealed record JobRequirements(
    Guid? JobMajorId,
    Guid? JobSubMajorId,
    IReadOnlyCollection<Guid> RequiredSkillIds);
