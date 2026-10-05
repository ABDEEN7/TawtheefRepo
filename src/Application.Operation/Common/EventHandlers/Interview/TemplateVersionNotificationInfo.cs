using Application.Operation.Common.Services.Interview;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Interview;
using Tawtheef.Notifications.Templates.InterviewShared;

namespace Application.Operation.Common.EventHandlers.Interview;

/// <summary>What the template-version notifications show about the parent template, in either language.</summary>
internal sealed record TemplateVersionNotificationInfo(
    Guid TemplateId, string TitleAr, string? TitleEn, string? JobAr, string? JobEn, int VersionNo)
{
    public string LinkPath => $"{InterviewNotificationLinks.DefaultPath(InterviewEntityKind.Template)}?template={TemplateId}";

    public string Name(string lang) => $"{InterviewNotificationText.Pick(lang, TitleAr, TitleEn)} (v{VersionNo})";

    public string? Job(string lang) => JobAr is null ? null : InterviewNotificationText.Pick(lang, JobAr, JobEn);

    public static async Task<TemplateVersionNotificationInfo?> LoadAsync(
        IUnitOfWork unitOfWork, InterviewTemplateVersion version, CancellationToken ct)
    {
        var template = await unitOfWork.GetEntityRepository<InterviewTemplate>().DbSet
            .AsNoTracking()
            .Where(t => t.Id == version.InterviewTemplateId)
            .Select(t => new
            {
                t.Id,
                t.TitleAr,
                t.TitleEn,
                JobAr = t.JobTitle == null ? null : t.JobTitle.JobNameAr,
                JobEn = t.JobTitle == null ? null : t.JobTitle.JobNameEn
            })
            .FirstOrDefaultAsync(ct);

        return template is null
            ? null
            : new TemplateVersionNotificationInfo(
                template.Id, template.TitleAr, template.TitleEn, template.JobAr, template.JobEn, version.VersionNo);
    }
}
