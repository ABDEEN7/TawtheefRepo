using Application.Operation.Features.Employee.Interview.Evaluation.Services;
using Microsoft.AspNetCore.Http;
using Tawtheef.Application.Common.Security;

namespace Application.Operation.Features.Employee.Interview.Dashboard.Services;

// What the caller may see on the Interview Dashboard, derived only from the rules the source modules
// already enforce - access to the dashboard itself grants nothing extra:
//  - Templates / Committees / Results: the same View|Manage permission their own list endpoints require
//    (those lists are not row-scoped, so neither are these sections).
//  - Execution (schedules, appointments, attendance, evaluation progress, operational issues):
//    InterviewSchedule.* sees everything, exactly like the schedule list; an InterviewEvaluation-only user
//    is limited to the jobs whose committee they sit on via EvaluationSessionAccessResolver - the rule
//    Start Interview already uses (HR/SuperAdmin bypass included).
public sealed record InterviewDashboardAccess(
    bool Templates,
    bool Committees,
    bool Execution,
    IReadOnlyCollection<Guid>? ExecutionJobIds,
    bool Results)
{
    public bool Any => Templates || Committees || Execution || Results;
}

public sealed class InterviewDashboardAccessResolver(
    IHttpContextAccessor httpContextAccessor,
    EvaluationSessionAccessResolver sessionAccessResolver)
{
    public async Task<InterviewDashboardAccess> ResolveAsync(CancellationToken cancellationToken)
    {
        var templates = Has(PermissionKeys.InterviewEvaluationTemplate.View, PermissionKeys.InterviewEvaluationTemplate.Manage);
        var committees = Has(PermissionKeys.InterviewCommittee.View, PermissionKeys.InterviewCommittee.Manage);
        var results = Has(PermissionKeys.InterviewResultReport.View, PermissionKeys.InterviewResultReport.Manage);

        var execution = false;
        IReadOnlyCollection<Guid>? executionJobIds = null;
        if (Has(PermissionKeys.InterviewSchedule.View, PermissionKeys.InterviewSchedule.Manage))
        {
            execution = true;
        }
        else if (Has(PermissionKeys.InterviewEvaluation.View, PermissionKeys.InterviewEvaluation.Manage))
        {
            execution = true;
            executionJobIds = (await sessionAccessResolver.GetAccessibleJobIdsAsync(cancellationToken))?.ToList();
        }

        return new InterviewDashboardAccess(templates, committees, execution, executionJobIds, results);
    }

    private bool Has(params string[] permissions)
    {
        var user = httpContextAccessor.HttpContext?.User;
        return user is not null && permissions.Any(p => user.HasClaim(RoleClaimTypes.Permission, p));
    }
}
