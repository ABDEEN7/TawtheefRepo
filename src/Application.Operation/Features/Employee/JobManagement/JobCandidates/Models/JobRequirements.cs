namespace Application.Operation.Features.Employee.JobManagement.JobCandidates.Models;

public sealed record JobRequirements(
    Guid? JobMajorId,
    Guid? JobSubMajorId,
    IReadOnlyCollection<JobSpecialization> JobSpecializations,
    IReadOnlyCollection<Guid> QualificationLevelIds,
    IReadOnlyCollection<Guid> RequiredSkillIds);

public sealed record JobSpecialization(Guid MajorId, Guid SubMajorId);
