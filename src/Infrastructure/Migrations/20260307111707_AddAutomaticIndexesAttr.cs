using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAutomaticIndexesAttr : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_UserProfileLogger_AttachmentId",
                schema: "hr",
                table: "UserProfileLogger",
                column: "AttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfileLogger_EntityId",
                schema: "hr",
                table: "UserProfileLogger",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfile_NationalNumber",
                schema: "app",
                table: "UserProfile",
                column: "NationalNumber");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfile_Provider",
                schema: "app",
                table: "UserProfile",
                column: "Provider");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfile_Status",
                schema: "app",
                table: "UserProfile",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingCourse_Provider",
                schema: "pro",
                table: "TrainingCourse",
                column: "Provider");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewItem_EntityId",
                schema: "hr",
                table: "ReviewItem",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewItem_ResourceId",
                schema: "hr",
                table: "ReviewItem",
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewItem_ReviewedById",
                schema: "hr",
                table: "ReviewItem",
                column: "ReviewedById");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewItem_Status",
                schema: "hr",
                table: "ReviewItem",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshToken_UserId",
                table: "RefreshToken",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileChangeRequest_CanceledById",
                schema: "hr",
                table: "ProfileChangeRequest",
                column: "CanceledById");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileChangeRequest_NewResourceId",
                schema: "hr",
                table: "ProfileChangeRequest",
                column: "NewResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileChangeRequest_OldResourceId",
                schema: "hr",
                table: "ProfileChangeRequest",
                column: "OldResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileChangeRequest_RequestedById",
                schema: "hr",
                table: "ProfileChangeRequest",
                column: "RequestedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileChangeRequest_ReviewedById",
                schema: "hr",
                table: "ProfileChangeRequest",
                column: "ReviewedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileChangeRequest_Status",
                schema: "hr",
                table: "ProfileChangeRequest",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileChangeRequest_UserProfileId",
                schema: "hr",
                table: "ProfileChangeRequest",
                column: "UserProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_Status",
                table: "Notifications",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_JobTabReviewNote_ReviewCycleId",
                schema: "hr",
                table: "JobTabReviewNote",
                column: "ReviewCycleId");

            migrationBuilder.CreateIndex(
                name: "IX_JobReviewAttachment_ReviewCycleId",
                schema: "hr",
                table: "JobReviewAttachment",
                column: "ReviewCycleId");

            migrationBuilder.CreateIndex(
                name: "IX_JobPointsDetail_ReferenceId",
                schema: "hr",
                table: "JobPointsDetail",
                column: "ReferenceId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityLog_EntityId",
                table: "EntityLog",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_CandidateTypeProviderLogin_CandidateTypeId",
                schema: "lkp",
                table: "CandidateTypeProviderLogin",
                column: "CandidateTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditTrailEntry_AttachmentId",
                schema: "hr",
                table: "AuditTrailEntry",
                column: "AttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditTrailEntry_EntityId",
                schema: "hr",
                table: "AuditTrailEntry",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditTrailEntry_UserId",
                schema: "hr",
                table: "AuditTrailEntry",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditTrailEntry_UserProfileId",
                schema: "hr",
                table: "AuditTrailEntry",
                column: "UserProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_CreatedById",
                table: "AspNetUsers",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_DeletedById",
                table: "AspNetUsers",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_UpdatedById",
                table: "AspNetUsers",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ActionLog_AttachmentId",
                schema: "hr",
                table: "ActionLog",
                column: "AttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_ActionLog_EntityId",
                schema: "hr",
                table: "ActionLog",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_ActionLog_UserId",
                schema: "hr",
                table: "ActionLog",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ActionLog_UserProfileId",
                schema: "hr",
                table: "ActionLog",
                column: "UserProfileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserProfileLogger_AttachmentId",
                schema: "hr",
                table: "UserProfileLogger");

            migrationBuilder.DropIndex(
                name: "IX_UserProfileLogger_EntityId",
                schema: "hr",
                table: "UserProfileLogger");

            migrationBuilder.DropIndex(
                name: "IX_UserProfile_NationalNumber",
                schema: "app",
                table: "UserProfile");

            migrationBuilder.DropIndex(
                name: "IX_UserProfile_Provider",
                schema: "app",
                table: "UserProfile");

            migrationBuilder.DropIndex(
                name: "IX_UserProfile_Status",
                schema: "app",
                table: "UserProfile");

            migrationBuilder.DropIndex(
                name: "IX_TrainingCourse_Provider",
                schema: "pro",
                table: "TrainingCourse");

            migrationBuilder.DropIndex(
                name: "IX_ReviewItem_EntityId",
                schema: "hr",
                table: "ReviewItem");

            migrationBuilder.DropIndex(
                name: "IX_ReviewItem_ResourceId",
                schema: "hr",
                table: "ReviewItem");

            migrationBuilder.DropIndex(
                name: "IX_ReviewItem_ReviewedById",
                schema: "hr",
                table: "ReviewItem");

            migrationBuilder.DropIndex(
                name: "IX_ReviewItem_Status",
                schema: "hr",
                table: "ReviewItem");

            migrationBuilder.DropIndex(
                name: "IX_RefreshToken_UserId",
                table: "RefreshToken");

            migrationBuilder.DropIndex(
                name: "IX_ProfileChangeRequest_CanceledById",
                schema: "hr",
                table: "ProfileChangeRequest");

            migrationBuilder.DropIndex(
                name: "IX_ProfileChangeRequest_NewResourceId",
                schema: "hr",
                table: "ProfileChangeRequest");

            migrationBuilder.DropIndex(
                name: "IX_ProfileChangeRequest_OldResourceId",
                schema: "hr",
                table: "ProfileChangeRequest");

            migrationBuilder.DropIndex(
                name: "IX_ProfileChangeRequest_RequestedById",
                schema: "hr",
                table: "ProfileChangeRequest");

            migrationBuilder.DropIndex(
                name: "IX_ProfileChangeRequest_ReviewedById",
                schema: "hr",
                table: "ProfileChangeRequest");

            migrationBuilder.DropIndex(
                name: "IX_ProfileChangeRequest_Status",
                schema: "hr",
                table: "ProfileChangeRequest");

            migrationBuilder.DropIndex(
                name: "IX_ProfileChangeRequest_UserProfileId",
                schema: "hr",
                table: "ProfileChangeRequest");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_Status",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_JobTabReviewNote_ReviewCycleId",
                schema: "hr",
                table: "JobTabReviewNote");

            migrationBuilder.DropIndex(
                name: "IX_JobReviewAttachment_ReviewCycleId",
                schema: "hr",
                table: "JobReviewAttachment");

            migrationBuilder.DropIndex(
                name: "IX_JobPointsDetail_ReferenceId",
                schema: "hr",
                table: "JobPointsDetail");

            migrationBuilder.DropIndex(
                name: "IX_EntityLog_EntityId",
                table: "EntityLog");

            migrationBuilder.DropIndex(
                name: "IX_CandidateTypeProviderLogin_CandidateTypeId",
                schema: "lkp",
                table: "CandidateTypeProviderLogin");

            migrationBuilder.DropIndex(
                name: "IX_AuditTrailEntry_AttachmentId",
                schema: "hr",
                table: "AuditTrailEntry");

            migrationBuilder.DropIndex(
                name: "IX_AuditTrailEntry_EntityId",
                schema: "hr",
                table: "AuditTrailEntry");

            migrationBuilder.DropIndex(
                name: "IX_AuditTrailEntry_UserId",
                schema: "hr",
                table: "AuditTrailEntry");

            migrationBuilder.DropIndex(
                name: "IX_AuditTrailEntry_UserProfileId",
                schema: "hr",
                table: "AuditTrailEntry");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_CreatedById",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_DeletedById",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_UpdatedById",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_ActionLog_AttachmentId",
                schema: "hr",
                table: "ActionLog");

            migrationBuilder.DropIndex(
                name: "IX_ActionLog_EntityId",
                schema: "hr",
                table: "ActionLog");

            migrationBuilder.DropIndex(
                name: "IX_ActionLog_UserId",
                schema: "hr",
                table: "ActionLog");

            migrationBuilder.DropIndex(
                name: "IX_ActionLog_UserProfileId",
                schema: "hr",
                table: "ActionLog");
        }
    }
}
