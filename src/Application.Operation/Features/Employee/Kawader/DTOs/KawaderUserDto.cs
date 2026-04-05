namespace Application.Operation.Features.Employee.Kawader.DTOs;

public class KawaderUserDto
{
    public string Qid { get; set; } = string.Empty;
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public bool IsInvited { get; set; }
    public DateTime? InvitedAt { get; set; }
    public DateTime CreatedDate { get; set; }
}
