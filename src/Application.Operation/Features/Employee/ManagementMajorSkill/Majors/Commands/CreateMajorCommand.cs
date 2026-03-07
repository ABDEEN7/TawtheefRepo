using MediatR;
using FluentResults;

namespace Application.Operation.Features.Employee.ManagementMajorSkill.Majors.Commands;

public record CreateMajorCommand(string NameAr, string NameEn, bool IsActive, Guid? ParentMajorId): IRequest<IResult<Unit>>;

