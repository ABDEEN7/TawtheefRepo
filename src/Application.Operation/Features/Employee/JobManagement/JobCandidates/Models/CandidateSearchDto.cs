using System;

namespace Application.Operation.Features.Employee.JobManagement.JobCandidates.Models;

public class CandidateSearchDto
{
    public Guid CandidateId { get; set; }
    public string? FullName { get; set; }
    public string? NationalId { get; set; }
    public string? Email { get; set; }
}
