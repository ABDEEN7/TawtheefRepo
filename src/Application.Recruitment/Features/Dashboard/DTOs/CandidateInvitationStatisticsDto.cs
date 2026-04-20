namespace Application.Recruitment.Features.Dashboard.DTOs;

public sealed class CandidateInvitationStatisticsDto
{
    public int Received { get; set; }
    public int Accepted { get; set; }
    public int Rejected { get; set; }
}
