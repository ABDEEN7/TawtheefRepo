using Microsoft.EntityFrameworkCore;

namespace Tawtheef.Domain.Entities.Recruitment.JobDetails;

[Owned]
public class JobBasics 
{
    public string Title { get; set; } = string.Empty;
    public Guid RequestingDepartmentId { get; set; }
    public Guid JobCategoryId { get; set; }
    public Guid GenderId { get; set; }
    public Guid TargetEntityId { get; set; }
    public Guid MajorId { get; set; }
    public Guid WorkTypeId { get; set; }
    public int Vacancies { get; set; }
    public DateTimeOffset Deadline { get; set; }
}
