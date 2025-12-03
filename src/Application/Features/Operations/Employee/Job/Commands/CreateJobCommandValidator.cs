using FluentValidation;
using Tawtheef.Application.Common.Constants;
using Tawtheef.Application.Features.Operations.Employee.Job.Dtos;

namespace Tawtheef.Application.Features.Operations.Employee.Job.Commands;

public class CreateJobCommandValidator : AbstractValidator<CreateJobCommand>
{
    public CreateJobCommandValidator()
    {
        // Basic Job Validation
        RuleFor(x => x.Job)
            .NotNull()
            .WithMessage(JobValidationMessages.JOB_REQUIRED);

        When(x => x.Job != null, () =>
        {
            // Basic Information
            RuleFor(x => x.Job.Title)
                .NotEmpty().WithMessage(JobValidationMessages.JOB_TITLE_REQUIRED)
                .MaximumLength(200).WithMessage(JobValidationMessages.JOB_TITLE_MAX_LENGTH);

            RuleFor(x => x.Job.Vacancies)
                .GreaterThan(0)
                .WithMessage(JobValidationMessages.VACANCIES_GREATER_THAN_ZERO);

            RuleFor(x => x.Job.Deadline)
                .GreaterThan(DateTimeOffset.UtcNow)
                .WithMessage(JobValidationMessages.DEADLINE_FUTURE_DATE);

            RuleFor(x => x.Job.Benefits)
                .NotEmpty().WithMessage(JobValidationMessages.JOB_BENEFITS_REQUIRED)
                .MaximumLength(2000).WithMessage(JobValidationMessages.JOB_BENEFITS_MAX_LENGTH);

            RuleFor(x => x.Job.Overview)
                .MaximumLength(4000).WithMessage(JobValidationMessages.JOB_OVERVIEW_MAX_LENGTH)
                .When(x => !string.IsNullOrEmpty(x.Job.Overview));

            RuleFor(x => x.Job.QualificationsDescription)
                .MaximumLength(4000).WithMessage(JobValidationMessages.JOB_QUALIFICATIONS_MAX_LENGTH)
                .When(x => !string.IsNullOrEmpty(x.Job.QualificationsDescription));

            // New Requirements according to BRD
            RuleFor(x => x.Job.MinimumExperienceYears)
                .GreaterThanOrEqualTo(0)
                .WithMessage(JobValidationMessages.MINIMUM_EXPERIENCE_NON_NEGATIVE);

            RuleFor(x => x.Job.MinimumAge)
                .GreaterThanOrEqualTo(18)
                .WithMessage(JobValidationMessages.MINIMUM_AGE_VALID);

            RuleFor(x => x.Job.MaximumAge)
                .GreaterThanOrEqualTo(x => x.Job.MinimumAge)
                .WithMessage(JobValidationMessages.MAXIMUM_AGE_GREATER_THAN_MINIMUM)
                .When(x => x.Job.MinimumAge > 0);

            // Foreign Keys Validation
            RuleFor(x => x.Job.SectorId)
                .NotEmpty().WithMessage(JobValidationMessages.SECTOR_REQUIRED);

            RuleFor(x => x.Job.ManagementId)
                .NotEmpty().WithMessage(JobValidationMessages.MANAGEMENT_REQUIRED);

            RuleFor(x => x.Job.RequestingDepartmentId)
                .NotEmpty().WithMessage(JobValidationMessages.REQUESTING_DEPT_REQUIRED);

            RuleFor(x => x.Job.JobCategoryId)
                .NotEmpty().WithMessage(JobValidationMessages.JOB_CATEGORY_REQUIRED);

            RuleFor(x => x.Job.WorkLocationId)
                .NotEmpty().WithMessage(JobValidationMessages.WORK_LOCATION_REQUIRED);

            RuleFor(x => x.Job.MajorId)
                .NotEmpty().WithMessage(JobValidationMessages.MAJOR_REQUIRED);

            RuleFor(x => x.Job.WorkTypeId)
                .NotEmpty().WithMessage(JobValidationMessages.WORK_TYPE_REQUIRED);

            // Collections Validation
            RuleFor(x => x.Job.Degrees)
                .NotEmpty().WithMessage(JobValidationMessages.DEGREES_REQUIRED);

            // Quota Validation
            //RuleFor(x => x.Job.Quota)
            //    .NotNull().WithMessage(JobValidationMessages.JOB_QUOTAS_REQUIRED)
            //    .Must(HaveValidQuotaTotal)
            //    .WithMessage(JobValidationMessages.JOB_QUOTA_TOTAL_INVALID);

            //When(x => x.Job.Quota != null, () =>
            //{
            //    RuleFor(x => x.Job.Quota.QatariCitizens)
            //        .InclusiveBetween(0, 100).WithMessage(JobValidationMessages.QUOTA_PERCENTAGE_RANGE);

            //    RuleFor(x => x.Job.Quota.QatarMother)
            //        .InclusiveBetween(0, 100).WithMessage(JobValidationMessages.QUOTA_PERCENTAGE_RANGE);

            //    RuleFor(x => x.Job.Quota.NonQatariSpouse)
            //        .InclusiveBetween(0, 100).WithMessage(JobValidationMessages.QUOTA_PERCENTAGE_RANGE);

            //    RuleFor(x => x.Job.Quota.Gcc)
            //        .InclusiveBetween(0, 100).WithMessage(JobValidationMessages.QUOTA_PERCENTAGE_RANGE);

            //    RuleFor(x => x.Job.Quota.QuGrads)
            //        .InclusiveBetween(0, 100).WithMessage(JobValidationMessages.QUOTA_PERCENTAGE_RANGE);

            //    RuleFor(x => x.Job.Quota.Residents)
            //        .InclusiveBetween(0, 100).WithMessage(JobValidationMessages.QUOTA_PERCENTAGE_RANGE);

            //    RuleFor(x => x.Job.Quota.ResidentsBreakdowns)
            //        .NotNull().WithMessage(JobValidationMessages.JOB_QUOTA_BREAKDOWN_REQUIRED)
            //        .Must(list => list!.Count > 0)
            //        .WithMessage(JobValidationMessages.JOB_QUOTA_BREAKDOWN_REQUIRED);

            //    RuleForEach(x => x.Job.Quota.ResidentsBreakdowns)
            //        .ChildRules(b =>
            //        {
            //            b.RuleFor(rb => rb.Percentage)
            //                .GreaterThan(0)
            //                .WithMessage(JobValidationMessages.QUOTA_PERCENTAGE_GREATER_THAN_ZERO)
            //                .LessThanOrEqualTo(100)
            //                .WithMessage(JobValidationMessages.QUOTA_PERCENTAGE_MAX_100);

            //            b.RuleFor(rb => rb.NationalityId)
            //                .NotEmpty()
            //                .WithMessage(JobValidationMessages.QUOTA_NATIONALITY_REQUIRED);
            //        });
            //});

            // Attachments Validation
            RuleForEach(x => x.Job.RequiredAttachments)
                .ChildRules(a =>
                {
                    a.RuleFor(att => att.Title)
                        .NotEmpty().WithMessage(JobValidationMessages.ATTACHMENT_TITLE_REQUIRED)
                        .MaximumLength(200).WithMessage(JobValidationMessages.ATTACHMENT_TITLE_MAX_LENGTH);
                });
        });
    }

    private static bool HaveValidQuotaTotal(JobQuotaDto? quotas)
    {
        if (quotas == null)
            return false;

        var mainQuotasSum = quotas.QatariCitizens + quotas.QatarMother + quotas.NonQatariSpouse +
                           quotas.Gcc + quotas.QuGrads + quotas.Residents;

        if (Math.Abs(mainQuotasSum - 100) > 0.01m)
            return false;

        if (quotas.ResidentsBreakdowns == null || !quotas.ResidentsBreakdowns.Any())
            return false;

        var breakdownSum = quotas.ResidentsBreakdowns.Sum(b => b.Percentage);
        return Math.Abs(breakdownSum - 100) <= 0.01m;
    }
}
