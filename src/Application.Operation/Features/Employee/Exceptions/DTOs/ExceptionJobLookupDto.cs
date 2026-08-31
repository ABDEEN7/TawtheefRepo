namespace Application.Operation.Features.Employee.Exceptions.DTOs;

public sealed record ExceptionJobLookupDto(
    Guid JobId,
    string JobNumber,
    string JobTitle,
    DateTimeOffset ClosingDate,
    Guid JobStatusId,
    string Status);
