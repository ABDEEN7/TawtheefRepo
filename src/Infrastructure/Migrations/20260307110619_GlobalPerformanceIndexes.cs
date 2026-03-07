using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GlobalPerformanceIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_WorkType_CreatedDate",
                schema: "lkp",
                table: "WorkType",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_WorkType_IsDeleted",
                schema: "lkp",
                table: "WorkType",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_UserType_CreatedDate",
                schema: "lkp",
                table: "UserType",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_UserType_IsDeleted",
                schema: "lkp",
                table: "UserType",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_UserSession_CreatedDate",
                table: "UserSession",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_UserSession_IsDeleted",
                table: "UserSession",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfileLogger_CreatedDate",
                schema: "hr",
                table: "UserProfileLogger",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfileLogger_IsDeleted",
                schema: "hr",
                table: "UserProfileLogger",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfile_CreatedDate",
                schema: "app",
                table: "UserProfile",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfile_IsDeleted",
                schema: "app",
                table: "UserProfile",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_University_CreatedDate",
                schema: "lkp",
                table: "University",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_University_IsDeleted",
                schema: "lkp",
                table: "University",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingCourse_CreatedDate",
                schema: "pro",
                table: "TrainingCourse",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingCourse_IsDeleted",
                schema: "pro",
                table: "TrainingCourse",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_TargetEntity_CreatedDate",
                schema: "lkp",
                table: "TargetEntity",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_TargetEntity_IsDeleted",
                schema: "lkp",
                table: "TargetEntity",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_StudyType_CreatedDate",
                schema: "lkp",
                table: "StudyType",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_StudyType_IsDeleted",
                schema: "lkp",
                table: "StudyType",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_SponsorType_CreatedDate",
                schema: "lkp",
                table: "SponsorType",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_SponsorType_IsDeleted",
                schema: "lkp",
                table: "SponsorType",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_SponsorProfile_CreatedDate",
                schema: "app",
                table: "SponsorProfile",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_SponsorProfile_IsDeleted",
                schema: "app",
                table: "SponsorProfile",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_SkillType_CreatedDate",
                schema: "lkp",
                table: "SkillType",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_SkillType_IsDeleted",
                schema: "lkp",
                table: "SkillType",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_SkillLevel_CreatedDate",
                schema: "lkp",
                table: "SkillLevel",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_SkillLevel_IsDeleted",
                schema: "lkp",
                table: "SkillLevel",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Skill_CreatedDate",
                schema: "lkp",
                table: "Skill",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_Skill_IsDeleted",
                schema: "lkp",
                table: "Skill",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Sector_CreatedDate",
                schema: "lkp",
                table: "Sector",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_Sector_IsDeleted",
                schema: "lkp",
                table: "Sector",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewItem_CreatedDate",
                schema: "hr",
                table: "ReviewItem",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewItem_IsDeleted",
                schema: "hr",
                table: "ReviewItem",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Resources_CreatedDate",
                table: "Resources",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_Resources_IsDeleted",
                table: "Resources",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_ResidenceAddress_CreatedDate",
                schema: "pro",
                table: "ResidenceAddress",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_ResidenceAddress_IsDeleted",
                schema: "pro",
                table: "ResidenceAddress",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Religion_CreatedDate",
                schema: "lkp",
                table: "Religion",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_Religion_IsDeleted",
                schema: "lkp",
                table: "Religion",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshToken_CreatedDate",
                table: "RefreshToken",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshToken_IsDeleted",
                table: "RefreshToken",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_RatingGrade_CreatedDate",
                schema: "lkp",
                table: "RatingGrade",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_RatingGrade_IsDeleted",
                schema: "lkp",
                table: "RatingGrade",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Qualification_CreatedDate",
                schema: "pro",
                table: "Qualification",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_Qualification_IsDeleted",
                schema: "pro",
                table: "Qualification",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderLogin_CreatedDate",
                schema: "lkp",
                table: "ProviderLogin",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderLogin_IsDeleted",
                schema: "lkp",
                table: "ProviderLogin",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileSkill_CreatedDate",
                schema: "pro",
                table: "ProfileSkill",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileSkill_IsDeleted",
                schema: "pro",
                table: "ProfileSkill",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileReviewDecision_CreatedDate",
                schema: "hr",
                table: "ProfileReviewDecision",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileReviewDecision_IsDeleted",
                schema: "hr",
                table: "ProfileReviewDecision",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileLanguage_CreatedDate",
                schema: "pro",
                table: "ProfileLanguage",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileLanguage_IsDeleted",
                schema: "pro",
                table: "ProfileLanguage",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileChangeRequest_CreatedDate",
                schema: "hr",
                table: "ProfileChangeRequest",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileChangeRequest_IsDeleted",
                schema: "hr",
                table: "ProfileChangeRequest",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileAssignment_CreatedDate",
                schema: "hr",
                table: "ProfileAssignment",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileAssignment_IsDeleted",
                schema: "hr",
                table: "ProfileAssignment",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileAdditionalAttachment_CreatedDate",
                schema: "pro",
                table: "ProfileAdditionalAttachment",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileAdditionalAttachment_IsDeleted",
                schema: "pro",
                table: "ProfileAdditionalAttachment",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Permission_CreatedDate",
                schema: "lkp",
                table: "Permission",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_Permission_IsDeleted",
                schema: "lkp",
                table: "Permission",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_OfficeSupportedCountry_CreatedDate",
                schema: "lkp",
                table: "OfficeSupportedCountry",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_OfficeSupportedCountry_IsDeleted",
                schema: "lkp",
                table: "OfficeSupportedCountry",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Office_CreatedDate",
                schema: "lkp",
                table: "Office",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_Office_IsDeleted",
                schema: "lkp",
                table: "Office",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_CreatedDate",
                table: "Notifications",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_IsDeleted",
                table: "Notifications",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_MaritalStatus_CreatedDate",
                schema: "lkp",
                table: "MaritalStatus",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_MaritalStatus_IsDeleted",
                schema: "lkp",
                table: "MaritalStatus",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Management_CreatedDate",
                schema: "lkp",
                table: "Management",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_Management_IsDeleted",
                schema: "lkp",
                table: "Management",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_MajorSkill_CreatedDate",
                schema: "hr",
                table: "MajorSkill",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_MajorSkill_IsDeleted",
                schema: "hr",
                table: "MajorSkill",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Major_CreatedDate",
                schema: "lkp",
                table: "Major",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_Major_IsDeleted",
                schema: "lkp",
                table: "Major",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_LoginAttempt_CreatedDate",
                table: "LoginAttempt",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_LoginAttempt_IsDeleted",
                table: "LoginAttempt",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_LanguageLevel_CreatedDate",
                schema: "lkp",
                table: "LanguageLevel",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_LanguageLevel_IsDeleted",
                schema: "lkp",
                table: "LanguageLevel",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Language_CreatedDate",
                schema: "lkp",
                table: "Language",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_Language_IsDeleted",
                schema: "lkp",
                table: "Language",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_KawaderQids_CreatedDate",
                table: "KawaderQids",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_KawaderQids_IsDeleted",
                table: "KawaderQids",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_JobTitle_CreatedDate",
                schema: "lkp",
                table: "JobTitle",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_JobTitle_IsDeleted",
                schema: "lkp",
                table: "JobTitle",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_JobTabReviewNote_CreatedDate",
                schema: "hr",
                table: "JobTabReviewNote",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_JobTabReviewNote_IsDeleted",
                schema: "hr",
                table: "JobTabReviewNote",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_JobStatus_CreatedDate",
                schema: "lkp",
                table: "JobStatus",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_JobStatus_IsDeleted",
                schema: "lkp",
                table: "JobStatus",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_JobSkill_CreatedDate",
                schema: "hr",
                table: "JobSkill",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_JobSkill_IsDeleted",
                schema: "hr",
                table: "JobSkill",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_JobReviewAttachment_CreatedDate",
                schema: "hr",
                table: "JobReviewAttachment",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_JobReviewAttachment_IsDeleted",
                schema: "hr",
                table: "JobReviewAttachment",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_JobResponsibility_CreatedDate",
                schema: "hr",
                table: "JobResponsibility",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_JobResponsibility_IsDeleted",
                schema: "hr",
                table: "JobResponsibility",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_JobRequiredAttachment_CreatedDate",
                schema: "hr",
                table: "JobRequiredAttachment",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_JobRequiredAttachment_IsDeleted",
                schema: "hr",
                table: "JobRequiredAttachment",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_JobPointsMain_CreatedDate",
                schema: "hr",
                table: "JobPointsMain",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_JobPointsMain_IsDeleted",
                schema: "hr",
                table: "JobPointsMain",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_JobPointsDetail_CreatedDate",
                schema: "hr",
                table: "JobPointsDetail",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_JobPointsDetail_IsDeleted",
                schema: "hr",
                table: "JobPointsDetail",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_JobPointConfiguration_CreatedDate",
                schema: "hr",
                table: "JobPointConfiguration",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_JobPointConfiguration_IsDeleted",
                schema: "hr",
                table: "JobPointConfiguration",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_JobDegree_CreatedDate",
                schema: "hr",
                table: "JobDegree",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_JobDegree_IsDeleted",
                schema: "hr",
                table: "JobDegree",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_JobCondition_CreatedDate",
                schema: "hr",
                table: "JobCondition",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_JobCondition_IsDeleted",
                schema: "hr",
                table: "JobCondition",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_JobCategoryCandidateSettings_CreatedDate",
                schema: "hr",
                table: "JobCategoryCandidateSettings",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_JobCategoryCandidateSettings_IsDeleted",
                schema: "hr",
                table: "JobCategoryCandidateSettings",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_JobCategory_CreatedDate",
                schema: "lkp",
                table: "JobCategory",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_JobCategory_IsDeleted",
                schema: "lkp",
                table: "JobCategory",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_JobCandidateTypePercentage_CreatedDate",
                schema: "hr",
                table: "JobCandidateTypePercentage",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_JobCandidateTypePercentage_IsDeleted",
                schema: "hr",
                table: "JobCandidateTypePercentage",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_JobCandidateNationalityPercentage_CreatedDate",
                schema: "hr",
                table: "JobCandidateNationalityPercentage",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_JobCandidateNationalityPercentage_IsDeleted",
                schema: "hr",
                table: "JobCandidateNationalityPercentage",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_JobCandidateFilterSetting_CreatedDate",
                schema: "hr",
                table: "JobCandidateFilterSetting",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_JobCandidateFilterSetting_IsDeleted",
                schema: "hr",
                table: "JobCandidateFilterSetting",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Job_CreatedDate",
                schema: "hr",
                table: "Job",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_Job_IsDeleted",
                schema: "hr",
                table: "Job",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_InvitationStatus_CreatedDate",
                schema: "lkp",
                table: "InvitationStatus",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_InvitationStatus_IsDeleted",
                schema: "lkp",
                table: "InvitationStatus",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Invitation_CreatedDate",
                schema: "hr",
                table: "Invitation",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_Invitation_IsDeleted",
                schema: "hr",
                table: "Invitation",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_HomeSuccessStory_CreatedDate",
                schema: "app",
                table: "HomeSuccessStory",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_HomeSuccessStory_IsDeleted",
                schema: "app",
                table: "HomeSuccessStory",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_HistoryInvitation_CreatedDate",
                table: "HistoryInvitation",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_HistoryInvitation_IsDeleted",
                table: "HistoryInvitation",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Gender_CreatedDate",
                schema: "lkp",
                table: "Gender",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_Gender_IsDeleted",
                schema: "lkp",
                table: "Gender",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_FAQ_CreatedDate",
                schema: "app",
                table: "FAQ",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_FAQ_IsDeleted",
                schema: "app",
                table: "FAQ",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Experience_CreatedDate",
                schema: "pro",
                table: "Experience",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_Experience_IsDeleted",
                schema: "pro",
                table: "Experience",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_EntityLog_CreatedDate",
                table: "EntityLog",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_EntityLog_IsDeleted",
                table: "EntityLog",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeProfile_CreatedDate",
                table: "EmployeeProfile",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeProfile_IsDeleted",
                table: "EmployeeProfile",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_EmailTemplates_CreatedDate",
                table: "EmailTemplates",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_EmailTemplates_IsDeleted",
                table: "EmailTemplates",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_EmailQueues_CreatedDate",
                table: "EmailQueues",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_EmailQueues_IsDeleted",
                table: "EmailQueues",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Department_CreatedDate",
                schema: "lkp",
                table: "Department",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_Department_IsDeleted",
                schema: "lkp",
                table: "Department",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Degree_CreatedDate",
                schema: "lkp",
                table: "Degree",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_Degree_IsDeleted",
                schema: "lkp",
                table: "Degree",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Country_CreatedDate",
                schema: "lkp",
                table: "Country",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_Country_IsDeleted",
                schema: "lkp",
                table: "Country",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_ContactVerification_CreatedDate",
                table: "ContactVerification",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_ContactVerification_IsDeleted",
                table: "ContactVerification",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_City_CreatedDate",
                schema: "lkp",
                table: "City",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_City_IsDeleted",
                schema: "lkp",
                table: "City",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_CandidateType_CreatedDate",
                schema: "lkp",
                table: "CandidateType",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_CandidateType_IsDeleted",
                schema: "lkp",
                table: "CandidateType",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_AuditTrailEntry_CreatedDate",
                schema: "hr",
                table: "AuditTrailEntry",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_AuditTrailEntry_IsDeleted",
                schema: "hr",
                table: "AuditTrailEntry",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_IsDeleted",
                table: "AspNetUsers",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_ActionLog_CreatedDate",
                schema: "hr",
                table: "ActionLog",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_ActionLog_IsDeleted",
                schema: "hr",
                table: "ActionLog",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_AchievementType_CreatedDate",
                schema: "lkp",
                table: "AchievementType",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_AchievementType_IsDeleted",
                schema: "lkp",
                table: "AchievementType",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Achievement_CreatedDate",
                schema: "pro",
                table: "Achievement",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_Achievement_IsDeleted",
                schema: "pro",
                table: "Achievement",
                column: "IsDeleted");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WorkType_CreatedDate",
                schema: "lkp",
                table: "WorkType");

            migrationBuilder.DropIndex(
                name: "IX_WorkType_IsDeleted",
                schema: "lkp",
                table: "WorkType");

            migrationBuilder.DropIndex(
                name: "IX_UserType_CreatedDate",
                schema: "lkp",
                table: "UserType");

            migrationBuilder.DropIndex(
                name: "IX_UserType_IsDeleted",
                schema: "lkp",
                table: "UserType");

            migrationBuilder.DropIndex(
                name: "IX_UserSession_CreatedDate",
                table: "UserSession");

            migrationBuilder.DropIndex(
                name: "IX_UserSession_IsDeleted",
                table: "UserSession");

            migrationBuilder.DropIndex(
                name: "IX_UserProfileLogger_CreatedDate",
                schema: "hr",
                table: "UserProfileLogger");

            migrationBuilder.DropIndex(
                name: "IX_UserProfileLogger_IsDeleted",
                schema: "hr",
                table: "UserProfileLogger");

            migrationBuilder.DropIndex(
                name: "IX_UserProfile_CreatedDate",
                schema: "app",
                table: "UserProfile");

            migrationBuilder.DropIndex(
                name: "IX_UserProfile_IsDeleted",
                schema: "app",
                table: "UserProfile");

            migrationBuilder.DropIndex(
                name: "IX_University_CreatedDate",
                schema: "lkp",
                table: "University");

            migrationBuilder.DropIndex(
                name: "IX_University_IsDeleted",
                schema: "lkp",
                table: "University");

            migrationBuilder.DropIndex(
                name: "IX_TrainingCourse_CreatedDate",
                schema: "pro",
                table: "TrainingCourse");

            migrationBuilder.DropIndex(
                name: "IX_TrainingCourse_IsDeleted",
                schema: "pro",
                table: "TrainingCourse");

            migrationBuilder.DropIndex(
                name: "IX_TargetEntity_CreatedDate",
                schema: "lkp",
                table: "TargetEntity");

            migrationBuilder.DropIndex(
                name: "IX_TargetEntity_IsDeleted",
                schema: "lkp",
                table: "TargetEntity");

            migrationBuilder.DropIndex(
                name: "IX_StudyType_CreatedDate",
                schema: "lkp",
                table: "StudyType");

            migrationBuilder.DropIndex(
                name: "IX_StudyType_IsDeleted",
                schema: "lkp",
                table: "StudyType");

            migrationBuilder.DropIndex(
                name: "IX_SponsorType_CreatedDate",
                schema: "lkp",
                table: "SponsorType");

            migrationBuilder.DropIndex(
                name: "IX_SponsorType_IsDeleted",
                schema: "lkp",
                table: "SponsorType");

            migrationBuilder.DropIndex(
                name: "IX_SponsorProfile_CreatedDate",
                schema: "app",
                table: "SponsorProfile");

            migrationBuilder.DropIndex(
                name: "IX_SponsorProfile_IsDeleted",
                schema: "app",
                table: "SponsorProfile");

            migrationBuilder.DropIndex(
                name: "IX_SkillType_CreatedDate",
                schema: "lkp",
                table: "SkillType");

            migrationBuilder.DropIndex(
                name: "IX_SkillType_IsDeleted",
                schema: "lkp",
                table: "SkillType");

            migrationBuilder.DropIndex(
                name: "IX_SkillLevel_CreatedDate",
                schema: "lkp",
                table: "SkillLevel");

            migrationBuilder.DropIndex(
                name: "IX_SkillLevel_IsDeleted",
                schema: "lkp",
                table: "SkillLevel");

            migrationBuilder.DropIndex(
                name: "IX_Skill_CreatedDate",
                schema: "lkp",
                table: "Skill");

            migrationBuilder.DropIndex(
                name: "IX_Skill_IsDeleted",
                schema: "lkp",
                table: "Skill");

            migrationBuilder.DropIndex(
                name: "IX_Sector_CreatedDate",
                schema: "lkp",
                table: "Sector");

            migrationBuilder.DropIndex(
                name: "IX_Sector_IsDeleted",
                schema: "lkp",
                table: "Sector");

            migrationBuilder.DropIndex(
                name: "IX_ReviewItem_CreatedDate",
                schema: "hr",
                table: "ReviewItem");

            migrationBuilder.DropIndex(
                name: "IX_ReviewItem_IsDeleted",
                schema: "hr",
                table: "ReviewItem");

            migrationBuilder.DropIndex(
                name: "IX_Resources_CreatedDate",
                table: "Resources");

            migrationBuilder.DropIndex(
                name: "IX_Resources_IsDeleted",
                table: "Resources");

            migrationBuilder.DropIndex(
                name: "IX_ResidenceAddress_CreatedDate",
                schema: "pro",
                table: "ResidenceAddress");

            migrationBuilder.DropIndex(
                name: "IX_ResidenceAddress_IsDeleted",
                schema: "pro",
                table: "ResidenceAddress");

            migrationBuilder.DropIndex(
                name: "IX_Religion_CreatedDate",
                schema: "lkp",
                table: "Religion");

            migrationBuilder.DropIndex(
                name: "IX_Religion_IsDeleted",
                schema: "lkp",
                table: "Religion");

            migrationBuilder.DropIndex(
                name: "IX_RefreshToken_CreatedDate",
                table: "RefreshToken");

            migrationBuilder.DropIndex(
                name: "IX_RefreshToken_IsDeleted",
                table: "RefreshToken");

            migrationBuilder.DropIndex(
                name: "IX_RatingGrade_CreatedDate",
                schema: "lkp",
                table: "RatingGrade");

            migrationBuilder.DropIndex(
                name: "IX_RatingGrade_IsDeleted",
                schema: "lkp",
                table: "RatingGrade");

            migrationBuilder.DropIndex(
                name: "IX_Qualification_CreatedDate",
                schema: "pro",
                table: "Qualification");

            migrationBuilder.DropIndex(
                name: "IX_Qualification_IsDeleted",
                schema: "pro",
                table: "Qualification");

            migrationBuilder.DropIndex(
                name: "IX_ProviderLogin_CreatedDate",
                schema: "lkp",
                table: "ProviderLogin");

            migrationBuilder.DropIndex(
                name: "IX_ProviderLogin_IsDeleted",
                schema: "lkp",
                table: "ProviderLogin");

            migrationBuilder.DropIndex(
                name: "IX_ProfileSkill_CreatedDate",
                schema: "pro",
                table: "ProfileSkill");

            migrationBuilder.DropIndex(
                name: "IX_ProfileSkill_IsDeleted",
                schema: "pro",
                table: "ProfileSkill");

            migrationBuilder.DropIndex(
                name: "IX_ProfileReviewDecision_CreatedDate",
                schema: "hr",
                table: "ProfileReviewDecision");

            migrationBuilder.DropIndex(
                name: "IX_ProfileReviewDecision_IsDeleted",
                schema: "hr",
                table: "ProfileReviewDecision");

            migrationBuilder.DropIndex(
                name: "IX_ProfileLanguage_CreatedDate",
                schema: "pro",
                table: "ProfileLanguage");

            migrationBuilder.DropIndex(
                name: "IX_ProfileLanguage_IsDeleted",
                schema: "pro",
                table: "ProfileLanguage");

            migrationBuilder.DropIndex(
                name: "IX_ProfileChangeRequest_CreatedDate",
                schema: "hr",
                table: "ProfileChangeRequest");

            migrationBuilder.DropIndex(
                name: "IX_ProfileChangeRequest_IsDeleted",
                schema: "hr",
                table: "ProfileChangeRequest");

            migrationBuilder.DropIndex(
                name: "IX_ProfileAssignment_CreatedDate",
                schema: "hr",
                table: "ProfileAssignment");

            migrationBuilder.DropIndex(
                name: "IX_ProfileAssignment_IsDeleted",
                schema: "hr",
                table: "ProfileAssignment");

            migrationBuilder.DropIndex(
                name: "IX_ProfileAdditionalAttachment_CreatedDate",
                schema: "pro",
                table: "ProfileAdditionalAttachment");

            migrationBuilder.DropIndex(
                name: "IX_ProfileAdditionalAttachment_IsDeleted",
                schema: "pro",
                table: "ProfileAdditionalAttachment");

            migrationBuilder.DropIndex(
                name: "IX_Permission_CreatedDate",
                schema: "lkp",
                table: "Permission");

            migrationBuilder.DropIndex(
                name: "IX_Permission_IsDeleted",
                schema: "lkp",
                table: "Permission");

            migrationBuilder.DropIndex(
                name: "IX_OfficeSupportedCountry_CreatedDate",
                schema: "lkp",
                table: "OfficeSupportedCountry");

            migrationBuilder.DropIndex(
                name: "IX_OfficeSupportedCountry_IsDeleted",
                schema: "lkp",
                table: "OfficeSupportedCountry");

            migrationBuilder.DropIndex(
                name: "IX_Office_CreatedDate",
                schema: "lkp",
                table: "Office");

            migrationBuilder.DropIndex(
                name: "IX_Office_IsDeleted",
                schema: "lkp",
                table: "Office");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_CreatedDate",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_IsDeleted",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_MaritalStatus_CreatedDate",
                schema: "lkp",
                table: "MaritalStatus");

            migrationBuilder.DropIndex(
                name: "IX_MaritalStatus_IsDeleted",
                schema: "lkp",
                table: "MaritalStatus");

            migrationBuilder.DropIndex(
                name: "IX_Management_CreatedDate",
                schema: "lkp",
                table: "Management");

            migrationBuilder.DropIndex(
                name: "IX_Management_IsDeleted",
                schema: "lkp",
                table: "Management");

            migrationBuilder.DropIndex(
                name: "IX_MajorSkill_CreatedDate",
                schema: "hr",
                table: "MajorSkill");

            migrationBuilder.DropIndex(
                name: "IX_MajorSkill_IsDeleted",
                schema: "hr",
                table: "MajorSkill");

            migrationBuilder.DropIndex(
                name: "IX_Major_CreatedDate",
                schema: "lkp",
                table: "Major");

            migrationBuilder.DropIndex(
                name: "IX_Major_IsDeleted",
                schema: "lkp",
                table: "Major");

            migrationBuilder.DropIndex(
                name: "IX_LoginAttempt_CreatedDate",
                table: "LoginAttempt");

            migrationBuilder.DropIndex(
                name: "IX_LoginAttempt_IsDeleted",
                table: "LoginAttempt");

            migrationBuilder.DropIndex(
                name: "IX_LanguageLevel_CreatedDate",
                schema: "lkp",
                table: "LanguageLevel");

            migrationBuilder.DropIndex(
                name: "IX_LanguageLevel_IsDeleted",
                schema: "lkp",
                table: "LanguageLevel");

            migrationBuilder.DropIndex(
                name: "IX_Language_CreatedDate",
                schema: "lkp",
                table: "Language");

            migrationBuilder.DropIndex(
                name: "IX_Language_IsDeleted",
                schema: "lkp",
                table: "Language");

            migrationBuilder.DropIndex(
                name: "IX_KawaderQids_CreatedDate",
                table: "KawaderQids");

            migrationBuilder.DropIndex(
                name: "IX_KawaderQids_IsDeleted",
                table: "KawaderQids");

            migrationBuilder.DropIndex(
                name: "IX_JobTitle_CreatedDate",
                schema: "lkp",
                table: "JobTitle");

            migrationBuilder.DropIndex(
                name: "IX_JobTitle_IsDeleted",
                schema: "lkp",
                table: "JobTitle");

            migrationBuilder.DropIndex(
                name: "IX_JobTabReviewNote_CreatedDate",
                schema: "hr",
                table: "JobTabReviewNote");

            migrationBuilder.DropIndex(
                name: "IX_JobTabReviewNote_IsDeleted",
                schema: "hr",
                table: "JobTabReviewNote");

            migrationBuilder.DropIndex(
                name: "IX_JobStatus_CreatedDate",
                schema: "lkp",
                table: "JobStatus");

            migrationBuilder.DropIndex(
                name: "IX_JobStatus_IsDeleted",
                schema: "lkp",
                table: "JobStatus");

            migrationBuilder.DropIndex(
                name: "IX_JobSkill_CreatedDate",
                schema: "hr",
                table: "JobSkill");

            migrationBuilder.DropIndex(
                name: "IX_JobSkill_IsDeleted",
                schema: "hr",
                table: "JobSkill");

            migrationBuilder.DropIndex(
                name: "IX_JobReviewAttachment_CreatedDate",
                schema: "hr",
                table: "JobReviewAttachment");

            migrationBuilder.DropIndex(
                name: "IX_JobReviewAttachment_IsDeleted",
                schema: "hr",
                table: "JobReviewAttachment");

            migrationBuilder.DropIndex(
                name: "IX_JobResponsibility_CreatedDate",
                schema: "hr",
                table: "JobResponsibility");

            migrationBuilder.DropIndex(
                name: "IX_JobResponsibility_IsDeleted",
                schema: "hr",
                table: "JobResponsibility");

            migrationBuilder.DropIndex(
                name: "IX_JobRequiredAttachment_CreatedDate",
                schema: "hr",
                table: "JobRequiredAttachment");

            migrationBuilder.DropIndex(
                name: "IX_JobRequiredAttachment_IsDeleted",
                schema: "hr",
                table: "JobRequiredAttachment");

            migrationBuilder.DropIndex(
                name: "IX_JobPointsMain_CreatedDate",
                schema: "hr",
                table: "JobPointsMain");

            migrationBuilder.DropIndex(
                name: "IX_JobPointsMain_IsDeleted",
                schema: "hr",
                table: "JobPointsMain");

            migrationBuilder.DropIndex(
                name: "IX_JobPointsDetail_CreatedDate",
                schema: "hr",
                table: "JobPointsDetail");

            migrationBuilder.DropIndex(
                name: "IX_JobPointsDetail_IsDeleted",
                schema: "hr",
                table: "JobPointsDetail");

            migrationBuilder.DropIndex(
                name: "IX_JobPointConfiguration_CreatedDate",
                schema: "hr",
                table: "JobPointConfiguration");

            migrationBuilder.DropIndex(
                name: "IX_JobPointConfiguration_IsDeleted",
                schema: "hr",
                table: "JobPointConfiguration");

            migrationBuilder.DropIndex(
                name: "IX_JobDegree_CreatedDate",
                schema: "hr",
                table: "JobDegree");

            migrationBuilder.DropIndex(
                name: "IX_JobDegree_IsDeleted",
                schema: "hr",
                table: "JobDegree");

            migrationBuilder.DropIndex(
                name: "IX_JobCondition_CreatedDate",
                schema: "hr",
                table: "JobCondition");

            migrationBuilder.DropIndex(
                name: "IX_JobCondition_IsDeleted",
                schema: "hr",
                table: "JobCondition");

            migrationBuilder.DropIndex(
                name: "IX_JobCategoryCandidateSettings_CreatedDate",
                schema: "hr",
                table: "JobCategoryCandidateSettings");

            migrationBuilder.DropIndex(
                name: "IX_JobCategoryCandidateSettings_IsDeleted",
                schema: "hr",
                table: "JobCategoryCandidateSettings");

            migrationBuilder.DropIndex(
                name: "IX_JobCategory_CreatedDate",
                schema: "lkp",
                table: "JobCategory");

            migrationBuilder.DropIndex(
                name: "IX_JobCategory_IsDeleted",
                schema: "lkp",
                table: "JobCategory");

            migrationBuilder.DropIndex(
                name: "IX_JobCandidateTypePercentage_CreatedDate",
                schema: "hr",
                table: "JobCandidateTypePercentage");

            migrationBuilder.DropIndex(
                name: "IX_JobCandidateTypePercentage_IsDeleted",
                schema: "hr",
                table: "JobCandidateTypePercentage");

            migrationBuilder.DropIndex(
                name: "IX_JobCandidateNationalityPercentage_CreatedDate",
                schema: "hr",
                table: "JobCandidateNationalityPercentage");

            migrationBuilder.DropIndex(
                name: "IX_JobCandidateNationalityPercentage_IsDeleted",
                schema: "hr",
                table: "JobCandidateNationalityPercentage");

            migrationBuilder.DropIndex(
                name: "IX_JobCandidateFilterSetting_CreatedDate",
                schema: "hr",
                table: "JobCandidateFilterSetting");

            migrationBuilder.DropIndex(
                name: "IX_JobCandidateFilterSetting_IsDeleted",
                schema: "hr",
                table: "JobCandidateFilterSetting");

            migrationBuilder.DropIndex(
                name: "IX_Job_CreatedDate",
                schema: "hr",
                table: "Job");

            migrationBuilder.DropIndex(
                name: "IX_Job_IsDeleted",
                schema: "hr",
                table: "Job");

            migrationBuilder.DropIndex(
                name: "IX_InvitationStatus_CreatedDate",
                schema: "lkp",
                table: "InvitationStatus");

            migrationBuilder.DropIndex(
                name: "IX_InvitationStatus_IsDeleted",
                schema: "lkp",
                table: "InvitationStatus");

            migrationBuilder.DropIndex(
                name: "IX_Invitation_CreatedDate",
                schema: "hr",
                table: "Invitation");

            migrationBuilder.DropIndex(
                name: "IX_Invitation_IsDeleted",
                schema: "hr",
                table: "Invitation");

            migrationBuilder.DropIndex(
                name: "IX_HomeSuccessStory_CreatedDate",
                schema: "app",
                table: "HomeSuccessStory");

            migrationBuilder.DropIndex(
                name: "IX_HomeSuccessStory_IsDeleted",
                schema: "app",
                table: "HomeSuccessStory");

            migrationBuilder.DropIndex(
                name: "IX_HistoryInvitation_CreatedDate",
                table: "HistoryInvitation");

            migrationBuilder.DropIndex(
                name: "IX_HistoryInvitation_IsDeleted",
                table: "HistoryInvitation");

            migrationBuilder.DropIndex(
                name: "IX_Gender_CreatedDate",
                schema: "lkp",
                table: "Gender");

            migrationBuilder.DropIndex(
                name: "IX_Gender_IsDeleted",
                schema: "lkp",
                table: "Gender");

            migrationBuilder.DropIndex(
                name: "IX_FAQ_CreatedDate",
                schema: "app",
                table: "FAQ");

            migrationBuilder.DropIndex(
                name: "IX_FAQ_IsDeleted",
                schema: "app",
                table: "FAQ");

            migrationBuilder.DropIndex(
                name: "IX_Experience_CreatedDate",
                schema: "pro",
                table: "Experience");

            migrationBuilder.DropIndex(
                name: "IX_Experience_IsDeleted",
                schema: "pro",
                table: "Experience");

            migrationBuilder.DropIndex(
                name: "IX_EntityLog_CreatedDate",
                table: "EntityLog");

            migrationBuilder.DropIndex(
                name: "IX_EntityLog_IsDeleted",
                table: "EntityLog");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeProfile_CreatedDate",
                table: "EmployeeProfile");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeProfile_IsDeleted",
                table: "EmployeeProfile");

            migrationBuilder.DropIndex(
                name: "IX_EmailTemplates_CreatedDate",
                table: "EmailTemplates");

            migrationBuilder.DropIndex(
                name: "IX_EmailTemplates_IsDeleted",
                table: "EmailTemplates");

            migrationBuilder.DropIndex(
                name: "IX_EmailQueues_CreatedDate",
                table: "EmailQueues");

            migrationBuilder.DropIndex(
                name: "IX_EmailQueues_IsDeleted",
                table: "EmailQueues");

            migrationBuilder.DropIndex(
                name: "IX_Department_CreatedDate",
                schema: "lkp",
                table: "Department");

            migrationBuilder.DropIndex(
                name: "IX_Department_IsDeleted",
                schema: "lkp",
                table: "Department");

            migrationBuilder.DropIndex(
                name: "IX_Degree_CreatedDate",
                schema: "lkp",
                table: "Degree");

            migrationBuilder.DropIndex(
                name: "IX_Degree_IsDeleted",
                schema: "lkp",
                table: "Degree");

            migrationBuilder.DropIndex(
                name: "IX_Country_CreatedDate",
                schema: "lkp",
                table: "Country");

            migrationBuilder.DropIndex(
                name: "IX_Country_IsDeleted",
                schema: "lkp",
                table: "Country");

            migrationBuilder.DropIndex(
                name: "IX_ContactVerification_CreatedDate",
                table: "ContactVerification");

            migrationBuilder.DropIndex(
                name: "IX_ContactVerification_IsDeleted",
                table: "ContactVerification");

            migrationBuilder.DropIndex(
                name: "IX_City_CreatedDate",
                schema: "lkp",
                table: "City");

            migrationBuilder.DropIndex(
                name: "IX_City_IsDeleted",
                schema: "lkp",
                table: "City");

            migrationBuilder.DropIndex(
                name: "IX_CandidateType_CreatedDate",
                schema: "lkp",
                table: "CandidateType");

            migrationBuilder.DropIndex(
                name: "IX_CandidateType_IsDeleted",
                schema: "lkp",
                table: "CandidateType");

            migrationBuilder.DropIndex(
                name: "IX_AuditTrailEntry_CreatedDate",
                schema: "hr",
                table: "AuditTrailEntry");

            migrationBuilder.DropIndex(
                name: "IX_AuditTrailEntry_IsDeleted",
                schema: "hr",
                table: "AuditTrailEntry");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_IsDeleted",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_ActionLog_CreatedDate",
                schema: "hr",
                table: "ActionLog");

            migrationBuilder.DropIndex(
                name: "IX_ActionLog_IsDeleted",
                schema: "hr",
                table: "ActionLog");

            migrationBuilder.DropIndex(
                name: "IX_AchievementType_CreatedDate",
                schema: "lkp",
                table: "AchievementType");

            migrationBuilder.DropIndex(
                name: "IX_AchievementType_IsDeleted",
                schema: "lkp",
                table: "AchievementType");

            migrationBuilder.DropIndex(
                name: "IX_Achievement_CreatedDate",
                schema: "pro",
                table: "Achievement");

            migrationBuilder.DropIndex(
                name: "IX_Achievement_IsDeleted",
                schema: "pro",
                table: "Achievement");
        }
    }
}
