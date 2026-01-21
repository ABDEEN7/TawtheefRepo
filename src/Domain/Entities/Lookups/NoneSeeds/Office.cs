using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FluentResults;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Domain.Events.Operation.Employee.Office;

namespace Tawtheef.Domain.Entities.Lookups.NoneSeeds;

public sealed record OfficeCode(string Value)
{
    public static OfficeCode New() => new($"OFF-{Guid.NewGuid():N}");
}

[Table(nameof(Office), Schema = Schemas.Lookup)]
public class Office : LookupBase
{
    public required Guid CountryId { get; init; }
    public Country? Country { get; init; }
    public required Guid OfficeAdminId { get; init; }
    public OfficeUser? OfficeAdmin { get; init; }
    /// <summary>
    /// this property add to be used to mapping with an external system and the old system.
    /// </summary>
    public required string Code { get; init; }
    [MaxLength(10)]
    public string? PhoneCountryCode { get; set; }
    [MaxLength(20)]
    public string? PhoneNumber { get; set; }
    public List<OfficeUser>? OfficeUsers { get; init; }
    public List<OfficeSupportedCountry> SupportedCountries { get; init; } = [];
    
    
    public static Result<Office> Create(OfficeCode code, string nameAr, string nameEn,
        Guid countryId, Guid adminId, string phoneCountryCode, string phoneNumber,
        IEnumerable<Guid> supportedCountryIds)
    {
        if (adminId == Guid.Empty)
            return Result.Fail<Office>(ErrorsCodes.OfficeAdminEmailInvalid);

        var ids = supportedCountryIds.Where(x => x != Guid.Empty).Distinct().ToList();

        if (ids.Count == 0)
            return Result.Fail<Office>(ErrorsCodes.OfficeSupportedCountriesRequired);

        if (countryId == Guid.Empty)
            return Result.Fail<Office>(ErrorsCodes.OfficeCountryRequired);

        var office = new Office
        {
            Id = Guid.NewGuid(),
            Code = code.Value,
            BackendName = code.Value,
            NameAr = nameAr.Trim(),
            NameEn = nameEn .Trim(),
            CountryId = countryId,
            OfficeAdminId = adminId,
            PhoneCountryCode = phoneCountryCode.Trim(),
            PhoneNumber = phoneNumber.Trim()
        };

        if (string.IsNullOrWhiteSpace(office.NameAr) && string.IsNullOrWhiteSpace(office.NameEn))
            return Result.Fail<Office>(ErrorsCodes.OfficeNameRequired);

        foreach (var id in ids)
            office.SupportedCountries.Add(new OfficeSupportedCountry { CountryId = id });

        office.AddDomainEvent(new OfficeCreatedDomainEvent(office.Id, adminId, DateTimeOffset.UtcNow));
        return Result.Ok(office);
    }
}
