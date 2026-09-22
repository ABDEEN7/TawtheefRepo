using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Operation.Features.Employee.Interview.Committee.DTOs;

public sealed record EligibleCommitteeMemberDto(
    Guid Id,
    string NameAr,
    string NameEn,
    string? Email);
