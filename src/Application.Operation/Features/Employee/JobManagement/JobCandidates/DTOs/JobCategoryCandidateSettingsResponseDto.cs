namespace Application.Operation.Features.Employee.JobManagement.JobCandidates.DTOs;

public sealed class JobCategoryCandidateSettingsResponseDto
{
    public Guid Id { get; init; }
    public int AcademicJobVacancies { get; init; }
    public int LaborJobVacancies { get; init; }
    public int AdministrativeJobVacancies { get; init; }
}
