namespace Application.Operation.Features.Employee.JobManagement.Job.DTOs;

public class JobSpecializationRequestDto
{
    public Guid MajorId { get; set; }
    public Guid? SubMajorId { get; set; }
}
