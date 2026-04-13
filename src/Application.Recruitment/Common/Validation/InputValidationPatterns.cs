namespace Application.Recruitment.Common.Validation;

public static class InputValidationPatterns
{
    public const string Textbox = @"^[A-Za-z0-9\u0600-\u06FF\u0750-\u077F\s\-\u2013\u2014]+$";
    public const string TextArea = @"^[A-Za-z0-9\u0600-\u06FF\u0750-\u077F\s\-\u2013\u2014\'\.,\r\n]+$";
    public const string Email = @"^[A-Za-z0-9._%+\-]+@([A-Za-z0-9]+(-[A-Za-z0-9]+)*\.)+[A-Za-z]{2,}$";
    public const string NationalNumber = @"^[A-Za-z0-9\-]{6,20}$";
}
