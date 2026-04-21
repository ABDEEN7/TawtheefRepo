namespace Application.Operation.Features.Employee.JobManagement.JobOperations.DTOs;

public class UpdateJobBasicsDto
{
    public Guid JobTitleId { get; set; }
    public Guid SectorId { get; set; }
    public Guid ManagementId { get; set; }
    public Guid? DepartmentId { get; set; }
    public int YearsOfExperience { get; set; }
    public Guid JobCategoryId { get; set; }
    public Guid WorkLocationId { get; set; }
    public Guid? GenderId { get; set; }
    public Guid WorkTypeId { get; set; }
    public int NumberOfVacancies { get; set; }
    public DateTimeOffset ClosingDate { get; set; }
    public int MinimumAge { get; set; }
    public int MaximumAge { get; set; }
}
