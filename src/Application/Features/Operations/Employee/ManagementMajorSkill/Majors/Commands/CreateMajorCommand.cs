using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Tawtheef.Application.Features.Operations.Employee.ManagementMajorSkill.Majors.Commands;

public record CreateMajorCommand(string NameAr, string NameEn, bool IsActive, Guid? ParentMajorId): ICommand<IResult<Unit>>;
