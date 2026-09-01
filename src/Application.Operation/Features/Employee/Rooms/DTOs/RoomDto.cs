using Tawtheef.Application.Common.Models;
using Application.Operation.Features.Employee.Locations.DTOs;

namespace Application.Operation.Features.Employee.Rooms.DTOs;

public sealed record RoomDto
{
    public Guid Id { get; init; }
    public required string NameAr { get; init; }
    public required string NameEn { get; init; }
    public Guid LocationId { get; init; }
    public required LocationDto Location { get; init; }
    public Guid RoomTypeId { get; init; }
    public required DropdownOptions RoomType { get; init; }
    public int Capacity { get; init; }
    public Guid StatusId { get; init; }
    public required DropdownOptions Status { get; init; }
    public string? Notes { get; init; }
    public DateTime LastUpdated { get; init; }
}
