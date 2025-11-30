namespace Tawtheef.Application.Common.Constants;

public static class JobValidationMessages
{
    public const string JOB_ID_REQUIRED = "job_id_required";

    public const string JOB_REQUIRED = "job_required";

    public const string JOB_TITLE_REQUIRED = "job_title_required";
    public const string JOB_TITLE_MAX_LENGTH = "job_title_max_length";

    public const string VACANCIES_GREATER_THAN_ZERO = "vacancies_greater_than_zero";

    public const string DEADLINE_AT_LEAST_24_HOURS = "deadline_at_least_24_hours";

    public const string REQUESTING_DEPT_REQUIRED = "requesting_dept_required";

    public const string JOB_QUOTAS_REQUIRED = "job_quotas_required";
    public const string JOB_QUOTA_TOTAL_INVALID = "job_quota_total_invalid";

    public const string QUOTA_PERCENTAGE_GREATER_THAN_ZERO = "quota_percentage_greater_than_zero";
    public const string QUOTA_PERCENTAGE_MAX_100 = "quota_percentage_max_100";

    public const string CONDITION_REQUIRED = "condition_required";
    public const string CONDITION_MAX_LENGTH = "condition_max_length";

    public const string SKILL_REQUIRED = "skill_required";
    public const string SKILL_MAX_LENGTH = "skill_max_length";

    public const string QUOTA_NOT_FOUND = "quota_not_found";
    public const string QUOTA_ID_REQUIRED = "quota_id_required";

    public const string JOB_NOT_FOUND = "job_not_found";
    public const string JOB_DELETE_FAILED = "job_delete_failed";

    public const string JOB_RESIDENTS_BREAKDOWN_NOT_FOUND = "job_residents_breakdown_not_found";

    public const string UPDATE_FAILED = "update_failed";
}
