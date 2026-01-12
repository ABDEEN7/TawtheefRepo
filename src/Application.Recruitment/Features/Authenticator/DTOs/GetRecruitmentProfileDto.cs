namespace Application.Recruitment.Features.Authenticator.DTOs;

public class GetRecruitmentProfileDto
{
    public Guid UserId { get; set; }
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public required string PhoneNumber { get; set; }
    public required string Avatar { get; set; }
    public bool AgreedToTerms { get; set; }
}
