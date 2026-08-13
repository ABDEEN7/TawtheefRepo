namespace Tawtheef.Domain.Constants;

public static class UniversityIds
{
    public static readonly Guid Other =
        Guid.Parse("1c180819-90ce-43f1-a8bb-753d85ea33a9");

    public static bool IsOther(Guid? universityId) =>
        universityId.HasValue && universityId.Value == Other;
}
