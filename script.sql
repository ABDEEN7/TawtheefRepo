BEGIN TRANSACTION;
UPDATE [lkp].[JobStatus] SET [DescriptionAr] = N'الوظيفة قيد اعتماد النقاط.', [DescriptionEn] = N'Job is pending points approval.', [NameAr] = N'قيد اعتماد النقاط', [NameEn] = N'Pending Points Approval'
WHERE [Id] = '5c360b07-157c-630a-254a-9c01587d80a8';
SELECT @@ROWCOUNT;


INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260324084720_ChangeJobStatusApprovedToPendingPointsApproval', N'10.0.2');

COMMIT;
GO

