using Tawtheef.Application.Common.Models;

namespace Application.Operation.Features.Employee.TestSessions.DTOs;

public sealed record TestSessionLookupsDto(List<DropdownOptions> Exams, List<DropdownOptions> Jobs,
    List<DropdownOptions> Rooms, List<DropdownOptions> Statuses, List<DropdownOptions> Periods,
    List<DropdownOptions> Nationalities, List<DropdownOptions> Genders);
