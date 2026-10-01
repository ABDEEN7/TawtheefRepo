using Tawtheef.Application.Common.Models.Pagination;

namespace Application.Operation.Features.Employee.TestSessions.DTOs;

public sealed record ReadyTestSlotsDto(
    PaginatedResult<ReadyTestSlotListItemDto> Slots,
    ReadyTestSlotsSummaryDto Summary);
