namespace Application.Recruitment.Features.Dashboard.DTOs;

public sealed class CandidateInvitationStatisticsDto
{
    public int NewInvitations { get; set; }
    public int Withdrawn { get; set; }
    public int Applied { get; set; }
}
