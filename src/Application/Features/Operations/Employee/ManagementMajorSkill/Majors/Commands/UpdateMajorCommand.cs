using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Tawtheef.Application.Features.Operations.Employee.ManagementMajorSkill.Majors.Commands;

public record UpdateMajorCommand(Guid Id, string NameAr, string NameEn, bool IsActive, Guid? ParentMajorId): ICommand<IResult<Unit>>;
