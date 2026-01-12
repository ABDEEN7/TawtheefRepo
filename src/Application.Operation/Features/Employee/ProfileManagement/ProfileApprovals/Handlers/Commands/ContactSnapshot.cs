namespace Application.Operation.Features.Employee.ProfileManagement.ProfileApprovals.Handlers.Commands;

public sealed record ContactSnapshot
{
    public Guid? ResidenceCountryId { get; init; }
    public Guid? InterviewLocationId { get; init; }
    public string? Address { get; init; }
    public int? Zone { get; init; }
    public int? Street { get; init; }
    public int? Building { get; init; }
    public int? Unit { get; init; }
    public Guid? NationalAddressCertificateId { get; init; }
}