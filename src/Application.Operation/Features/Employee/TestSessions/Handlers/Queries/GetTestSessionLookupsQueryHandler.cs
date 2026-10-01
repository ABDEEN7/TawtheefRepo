using Application.Operation.Features.Employee.TestSessions.DTOs;
using Application.Operation.Features.Employee.TestSessions.Queries;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models;
using Tawtheef.Domain.Entities.Exams;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Application.Operation.Features.Employee.TestSessions.Handlers.Queries;

public sealed class GetTestSessionLookupsQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetTestSessionLookupsQuery, IResult<TestSessionLookupsDto>>
{
    public async Task<IResult<TestSessionLookupsDto>> Handle(GetTestSessionLookupsQuery request, CancellationToken ct)
    {
        var ar = string.Equals(request.Language, "ar", StringComparison.OrdinalIgnoreCase);
        var exams = await unitOfWork.Context.Set<Exam>().AsNoTracking()
            .Where(x => x.StatusId == ExamStatusIds.Approved).OrderBy(x => x.ExamNo)
            .Select(x =>
                new DropdownOptions
                {
                    Id = x.Id, Name = ar ? x.TitleAr : x.TitleEn ?? x.TitleAr, BackendName = x.ExamNo
                }).ToListAsync(ct);
        var jobs = await unitOfWork.Context.Set<JobTitle>().AsNoTracking().OrderBy(x => x.Id)
            .Select(x => new DropdownOptions
            {
                Id = x.Id, Name = ar ? x.JobNameAr : x.JobNameEn, BackendName = ""
            }).ToListAsync(ct);
        var rooms = await unitOfWork.Context.Set<Room>().AsNoTracking()
            .Where(x => x.StatusId == RoomStatusIds.Active).OrderBy(x => x.Id)
            .Select(x =>
                new DropdownOptions { Id = x.Id, Name = ar ? x.NameAr : x.NameEn ?? x.NameAr, BackendName = "" })
            .ToListAsync(ct);
        var statuses = await unitOfWork.Context.Set<TestSessionStatus>().AsNoTracking().OrderBy(x => x.Id)
            .Select(x =>
                new DropdownOptions { Id = x.Id, Name = ar ? x.NameAr : x.NameEn, BackendName = x.BackendName })
            .ToListAsync(ct);
        var periods = request.RoomId.HasValue
            ? await unitOfWork.Context.Set<TestSlot>().AsNoTracking().Where(x => x.RoomId == request.RoomId.Value)
                .OrderBy(x => x.SlotDate).ThenBy(x => x.StartTime)
                .Select(x => new DropdownOptions
                {
                    Id = x.Id, Name = ar ? x.TitleAr : x.TitleEn ?? x.TitleAr, BackendName = ""
                })
                .ToListAsync(ct)
            : [];
        var nationalities = await unitOfWork.Context.Set<Country>().AsNoTracking().OrderBy(x => x.Id)
            .Select(x => new DropdownOptions { Id = x.Id, Name = ar ? x.NameAr : x.NameEn, BackendName = "" })
            .ToListAsync(ct);
        var genders = await unitOfWork.Context.Set<Gender>().AsNoTracking().OrderBy(x => x.Id)
            .Select(x => new DropdownOptions { Id = x.Id, Name = ar ? x.NameAr : x.NameEn, BackendName = "" })
            .ToListAsync(ct);
        return Result.Ok(new TestSessionLookupsDto(exams, jobs, rooms, statuses, periods, nationalities, genders));
    }
}
