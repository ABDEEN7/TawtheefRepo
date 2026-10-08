namespace Tawtheef.Domain.Configurations.Settings;

public sealed class CandidateExamRealtimeSettings
{
    public const string SectionName = "CandidateExam";
    public int InterruptionGraceSeconds { get; init; } = 120;
    public int ConnectionLeaseSeconds { get; init; } = 90;
    public int ConnectionRenewalSeconds { get; init; } = 30;
    
    public int ResultGenerationIntervalSeconds { get; init; } = 30;
}
