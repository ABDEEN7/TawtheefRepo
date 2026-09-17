using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Operation.Features.Employee.Interview.EvaluationBank.DTOs;

public sealed record AxisDto(
    Guid Id,
    string NameAr,
    string? NameEn,
    string? DescriptionAr,
    string? DescriptionEn,
    bool IsActive);
