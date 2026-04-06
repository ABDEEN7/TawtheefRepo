namespace Application.Operation.Features.Employee.MinisterOffice.DTOs;

public sealed record CreateMinisterOfficeCandidateRequest(
    string Qid,
    string PhoneNumber,
    DateOnly QidExpiryDate);
