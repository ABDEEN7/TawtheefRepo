using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Application.Operation.Features.Employee.ManagementMajorSkill.Majors.Commands;

public record CreateMajorCommand(string NameAr, string NameEn, bool IsActive, Guid? ParentMajorId): ICommand<IResult<Unit>>;
