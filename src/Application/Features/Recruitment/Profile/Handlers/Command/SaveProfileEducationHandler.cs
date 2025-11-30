using System.Text.Json;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Recruitment.Profile.Command;
using Tawtheef.Application.Features.Recruitment.Profile.DTOs;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Applicant;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Recruitment.Profile.Handlers.Command;

public sealed class SaveProfileEducationHandler(IUnitOfWork uow, IMediator mediator)
    : IRequestHandler<SaveProfileEducationCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(SaveProfileEducationCommand cmd, CancellationToken ct)
    {
        var profileRepo   = uow.GetEntityRepository<UserProfile>();
        var educationRepo = uow.GetEntityRepository<Qualification>();

        var profile = await profileRepo.DbSet
            .FirstOrDefaultAsync(p => p.UserId == cmd.UserId, ct);

        if (profile is null)
            return Result.Fail<Unit>(ErrorsCodes.UserProfileNotFound);

        var json = cmd.Request.DegreesJson;
        if (string.IsNullOrWhiteSpace(json))
            return Result.Fail<Unit>(ErrorsCodes.InvalidDegreesJson);

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var degrees = JsonSerializer.Deserialize<List<SaveProfileEducationDegreeDto>>(json, options) ?? [];
        if (degrees.Count != cmd.Request.DegreeFiles.Count)
            return Result.Fail<Unit>(ErrorsCodes.InvalidDegreesCount);
        
        for (var i = 0; i < degrees.Count; i++)
        {
            var dto = degrees[i];
            var file = cmd.Request.DegreeFiles[i];
            var uploadResult = await mediator.Send(new UploadAttachmentCommand(file),ct);
            if (uploadResult.IsFailed)
                return Result.Fail<Unit>(uploadResult.Errors);
            var attachmentId = uploadResult.Value.ResourceId;

            var edu = new Qualification
            {
                UserProfileId = profile.Id,
                DegreeId     = dto.DegreeId,
                CountryId     = dto.GradCountryId,
                UniversityId  = dto.UniversityId,
                MajorId       = dto.MajorId,
                SubMajorId       = dto.SubMajorId,
                StudyTypeId   = dto.StudyTypeId,
                RatingId       = dto.GradeId,
                GraduationYear      = dto.GradYear,
                GPA           = dto.Gpa,
                CertificateId  = attachmentId,
            };

            await educationRepo.AddAsync(edu);
        }

        profile.IsDraft = true;
        await uow.SaveChangesAsync(ct);
        return Result.Ok(Unit.Value);
    }
}
