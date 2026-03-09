namespace Application.Recruitment.Common.Validation;

public static class InputValidationPatterns
{
    public const string Textbox = @"^[A-Za-z0-9\s]+$";
    public const string TextArea = @"^[A-Za-z0-9\s\-\'\.,\r\n]+$";
    public const string Email = @"^[A-Za-z0-9_@\.]+$";
}
