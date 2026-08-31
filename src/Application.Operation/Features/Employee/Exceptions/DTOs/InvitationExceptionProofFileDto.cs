namespace Application.Operation.Features.Employee.Exceptions.DTOs;

public sealed record InvitationExceptionProofFileDto(
    Stream Stream,
    string FileName,
    string ContentType);
