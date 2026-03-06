using MediatR;
using FluentResults;

namespace Application.Operation.Features.Employee.ManagementMajorSkill.Majors.Commands;

public record UpdateMajorCommand(Guid Id, string NameAr, string NameEn, bool IsActive, Guid? ParentMajorId): IRequest<IResult<Unit>>;

