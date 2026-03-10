namespace Application.Recruitment.Common.Validation;

public static class InputValidationPatterns
{
    public const string Textbox = @"^[A-Za-z0-9\u0600-\u06FF\u0750-\u077F\s]+$";
    public const string TextArea = @"^[A-Za-z0-9\u0600-\u06FF\u0750-\u077F\s\-\'\.,\r\n]+$";
    public const string Email = @"^[A-Za-z0-9_@\.]+$";
}
