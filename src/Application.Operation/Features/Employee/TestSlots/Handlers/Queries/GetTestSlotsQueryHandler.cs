using Application.Operation.Features.Employee.TestSlots.DTOs;
using Application.Operation.Features.Employee.TestSlots.Queries;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Extensions;
using Tawtheef.Domain.Entities.Exams;
using Tawtheef.Domain.Entities.Lookups;

namespace Application.Operation.Features.Employee.TestSlots.Handlers.Queries;

public sealed class GetTestSlotsQueryHandler(IUnitOfWork unitOfWork, IUserRepository userRepository)
    : IRequestHandler<GetTestSlotsQuery, IResult<PaginatedResult<TestSlotListItemDto>>>
{
    public async Task<IResult<PaginatedResult<TestSlotListItemDto>>> Handle(
        GetTestSlotsQuery request,
        CancellationToken cancellationToken)
    {
        var searchTerm = request.SearchTerm?.Trim();
        var isArabic = string.Equals(request.Language, "ar", StringComparison.OrdinalIgnoreCase);
        var sessions = unitOfWork.GetEntityRepository<TestSession>().DbSet.AsNoTracking();
        var candidates = unitOfWork.GetEntityRepository<TestSessionCandidate>().DbSet.AsNoTracking();
        var staff = unitOfWork.GetEntityRepository<TestSlotStaff>().DbSet.AsNoTracking();

        var testSlots = unitOfWork.GetEntityRepository<TestSlot>().DbSet.AsNoTracking()
            .WhereIf(!string.IsNullOrWhiteSpace(searchTerm),
                testSlot => EF.Functions.Like(testSlot.SlotNo, $"%{searchTerm}%") ||
                            EF.Functions.Like(testSlot.TitleAr, $"%{searchTerm}%") ||
                            EF.Functions.Like(testSlot.TitleEn!, $"%{searchTerm}%"))
            .WhereIf(request.RoomId.HasValue, testSlot => testSlot.RoomId == request.RoomId);

        if (request.DateFrom is { } dateFrom)
            testSlots = testSlots.Where(testSlot => testSlot.SlotDate >= dateFrom);

        if (request.DateTo is { } dateTo)
            testSlots = testSlots.Where(testSlot => testSlot.SlotDate <= dateTo);

        var page = await testSlots
            .OrderByDescending(testSlot => testSlot.SlotDate)
            .ThenBy(testSlot => testSlot.StartTime)
            .ThenBy(testSlot => testSlot.Id)
            .Select(testSlot => new
            {
                testSlot.Id,
                testSlot.SlotNo,
                Title = isArabic ? testSlot.TitleAr : testSlot.TitleEn ?? testSlot.TitleAr,
                testSlot.RoomId,
                RoomName = isArabic ? testSlot.Room!.NameAr : testSlot.Room!.NameEn ?? testSlot.Room.NameAr,
                testSlot.SlotDate,
                testSlot.StartTime,
                testSlot.EndTime,
                CandidateCount = candidates.Count(candidate => sessions.Any(session =>
                    session.Id == candidate.TestSessionId && session.TestSlotId == testSlot.Id)),
                HallSupervisorId = staff
                    .Where(member => member.TestSlotId == testSlot.Id && member.IsActive &&
                                     member.RoleId == TestSlotStaffRoleIds.HallSupervisor)
                    .OrderBy(member => member.Id)
                    .Select(member => (Guid?)member.StaffUserId)
                    .FirstOrDefault(),
                Status = new DropdownOptions
                {
                    Id = testSlot.StatusId,
                    BackendName = testSlot.Status!.BackendName,
                    Name = isArabic ? testSlot.Status.NameAr : testSlot.Status.NameEn
                }
            })
            .ToPaginatedListAsync(request with
            {
                PageNumber = Math.Max(1, request.PageNumber),
                PageSize = Math.Clamp(request.PageSize, 1, 50),
                SortBy = null
            }, cancellationToken);

        var supervisorIds = page.Items.Where(testSlot => testSlot.HallSupervisorId.HasValue)
            .Select(testSlot => testSlot.HallSupervisorId!.Value)
            .Distinct()
            .ToList();
        var supervisorNames = supervisorIds.Count == 0
            ? new Dictionary<Guid, string>()
            : await userRepository.Repository.DbSet.AsNoTracking()
                .Where(user => supervisorIds.Contains(user.Id))
                .Select(user => new
                {
                    user.Id,
                    Name = isArabic ? user.FullNameAr : user.FullNameEn
                })
                .ToDictionaryAsync(user => user.Id, user => user.Name, cancellationToken);

        var items = page.Items.Select(testSlot => new TestSlotListItemDto
        {
            Id = testSlot.Id,
            SlotNo = testSlot.SlotNo,
            Title = testSlot.Title,
            RoomId = testSlot.RoomId,
            RoomName = testSlot.RoomName,
            SlotDate = testSlot.SlotDate,
            StartTime = testSlot.StartTime,
            EndTime = testSlot.EndTime,
            CandidateCount = testSlot.CandidateCount,
            HallSupervisorId = testSlot.HallSupervisorId,
            HallSupervisorName = testSlot.HallSupervisorId is { } supervisorId
                ? supervisorNames.GetValueOrDefault(supervisorId)
                : null,
            Status = testSlot.Status
        }).ToList();

        return Result.Ok(new PaginatedResult<TestSlotListItemDto>(items,
            page.Metadata.TotalCount, Math.Max(1, request.PageNumber), Math.Clamp(request.PageSize, 1, 50)));
    }
}
