using System.Linq.Expressions;
using Application.Operation.Features.Employee.TestSessions.DTOs;
using Tawtheef.Domain.Entities.Applicant;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;
using Tawtheef.Domain.Entities.Recruitment;

namespace Application.Operation.Features.Employee.TestSessions.Services;

internal static class TestSessionCandidateProjection
{
    public static Expression<Func<Invitation, TestSessionCandidateListItemDto>> ForLanguage(bool isArabic) =>
        invitation => new TestSessionCandidateListItemDto(
            invitation.Id,
            invitation.Applicant!.Profile == null ? null : invitation.Applicant.Profile.NationalNumber,
            isArabic ? invitation.Applicant.FullNameAr : invitation.Applicant.FullNameEn,
            isArabic ? invitation.Job!.JobTitle!.JobNameAr : invitation.Job!.JobTitle!.JobNameEn,
            invitation.Applicant.Profile!.Nationality == null ? null :
                (isArabic ? invitation.Applicant.Profile.Nationality.NameAr : invitation.Applicant.Profile.Nationality.NameEn),
            invitation.Applicant.Profile!.Gender == null ? null :
                (isArabic ? invitation.Applicant.Profile.Gender.NameAr : invitation.Applicant.Profile.Gender.NameEn),
            invitation.Applicant.Profile == null
                ? null
                : invitation.Applicant.Profile.Qualifications!
                    .OrderByDescending(qualification =>
                        qualification.DegreeId == DegreeIds.Doctorate ? 8 :
                        qualification.DegreeId == DegreeIds.Master ? 7 :
                        qualification.DegreeId == DegreeIds.PostgraduateDiploma ? 6 :
                        qualification.DegreeId == DegreeIds.Bachelor ? 5 :
                        qualification.DegreeId == DegreeIds.IntermediateDiploma ? 4 :
                        qualification.DegreeId == DegreeIds.Secondary ? 3 :
                        qualification.DegreeId == DegreeIds.Preparatory ? 2 :
                        qualification.DegreeId == DegreeIds.Primary ? 1 : 0)
                    .ThenByDescending(qualification => qualification.GraduationYear)
                    .ThenByDescending(qualification => qualification.CreatedDate)
                    .Select(qualification => isArabic ? qualification.Degree!.NameAr : qualification.Degree!.NameEn)
                    .FirstOrDefault(),
            invitation.Applicant.Profile == null
                ? null
                : invitation.Applicant.Profile.Qualifications!
                    .OrderByDescending(qualification =>
                        qualification.DegreeId == DegreeIds.Doctorate ? 8 :
                        qualification.DegreeId == DegreeIds.Master ? 7 :
                        qualification.DegreeId == DegreeIds.PostgraduateDiploma ? 6 :
                        qualification.DegreeId == DegreeIds.Bachelor ? 5 :
                        qualification.DegreeId == DegreeIds.IntermediateDiploma ? 4 :
                        qualification.DegreeId == DegreeIds.Secondary ? 3 :
                        qualification.DegreeId == DegreeIds.Preparatory ? 2 :
                        qualification.DegreeId == DegreeIds.Primary ? 1 : 0)
                    .ThenByDescending(qualification => qualification.GraduationYear)
                    .ThenByDescending(qualification => qualification.CreatedDate)
                    .Select(qualification => (decimal?)qualification.GPA)
                    .FirstOrDefault(),
            invitation.Source,
            invitation.Source == InvitationSource.Exceptional
                ? "Excluded"
                : invitation.InvitationStatusId == InvitationStatusIds.NewInvitation ||
                  invitation.InvitationStatusId == InvitationStatusIds.Read
                    ? "NotReady"
                    : "Eligible",
            invitation.Source == InvitationSource.Exceptional
                ? null
                : invitation.InvitationStatusId == InvitationStatusIds.NewInvitation ||
                  invitation.InvitationStatusId == InvitationStatusIds.Read
                    ? isArabic ? invitation.InvitationStatus!.NameAr : invitation.InvitationStatus!.NameEn
                    : null);
}
