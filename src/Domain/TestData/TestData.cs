namespace Tawtheef.Domain.TestData;

public static class TestData
{
    public static List<long> QID_TEST()
    {
        var envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        if (string.Equals(envName, "Production", StringComparison.OrdinalIgnoreCase))
            return [];
        
        //Ask by Asmita for Testing
        return [28963404424, 29273602238, 27835624342];
    }

}
