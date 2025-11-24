namespace Tawtheef.Application.Common.Constants;

public static class JobValidationMessages
{
    public const string JobIdRequired = "Job ID is required";

    public const string JobRequired = "Job data is required";
    public const string JobBasicsRequired = "Job basics are required";
    
    public const string JobTitleRequired = "Job title is required";
    public const string JobTitleMaxLength = "Job title cannot exceed 200 characters";
    
    public const string VacanciesGreaterThanZero = "Vacancies must be greater than 0";

    public const string DeadlineAtLeast24Hours = "Deadline must be at least 24 hours from now";

    public const string RequestingDeptRequired = "Requesting department is required";

    public const string JobQuotasRequired = "Job quotas are required";
    public const string JobQuotaTotalInvalid = "Job quotas must total 100%";

    public const string QuotaPercentageGreaterThanZero = "Percentage must be greater than 0";
    public const string QuotaPercentageMax100 = "Percentage cannot exceed 100";

    public const string ConditionRequired = "Condition text cannot be empty";
    public const string ConditionMaxLength = "Condition cannot exceed 500 characters";

    public const string SkillRequired = "Skill text cannot be empty";
    public const string SkillMaxLength = "Skill cannot exceed 100 characters";
    
    public const string JobNotFound = "Job not found";
    public const string JobDeleteFailed = "Failed to delete job";
}
