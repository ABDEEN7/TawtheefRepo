using FluentResults;
using MediatR;

namespace Tawtheef.Application.Features.Operations.Employee.ManagementMajorSkill.Majors.Commands;

public record CreateMajorCommand(string NameAr, string NameEn, bool IsActive, Guid? ParentMajorId): IRequest<IResult<Unit>>;
