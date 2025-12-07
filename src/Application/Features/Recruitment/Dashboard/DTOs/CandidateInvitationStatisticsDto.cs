namespace Tawtheef.Application.Features.Recruitment.Dashboard.DTOs;

public sealed class CandidateInvitationStatisticsDto
{
    public int NewInvitations { get; set; }
    public int UnderReview { get; set; }
    public int Withdrawn { get; set; }
    public int Applied { get; set; }
}
