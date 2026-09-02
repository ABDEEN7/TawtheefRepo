namespace Tawtheef.Application.Common.Utils;

public static class MoiUtils
{
    public const int QatarNationalityCode = 634;
    public const int QidHolderNationalityCode = 0;

    public static string MaskQid(string? qid)
    {
        if (string.IsNullOrWhiteSpace(qid)) return "—";
        // keep last 3 digits only
        var digits = new string(qid.Where(char.IsDigit).ToArray());
        if (digits.Length <= 3) return "***";
        return new string('*', digits.Length - 3) + digits[^3..];
    }

    public static bool IsQatarMobileNumber(string phone) => phone.StartsWith("+974");

    public static string NormalizePhone(string phone)
    {
        var digits = new string(phone.Where(char.IsDigit).ToArray());
        if (string.IsNullOrWhiteSpace(digits)) return string.Empty;

        if (digits.StartsWith("974") && digits.Length == 11) return "+" + digits;
        if (digits.Length == 8) return "+974" + digits;

        return "+" + digits;
    }
}
