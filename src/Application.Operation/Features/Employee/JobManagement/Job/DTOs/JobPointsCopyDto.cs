namespace Application.Operation.Features.Employee.Job.DTOs;

public class JobPointsCopyDto
{
    public int ApplicantCategory { get; set; }
    public int Education { get; set; }
    public int Experience { get; set; }
    public int Training { get; set; }
    public int Skills { get; set; }
    public int Languages { get; set; }
    public int Certificates { get; set; }
    public int Total { get; set; }
    public List<JobPointsDetailCopyDto> Details { get; set; } = [];
}
