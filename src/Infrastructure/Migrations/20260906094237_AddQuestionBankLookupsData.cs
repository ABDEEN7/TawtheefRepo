using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddQuestionBankLookupsData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Stage_BackendName",
                schema: "lkp",
                table: "Stage");

            migrationBuilder.DropIndex(
                name: "IX_QuestionType_BackendName",
                schema: "lkp",
                table: "QuestionType");

            migrationBuilder.DropIndex(
                name: "IX_QuestionReviewDecision_BackendName",
                schema: "lkp",
                table: "QuestionReviewDecision");

            migrationBuilder.DropIndex(
                name: "IX_QuestionChangeType_BackendName",
                schema: "lkp",
                table: "QuestionChangeType");

            migrationBuilder.DropIndex(
                name: "IX_QuestionBankType_BackendName",
                schema: "lkp",
                table: "QuestionBankType");

            migrationBuilder.DropIndex(
                name: "IX_QuestionBankRequestType_BackendName",
                schema: "lkp",
                table: "QuestionBankRequestType");

            migrationBuilder.DropIndex(
                name: "IX_QuestionBankRequestStatus_BackendName",
                schema: "lkp",
                table: "QuestionBankRequestStatus");

            migrationBuilder.DropIndex(
                name: "IX_QuestionBankRequestItemStatus_BackendName",
                schema: "lkp",
                table: "QuestionBankRequestItemStatus");

            migrationBuilder.DropIndex(
                name: "IX_DifficultyLevel_BackendName",
                schema: "lkp",
                table: "DifficultyLevel");

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "DifficultyLevel",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("580deb45-9304-4fcb-905e-868f5db3848b"), "MEDIUM", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 2, true, false, "متوسط", "Medium", null, null },
                    { new Guid("59e7808a-916b-44c4-92a9-41dd1c6e24e4"), "EASY", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 1, true, false, "سهل", "Easy", null, null },
                    { new Guid("9bbee09c-e2a3-463b-9901-4b646200074d"), "HARD", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 3, true, false, "صعب", "Hard", null, null }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "QuestionBankAssignmentStatus",
                columns: new[] { "Id", "Code", "DisplayOrder", "IsActive", "NameAr", "NameEn" },
                values: new object[,]
                {
                    { new Guid("238e3c8d-84de-4d70-8a1f-192ac93224ba"), "RETURNED_FOR_MODIFICATION", 4, true, "معاد للتعديل", "Returned for Modification" },
                    { new Guid("8b73c382-c04b-49f7-bb78-4bc235df872f"), "QUESTION_ENTRY_IN_PROGRESS", 2, true, "قيد إدخال الأسئلة", "Question Entry In Progress" },
                    { new Guid("9b38c5a0-fb16-44f2-8239-cb5ee704129d"), "ASSIGNED", 1, true, "تم الإسناد", "Assigned" },
                    { new Guid("a4851953-f48a-4aa9-afb9-b51dfbc1e4d8"), "CANCELLED", 7, true, "ملغي", "Cancelled" },
                    { new Guid("a8ca906a-e1a3-44c9-87c1-8b4ec6192ab0"), "COMPLETED", 6, true, "مكتمل", "Completed" },
                    { new Guid("c035e63f-d866-488f-affd-9ae08a7043dd"), "MODIFICATION_COMPLETED", 5, true, "تم إنهاء التعديلات", "Modification Completed" },
                    { new Guid("c6cddace-de75-4a6e-8d1f-65e7e93ba4ed"), "QUESTION_ENTRY_COMPLETED", 3, true, "تم إنهاء إدخال الأسئلة", "Question Entry Completed" }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "QuestionBankRequestItemStatus",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("1914dd04-27d9-468d-b03e-813df6ce87ff"), "APPROVED", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 3, true, false, "معتمد", "Approved", null, null },
                    { new Guid("19760735-e5f0-4236-9fff-8487f9f057ef"), "DRAFT", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 1, true, false, "مسودة", "Draft", null, null },
                    { new Guid("217164f8-c23d-4913-a549-16f80186f9cb"), "REJECTED", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 5, true, false, "مرفوض", "Rejected", null, null },
                    { new Guid("bfa1d9c1-a284-452b-8b59-a2d5fc0a6dfe"), "NEEDS_MODIFICATION", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 4, true, false, "يحتاج إلى تعديل", "Needs Modification", null, null },
                    { new Guid("c5ed702c-cfca-4d7e-8bf9-98cdf15f2106"), "PENDING_REVIEW", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 2, true, false, "بانتظار المراجعة", "Pending Review", null, null },
                    { new Guid("f48eedf5-4fe9-4ded-931b-c76bd5c9ba80"), "REMOVED_FROM_REQUEST", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 6, true, false, "تمت إزالته من الطلب", "Removed from Request", null, null }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "QuestionBankRequestStatus",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("4b25e8db-92f1-4b4c-baab-756a5d1fd3f8"), "PENDING_ASSIGNMENT", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 1, true, false, "بانتظار الإسناد", "Pending Assignment", null, null },
                    { new Guid("66fbfd41-563e-4a12-a006-8a970285f62b"), "QUESTION_ENTRY_IN_PROGRESS", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 2, true, false, "قيد إدخال الأسئلة", "Question Entry In Progress", null, null },
                    { new Guid("77687c70-161f-4a6c-977e-95f86b37d724"), "CANCELLED", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 6, true, false, "ملغي", "Cancelled", null, null },
                    { new Guid("8b6d8c90-2e40-4957-ac52-bd46b0ae9726"), "ISSUED", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 5, true, false, "تم الإصدار", "Issued", null, null },
                    { new Guid("97c0610c-a992-43e1-b9a1-bfdbcf5b34b0"), "PENDING_REVIEW", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 3, true, false, "بانتظار المراجعة", "Pending Review", null, null },
                    { new Guid("f5cbde41-7e46-4793-953f-ebe5f8e31570"), "MODIFICATION_IN_PROGRESS", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 4, true, false, "قيد التعديل", "Modification In Progress", null, null },
                    { new Guid("fa2e079c-9361-4e5a-9cbd-39dcb73a1520"), "REJECTED", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 7, true, false, "مرفوض", "Rejected", null, null }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "QuestionBankRequestType",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("4fa8e626-6395-4987-b949-73cb070148d4"), "MAINTENANCE", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 2, true, false, "صيانة", "Maintenance", null, null },
                    { new Guid("59659e5d-cf4c-46c5-bea1-3198a0f5a58c"), "CREATE", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 1, true, false, "إنشاء", "Create", null, null }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "QuestionBankType",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("0ef8f0d9-02d7-4863-9bfe-e63355f3686e"), "EDUCATIONAL", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 3, true, false, "تربوي", "Educational", null, null },
                    { new Guid("686928db-b630-4689-b265-ae17eded73cf"), "SKILLS", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 2, true, false, "مهارات", "Skills", null, null },
                    { new Guid("a7f9646b-fcc3-4a00-944c-ceed82acd557"), "SPECIALIZED", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 1, true, false, "تخصصي", "Specialized", null, null }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "QuestionChangeType",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("dd984c21-7b25-444b-85a9-78767e337ddc"), "ADD", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 1, true, false, "إضافة", "Add", null, null },
                    { new Guid("f02136e2-ac6c-4d3e-9c37-efd28587177d"), "DELETE", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 3, true, false, "حذف", "Delete", null, null },
                    { new Guid("f7abde66-546b-4055-bfed-215c47bea845"), "UPDATE", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 2, true, false, "تعديل", "Update", null, null }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "QuestionReviewDecision",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("73df4c1d-05a0-4bb4-9f19-5889a791aa27"), "APPROVED", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 1, true, false, "معتمد", "Approved", null, null },
                    { new Guid("a0ce4f21-955b-4290-bf88-fc1775a21410"), "REJECTED", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 3, true, false, "مرفوض", "Rejected", null, null },
                    { new Guid("a29e6157-401f-49ac-ba46-98ecd8ca73dd"), "NEEDS_MODIFICATION", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 2, true, false, "يحتاج إلى تعديل", "Needs Modification", null, null }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "QuestionType",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("04a6e083-d38f-4838-a820-402fa4c53a39"), "MULTIPLE_CHOICE", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 1, true, false, "اختيار من متعدد", "Multiple Choice", null, null },
                    { new Guid("62d2e3f9-562d-45f1-ace4-867b929cae43"), "TRUE_FALSE", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 2, true, false, "صح / خطأ", "True / False", null, null }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "Stage",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("34265c85-30ca-40ac-abaa-560d175860e0"), "PREPARATORY", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 2, true, false, "المرحلة الإعدادية", "Preparatory", null, null },
                    { new Guid("362bf79c-b5da-4145-86ea-b4babdd7e022"), "SECONDARY", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 3, true, false, "المرحلة الثانوية", "Secondary", null, null },
                    { new Guid("cc8ed439-fc4b-442c-b998-a190d0a4e97e"), "PRIMARY", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 1, true, false, "المرحلة الابتدائية", "Primary", null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Stage_BackendName",
                schema: "lkp",
                table: "Stage",
                column: "BackendName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Stage_DisplayOrder",
                schema: "lkp",
                table: "Stage",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionType_BackendName",
                schema: "lkp",
                table: "QuestionType",
                column: "BackendName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuestionType_DisplayOrder",
                schema: "lkp",
                table: "QuestionType",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionReviewDecision_BackendName",
                schema: "lkp",
                table: "QuestionReviewDecision",
                column: "BackendName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuestionReviewDecision_DisplayOrder",
                schema: "lkp",
                table: "QuestionReviewDecision",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionChangeType_BackendName",
                schema: "lkp",
                table: "QuestionChangeType",
                column: "BackendName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuestionChangeType_DisplayOrder",
                schema: "lkp",
                table: "QuestionChangeType",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankType_BackendName",
                schema: "lkp",
                table: "QuestionBankType",
                column: "BackendName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankType_DisplayOrder",
                schema: "lkp",
                table: "QuestionBankType",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankRequestType_BackendName",
                schema: "lkp",
                table: "QuestionBankRequestType",
                column: "BackendName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankRequestType_DisplayOrder",
                schema: "lkp",
                table: "QuestionBankRequestType",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankRequestStatus_BackendName",
                schema: "lkp",
                table: "QuestionBankRequestStatus",
                column: "BackendName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankRequestStatus_DisplayOrder",
                schema: "lkp",
                table: "QuestionBankRequestStatus",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankRequestItemStatus_BackendName",
                schema: "lkp",
                table: "QuestionBankRequestItemStatus",
                column: "BackendName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankRequestItemStatus_DisplayOrder",
                schema: "lkp",
                table: "QuestionBankRequestItemStatus",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_DifficultyLevel_BackendName",
                schema: "lkp",
                table: "DifficultyLevel",
                column: "BackendName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DifficultyLevel_DisplayOrder",
                schema: "lkp",
                table: "DifficultyLevel",
                column: "DisplayOrder");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Stage_BackendName",
                schema: "lkp",
                table: "Stage");

            migrationBuilder.DropIndex(
                name: "IX_Stage_DisplayOrder",
                schema: "lkp",
                table: "Stage");

            migrationBuilder.DropIndex(
                name: "IX_QuestionType_BackendName",
                schema: "lkp",
                table: "QuestionType");

            migrationBuilder.DropIndex(
                name: "IX_QuestionType_DisplayOrder",
                schema: "lkp",
                table: "QuestionType");

            migrationBuilder.DropIndex(
                name: "IX_QuestionReviewDecision_BackendName",
                schema: "lkp",
                table: "QuestionReviewDecision");

            migrationBuilder.DropIndex(
                name: "IX_QuestionReviewDecision_DisplayOrder",
                schema: "lkp",
                table: "QuestionReviewDecision");

            migrationBuilder.DropIndex(
                name: "IX_QuestionChangeType_BackendName",
                schema: "lkp",
                table: "QuestionChangeType");

            migrationBuilder.DropIndex(
                name: "IX_QuestionChangeType_DisplayOrder",
                schema: "lkp",
                table: "QuestionChangeType");

            migrationBuilder.DropIndex(
                name: "IX_QuestionBankType_BackendName",
                schema: "lkp",
                table: "QuestionBankType");

            migrationBuilder.DropIndex(
                name: "IX_QuestionBankType_DisplayOrder",
                schema: "lkp",
                table: "QuestionBankType");

            migrationBuilder.DropIndex(
                name: "IX_QuestionBankRequestType_BackendName",
                schema: "lkp",
                table: "QuestionBankRequestType");

            migrationBuilder.DropIndex(
                name: "IX_QuestionBankRequestType_DisplayOrder",
                schema: "lkp",
                table: "QuestionBankRequestType");

            migrationBuilder.DropIndex(
                name: "IX_QuestionBankRequestStatus_BackendName",
                schema: "lkp",
                table: "QuestionBankRequestStatus");

            migrationBuilder.DropIndex(
                name: "IX_QuestionBankRequestStatus_DisplayOrder",
                schema: "lkp",
                table: "QuestionBankRequestStatus");

            migrationBuilder.DropIndex(
                name: "IX_QuestionBankRequestItemStatus_BackendName",
                schema: "lkp",
                table: "QuestionBankRequestItemStatus");

            migrationBuilder.DropIndex(
                name: "IX_QuestionBankRequestItemStatus_DisplayOrder",
                schema: "lkp",
                table: "QuestionBankRequestItemStatus");

            migrationBuilder.DropIndex(
                name: "IX_DifficultyLevel_BackendName",
                schema: "lkp",
                table: "DifficultyLevel");

            migrationBuilder.DropIndex(
                name: "IX_DifficultyLevel_DisplayOrder",
                schema: "lkp",
                table: "DifficultyLevel");

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "DifficultyLevel",
                keyColumn: "Id",
                keyValue: new Guid("580deb45-9304-4fcb-905e-868f5db3848b"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "DifficultyLevel",
                keyColumn: "Id",
                keyValue: new Guid("59e7808a-916b-44c4-92a9-41dd1c6e24e4"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "DifficultyLevel",
                keyColumn: "Id",
                keyValue: new Guid("9bbee09c-e2a3-463b-9901-4b646200074d"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "QuestionBankAssignmentStatus",
                keyColumn: "Id",
                keyValue: new Guid("238e3c8d-84de-4d70-8a1f-192ac93224ba"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "QuestionBankAssignmentStatus",
                keyColumn: "Id",
                keyValue: new Guid("8b73c382-c04b-49f7-bb78-4bc235df872f"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "QuestionBankAssignmentStatus",
                keyColumn: "Id",
                keyValue: new Guid("9b38c5a0-fb16-44f2-8239-cb5ee704129d"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "QuestionBankAssignmentStatus",
                keyColumn: "Id",
                keyValue: new Guid("a4851953-f48a-4aa9-afb9-b51dfbc1e4d8"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "QuestionBankAssignmentStatus",
                keyColumn: "Id",
                keyValue: new Guid("a8ca906a-e1a3-44c9-87c1-8b4ec6192ab0"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "QuestionBankAssignmentStatus",
                keyColumn: "Id",
                keyValue: new Guid("c035e63f-d866-488f-affd-9ae08a7043dd"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "QuestionBankAssignmentStatus",
                keyColumn: "Id",
                keyValue: new Guid("c6cddace-de75-4a6e-8d1f-65e7e93ba4ed"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "QuestionBankRequestItemStatus",
                keyColumn: "Id",
                keyValue: new Guid("1914dd04-27d9-468d-b03e-813df6ce87ff"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "QuestionBankRequestItemStatus",
                keyColumn: "Id",
                keyValue: new Guid("19760735-e5f0-4236-9fff-8487f9f057ef"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "QuestionBankRequestItemStatus",
                keyColumn: "Id",
                keyValue: new Guid("217164f8-c23d-4913-a549-16f80186f9cb"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "QuestionBankRequestItemStatus",
                keyColumn: "Id",
                keyValue: new Guid("bfa1d9c1-a284-452b-8b59-a2d5fc0a6dfe"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "QuestionBankRequestItemStatus",
                keyColumn: "Id",
                keyValue: new Guid("c5ed702c-cfca-4d7e-8bf9-98cdf15f2106"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "QuestionBankRequestItemStatus",
                keyColumn: "Id",
                keyValue: new Guid("f48eedf5-4fe9-4ded-931b-c76bd5c9ba80"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "QuestionBankRequestStatus",
                keyColumn: "Id",
                keyValue: new Guid("4b25e8db-92f1-4b4c-baab-756a5d1fd3f8"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "QuestionBankRequestStatus",
                keyColumn: "Id",
                keyValue: new Guid("66fbfd41-563e-4a12-a006-8a970285f62b"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "QuestionBankRequestStatus",
                keyColumn: "Id",
                keyValue: new Guid("77687c70-161f-4a6c-977e-95f86b37d724"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "QuestionBankRequestStatus",
                keyColumn: "Id",
                keyValue: new Guid("8b6d8c90-2e40-4957-ac52-bd46b0ae9726"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "QuestionBankRequestStatus",
                keyColumn: "Id",
                keyValue: new Guid("97c0610c-a992-43e1-b9a1-bfdbcf5b34b0"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "QuestionBankRequestStatus",
                keyColumn: "Id",
                keyValue: new Guid("f5cbde41-7e46-4793-953f-ebe5f8e31570"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "QuestionBankRequestStatus",
                keyColumn: "Id",
                keyValue: new Guid("fa2e079c-9361-4e5a-9cbd-39dcb73a1520"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "QuestionBankRequestType",
                keyColumn: "Id",
                keyValue: new Guid("4fa8e626-6395-4987-b949-73cb070148d4"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "QuestionBankRequestType",
                keyColumn: "Id",
                keyValue: new Guid("59659e5d-cf4c-46c5-bea1-3198a0f5a58c"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "QuestionBankType",
                keyColumn: "Id",
                keyValue: new Guid("0ef8f0d9-02d7-4863-9bfe-e63355f3686e"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "QuestionBankType",
                keyColumn: "Id",
                keyValue: new Guid("686928db-b630-4689-b265-ae17eded73cf"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "QuestionBankType",
                keyColumn: "Id",
                keyValue: new Guid("a7f9646b-fcc3-4a00-944c-ceed82acd557"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "QuestionChangeType",
                keyColumn: "Id",
                keyValue: new Guid("dd984c21-7b25-444b-85a9-78767e337ddc"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "QuestionChangeType",
                keyColumn: "Id",
                keyValue: new Guid("f02136e2-ac6c-4d3e-9c37-efd28587177d"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "QuestionChangeType",
                keyColumn: "Id",
                keyValue: new Guid("f7abde66-546b-4055-bfed-215c47bea845"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "QuestionReviewDecision",
                keyColumn: "Id",
                keyValue: new Guid("73df4c1d-05a0-4bb4-9f19-5889a791aa27"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "QuestionReviewDecision",
                keyColumn: "Id",
                keyValue: new Guid("a0ce4f21-955b-4290-bf88-fc1775a21410"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "QuestionReviewDecision",
                keyColumn: "Id",
                keyValue: new Guid("a29e6157-401f-49ac-ba46-98ecd8ca73dd"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "QuestionType",
                keyColumn: "Id",
                keyValue: new Guid("04a6e083-d38f-4838-a820-402fa4c53a39"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "QuestionType",
                keyColumn: "Id",
                keyValue: new Guid("62d2e3f9-562d-45f1-ace4-867b929cae43"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "Stage",
                keyColumn: "Id",
                keyValue: new Guid("34265c85-30ca-40ac-abaa-560d175860e0"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "Stage",
                keyColumn: "Id",
                keyValue: new Guid("362bf79c-b5da-4145-86ea-b4babdd7e022"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "Stage",
                keyColumn: "Id",
                keyValue: new Guid("cc8ed439-fc4b-442c-b998-a190d0a4e97e"));

            migrationBuilder.CreateIndex(
                name: "IX_Stage_BackendName",
                schema: "lkp",
                table: "Stage",
                column: "BackendName",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionType_BackendName",
                schema: "lkp",
                table: "QuestionType",
                column: "BackendName",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionReviewDecision_BackendName",
                schema: "lkp",
                table: "QuestionReviewDecision",
                column: "BackendName",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionChangeType_BackendName",
                schema: "lkp",
                table: "QuestionChangeType",
                column: "BackendName",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankType_BackendName",
                schema: "lkp",
                table: "QuestionBankType",
                column: "BackendName",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankRequestType_BackendName",
                schema: "lkp",
                table: "QuestionBankRequestType",
                column: "BackendName",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankRequestStatus_BackendName",
                schema: "lkp",
                table: "QuestionBankRequestStatus",
                column: "BackendName",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankRequestItemStatus_BackendName",
                schema: "lkp",
                table: "QuestionBankRequestItemStatus",
                column: "BackendName",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_DifficultyLevel_BackendName",
                schema: "lkp",
                table: "DifficultyLevel",
                column: "BackendName",
                unique: true,
                filter: "[IsDeleted] = 0");
        }
    }
}
