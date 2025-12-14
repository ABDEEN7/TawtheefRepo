// Application/Common/Constants/JobValidationMessages.cs
namespace Tawtheef.Domain.Constants;

public static class JobValidationMessages
{
    public const string FIELD_REQUIRED = "field_required";
    public const string SKILLS_REQUIRE_MAJOR_DEGREES = "skills_require_major_degrees";
    public const string CANNOT_MODIFY_AGE_RANGE = "cannot_modify_age_range";
    public const string CANNOT_CLOSE_MANUALLY_BEFORE_DEADLINE = "cannot_close_manually_before_deadline";
    // «·ÕﬁÊ· «·≈·“«„Ì…
    public const string JOB_TITLE_AR_REQUIRED = "job_title_ar_required";
    public const string JOB_TITLE_EN_REQUIRED = "job_title_en_required";
    public const string JOB_TITLE_AR_MAX_LENGTH = "job_title_ar_max_length";
    public const string JOB_TITLE_EN_MAX_LENGTH = "job_title_en_max_length";
    public const string SECTOR_REQUIRED = "sector_required";
    public const string MANAGEMENT_REQUIRED = "management_required";
    public const string DEPARTMENT_REQUIRED = "department_required";
    public const string YEARS_EXPERIENCE_REQUIRED = "years_experience_required";
    public const string YEARS_EXPERIENCE_RANGE = "years_experience_range";
    public const string JOB_CATEGORY_REQUIRED = "job_category_required";
    public const string WORK_LOCATION_REQUIRED = "work_location_required";
    public const string MAJOR_REQUIRED = "major_required";
    public const string WORK_TYPE_REQUIRED = "work_type_required";
    public const string VACANCIES_REQUIRED = "vacancies_required";
    public const string VACANCIES_GREATER_THAN_ZERO = "vacancies_greater_than_zero";
    public const string CLOSING_DATE_REQUIRED = "closing_date_required";
    public const string MINIMUM_AGE_REQUIRED = "minimum_age_required";
    public const string MAXIMUM_AGE_REQUIRED = "maximum_age_required";

    public const string JOB_NOT_FOUND = "job_not_found";
    public const string JOB_REQUIRED = "job_required";
    public const string JOB_ID_REQUIRED = "job_id_required";
    
    
    public const string UPDATE_FAILED = "job_update_failed";


    // «·ﬁÊ«⁄œ «·Œ«’…
    public const string CLOSING_DATE_FUTURE = "closing_date_future";
    public const string MIN_AGE_SYSTEM_LIMIT = "min_age_system_limit";
    public const string MAX_AGE_SYSTEM_LIMIT = "max_age_system_limit";
    public const string AGE_RANGE_INVALID = "age_range_invalid";
    public const string DUPLICATE_JOB = "duplicate_job";

    // Õ«·… «·ÊŸÌ›…
    public const string JOB_STATUS_REQUIRED = "job_status_required";
    public const string ALL_TABS_REQUIRED = "all_tabs_required";
    public const string CAN_ONLY_EDIT_IN_DRAFT = "can_only_edit_in_draft";
    public const string CANNOT_EDIT_IN_APPROVAL = "cannot_edit_in_approval";
    public const string CAN_ONLY_PUBLISH_APPROVED = "can_only_publish_approved";
    public const string INVALID_STATUS_TRANSITION = "invalid_status_transition";

    // «· ”·”· «·Â—„Ì
    public const string MANAGEMENT_NOT_UNDER_SECTOR = "management_not_under_sector";
    public const string DEPARTMENT_NOT_UNDER_MANAGEMENT = "department_not_under_management";
    public const string SUBMAJOR_NOT_UNDER_MAJOR = "submajor_not_under_major";

    // «· »ÊÌ»« 
    public const string CONDITION_TEXT_REQUIRED = "condition_text_required";
    public const string RESPONSIBILITY_TEXT_REQUIRED = "responsibility_text_required";
    public const string ATTACHMENT_TITLE_REQUIRED = "attachment_title_required";
    public const string DUPLICATE_CONDITION = "duplicate_condition";
    public const string DUPLICATE_RESPONSIBILITY = "duplicate_responsibility";
    public const string DUPLICATE_ATTACHMENT = "duplicate_attachment";
    public const string DUPLICATE_SKILL = "duplicate_skill";
    public const string CANNOT_MODIFY_AFTER_APPROVAL = "cannot_modify_after_approval";
    public const string ATTACHMENT_TITLE_MAX_LENGTH = "attachment_title_max_length";
    
    public const string JOB_OVERVIEW_EN_REQUIRED = "job_overview_en_required";
    public const string JOB_OVERVIEW_AR_REQUIRED = "job_overview_ar_required";
    
    public const string JOB_OVERVIEW_EN_MAX_LENGTH = "job_overview_en_max_length";
    public const string JOB_OVERVIEW_AR_MAX_LENGTH = "job_overview_ar_max_length";

    public const string JOB_BENEFITS_EN_REQUIRED = "job_benefits_en_required";
    public const string JOB_BENEFITS_AR_REQUIRED = "job_benefits_ar_required";

    public const string JOB_BENEFITS_EN_MAX_LENGTH = "job_benefits_en_max_length";
    public const string JOB_BENEFITS_AR_MAX_LENGTH = "job_benefits_ar_max_length";

    // «·„Â«—« 
    public const string SKILL_NOT_IN_MAJOR = "skill_not_in_major";
    public const string CANNOT_CHANGE_SKILL_VISIBILITY = "cannot_change_skill_visibility";

    // ≈⁄œ«œ«  «·‰ﬁ«ÿ
    public const string POINTS_SETTINGS_REQUIRED = "points_settings_required";
    public const string CANNOT_SEND_INVITATIONS = "cannot_send_invitations";
    public const string CANNOT_PUBLISH_NO_POINTS = "cannot_publish_no_points";

    // Õ«·«  ‰Â«∆Ì…
    public const string JOB_EXPIRED = "job_expired";
    public const string CANNOT_APPLY = "cannot_apply";
    public const string CANNOT_MODIFY_TITLE = "cannot_modify_title";
    public const string CANNOT_MODIFY_QUALIFICATIONS = "cannot_modify_qualifications";
   
    public const string CANNOT_MODIFY_SKILLS = "cannot_modify_skills";

    public const string CONDITION_MAX_LENGTH = "condition_max_length";
    public const string RESPONSIBILITY_MAX_LENGTH = "responsibilty_Max_Length";

    public const string CANNOT_CANCEL_FINAL_STATE = "cannot_cancel_final_state";

    public const string JOB_QUALIFICATIONDESCRIPTION_EN_REQUIRED = "job_qualificationdescription_en_required";
    public const string JOB_QUALIFICATIONDESCRIPTION_AR_REQUIRED = "job_qualificationdescription_ar_required";

    public const string JOB_QUALIFICATIONDESCRIPTION_AR_MAX_LENGTH = "job_qualificationdescription_ar_max_length";
    public const string JOB_QUALIFICATIONDESCRIPTION_EN_MAX_LENGTH = "job_qualificationdescription_en_max_length";

    public const string JOB_DESCRIPTION_AR_REQUIRED = "job_description_ar_required";
    public const string JOB_DESCRIPTION_EN_REQUIRED = "job_description_en_required";

    public const string JOB_DESCRIPTION_AR_MAX_LENGTH = "job_description_ar_max_length";
    public const string JOB_DESCRIPTION_EN_MAX_LENGTH = "job_description_en_max_length";

}
