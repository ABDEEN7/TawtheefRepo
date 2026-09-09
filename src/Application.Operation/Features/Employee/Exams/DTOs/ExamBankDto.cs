namespace Application.Operation.Features.Employee.Exams.DTOs;

public sealed record ExamBankDto(Guid Id, Guid CategoryId, string NameAr, string NameEn,
    int VersionNo, int Total, int Easy, int Medium, int Hard);
