namespace Tawtheef.Domain.TestData;

public static class TestData
{
    public static List<long> QID_TEST()
    {
        var envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        if (string.Equals(envName, "Production", StringComparison.OrdinalIgnoreCase))
            return [];
        
        //Ask by Asmita for Testing
        return [28963404424, 29273602238, 27835624342, 
            29740002204, 29840002179, 29640002329, 29540002683, 
            29740002075, 29573603987, 29540002282, 29840002105,
            29040002493];
    }

}
