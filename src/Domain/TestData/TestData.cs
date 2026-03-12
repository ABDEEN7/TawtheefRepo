namespace Domain.TestData;

public static class TestData
{
    public static List<long> QID_TEST()
    {
        if (!string.Equals(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"), "Production",
                StringComparison.OrdinalIgnoreCase))
            return [28963404424, 29273602238, 27835624342];
        return new List<long>();
    }

}
