namespace Tawtheef.Application.Common.Constants;

public static class JobValidationMessages
{
    // Existing messages...
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
    public const string QUOTA_NATIONALITY_REQUIRED = "quota_nationality_required";
    public const string JOB_QUOTA_BREAKDOWN_REQUIRED = "job_quota_breakdown_required";

    // ----------------------------------------------------------
    // NEW MESSAGES BASED ON BRD AND VALIDATOR UPDATES
    // ----------------------------------------------------------

    // Job Description & Benefits
    public const string JOB_DESCRIPTION_REQUIRED = "job_description_required";
    public const string JOB_DESCRIPTION_MAX_LENGTH = "job_description_max_length";
    public const string JOB_BENEFITS_REQUIRED = "job_benefits_required";
    public const string JOB_BENEFITS_MAX_LENGTH = "job_benefits_max_length";

    // New Fields from BRD
    public const string MINIMUM_EXPERIENCE_NON_NEGATIVE = "minimum_experience_non_negative";
    public const string MINIMUM_AGE_VALID = "minimum_age_valid";
    public const string MAXIMUM_AGE_GREATER_THAN_MINIMUM = "maximum_age_greater_than_minimum";
    public const string AGE_WITHIN_SYSTEM_LIMITS = "age_within_system_limits";

    // Overview & Qualifications
    public const string JOB_OVERVIEW_MAX_LENGTH = "job_overview_max_length";
    public const string JOB_QUALIFICATIONS_MAX_LENGTH = "job_qualifications_max_length";

    // Foreign Keys (Required Fields)
    public const string SECTOR_REQUIRED = "sector_required";
    public const string MANAGEMENT_REQUIRED = "management_required";
    public const string JOB_CATEGORY_REQUIRED = "job_category_required";
    public const string WORK_LOCATION_REQUIRED = "work_location_required";
    public const string MAJOR_REQUIRED = "major_required";
    public const string WORK_TYPE_REQUIRED = "work_type_required";
    public const string DEGREES_REQUIRED = "degrees_required";

    // New Collections from BRD
    public const string RESPONSIBILITIES_REQUIRED = "responsibilities_required";
    public const string RESPONSIBILITY_REQUIRED = "responsibility_required";
    public const string RESPONSIBILITY_MAX_LENGTH = "responsibility_max_length";

    public const string REQUIRED_ATTACHMENTS_REQUIRED = "required_attachments_required";
    public const string ATTACHMENT_TITLE_REQUIRED = "attachment_title_required";
    public const string ATTACHMENT_TITLE_MAX_LENGTH = "attachment_title_max_length";

    // Quota Percentage Range
    public const string QUOTA_PERCENTAGE_RANGE = "quota_percentage_range";

    // Job Status & Workflow
    public const string JOB_STATUS_REQUIRED = "job_status_required";
    public const string JOB_STATUS_INVALID_TRANSITION = "job_status_invalid_transition";
    public const string JOB_CANNOT_BE_MODIFIED = "job_cannot_be_modified";
    public const string JOB_NOT_IN_DRAFT = "job_not_in_draft";
    public const string JOB_NOT_PENDING_APPROVAL = "job_not_pending_approval";
    public const string JOB_NOT_APPROVED = "job_not_approved";
    public const string JOB_ALREADY_PUBLISHED = "job_already_published";
    public const string JOB_ALREADY_CLOSED = "job_already_closed";
    public const string JOB_ALREADY_CANCELLED = "job_already_cancelled";

    // Duplication Check
    public const string DUPLICATE_JOB = "duplicate_job";
    public const string DUPLICATE_JOB_SAME_SETTINGS = "duplicate_job_same_settings";

    // Date Validations
    public const string DEADLINE_FUTURE_DATE = "deadline_future_date";
    public const string PUBLISH_DATE_FUTURE = "publish_date_future";
    public const string DEADLINE_AFTER_PUBLISH = "deadline_after_publish";

    // SubMajor Validation
    public const string SUBMAJOR_NOT_UNDER_MAJOR = "submajor_not_under_major";

    // Department Hierarchy Validation
    public const string DEPARTMENT_NOT_UNDER_MANAGEMENT = "department_not_under_management";
    public const string MANAGEMENT_NOT_UNDER_SECTOR = "management_not_under_sector";

    // Age System Limits
    public const string AGE_SYSTEM_LIMITS_NOT_SET = "age_system_limits_not_set";

    // Job Quota Validation
    public const string QUOTA_SUM_NOT_100 = "quota_sum_not_100";
    public const string RESIDENTS_BREAKDOWN_SUM_NOT_100 = "residents_breakdown_sum_not_100";

    // Skill Validation
    public const string SKILL_NOT_IN_MAJOR = "skill_not_in_major";
    public const string SKILL_SHOW_TO_APPLICANTS_REQUIRED = "skill_show_to_applicants_required";

    // Attachment Validation
    public const string ATTACHMENT_DUPLICATE_TITLE = "attachment_duplicate_title";

    // Collection Item Validation
    public const string COLLECTION_ITEM_DUPLICATE = "collection_item_duplicate";
    public const string COLLECTION_ITEM_EMPTY = "collection_item_empty";

    // User Permissions
    public const string USER_NO_PERMISSION_CREATE = "user_no_permission_create";
    public const string USER_NO_PERMISSION_UPDATE = "user_no_permission_update";
    public const string USER_NO_PERMISSION_APPROVE = "user_no_permission_approve";
    public const string USER_NO_PERMISSION_PUBLISH = "user_no_permission_publish";
    public const string USER_NO_PERMISSION_CANCEL = "user_no_permission_cancel";

    // System Settings
    public const string SYSTEM_SETTINGS_NOT_FOUND = "system_settings_not_found";

    // Job Reference Data
    public const string REFERENCE_DATA_NOT_FOUND = "reference_data_not_found";
    public const string MAJOR_NOT_FOUND = "major_not_found";
    public const string SUBMAJOR_NOT_FOUND = "submajor_not_found";
    public const string DEGREE_NOT_FOUND = "degree_not_found";
    public const string GENDER_NOT_FOUND = "gender_not_found";
    public const string SECTOR_NOT_FOUND = "sector_not_found";
    public const string MANAGEMENT_NOT_FOUND = "management_not_found";
    public const string DEPARTMENT_NOT_FOUND = "department_not_found";
    public const string JOB_CATEGORY_NOT_FOUND = "job_category_not_found";
    public const string WORK_LOCATION_NOT_FOUND = "work_location_not_found";
    public const string WORK_TYPE_NOT_FOUND = "work_type_not_found";
    public const string JOB_STATUS_NOT_FOUND = "job_status_not_found";
    public const string NATIONALITY_NOT_FOUND = "nationality_not_found";

    // Job Workflow Actions
    public const string JOB_APPROVE_SUCCESS = "job_approve_success";
    public const string JOB_REJECT_SUCCESS = "job_reject_success";
    public const string JOB_PUBLISH_SUCCESS = "job_publish_success";
    public const string JOB_CANCEL_SUCCESS = "job_cancel_success";
    public const string JOB_CLOSE_SUCCESS = "job_close_success";
    public const string JOB_SEND_FOR_APPROVAL_SUCCESS = "job_send_for_approval_success";

    // Success Messages
    public const string JOB_CREATED_SUCCESS = "job_created_success";
    public const string JOB_UPDATED_SUCCESS = "job_updated_success";
    public const string JOB_DELETED_SUCCESS = "job_deleted_success";
    public const string JOB_RETRIEVED_SUCCESS = "job_retrieved_success";
    public const string JOBS_RETRIEVED_SUCCESS = "jobs_retrieved_success";

    // File Upload
    public const string FILE_UPLOAD_FAILED = "file_upload_failed";
    public const string FILE_SIZE_EXCEEDED = "file_size_exceeded";
    public const string FILE_TYPE_NOT_ALLOWED = "file_type_not_allowed";

    // Notification
    public const string NOTIFICATION_SENT_SUCCESS = "notification_sent_success";
    public const string NOTIFICATION_FAILED = "notification_failed";

    // Audit Log
    public const string AUDIT_LOG_CREATED = "audit_log_created";

    // Export
    public const string EXPORT_SUCCESS = "export_success";
    public const string EXPORT_FAILED = "export_failed";

    // Search & Filter
    public const string INVALID_SEARCH_PARAMETERS = "invalid_search_parameters";
    public const string INVALID_PAGE_NUMBER = "invalid_page_number";
    public const string INVALID_PAGE_SIZE = "invalid_page_size";
    public const string INVALID_SORT_FIELD = "invalid_sort_field";
}
