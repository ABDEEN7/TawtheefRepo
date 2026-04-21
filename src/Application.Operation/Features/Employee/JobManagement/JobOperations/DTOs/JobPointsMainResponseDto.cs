namespace Application.Operation.Features.Employee.JobManagement.JobOperations.DTOs;

public class JobPointsMainResponseDto
{
    public Guid Id { get; set; }
    public Guid JobId { get; set; }

    public int ApplicantCategory { get; set; }
    public int Education { get; set; }
    public int Experience { get; set; }
    public int Training { get; set; }
    public int Certificates { get; set; }
    public int Skills { get; set; }
    public int Languages { get; set; }
    public int Total { get; set; }
    public bool IsApproved { get; set; }
    public Guid CreatedById { get; set; }
    public List<JobPointsDetailResponseDto> Details { get; set; } = [];
}
