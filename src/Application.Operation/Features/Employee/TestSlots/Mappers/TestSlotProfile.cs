using Application.Operation.Features.Employee.TestSlots.DTOs;
using Mapster;
using Tawtheef.Domain.Entities.Exams;

namespace Application.Operation.Features.Employee.TestSlots.Mappers;

public sealed class TestSlotProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<TestSlot, TestSlotListItemDto>()
            .Ignore(destination => destination.Title, destination => destination.RoomName,
                destination => destination.CandidateCount, destination => destination.HallSupervisorId);
        // destination => destination.HallSupervisorName, destination => destination.Status);
    }
}
