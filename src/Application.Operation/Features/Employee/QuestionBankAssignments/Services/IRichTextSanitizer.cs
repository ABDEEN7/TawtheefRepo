namespace Application.Operation.Features.Employee.QuestionBankAssignments.Services;

public interface IRichTextSanitizer
{
    string? Sanitize(string? html);

    bool HasMeaningfulContent(string? html);
}
