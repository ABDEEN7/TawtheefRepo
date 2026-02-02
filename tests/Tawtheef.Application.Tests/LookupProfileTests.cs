using System.Text.Json;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Common.Mappers;
using Tawtheef.Application.Common.Models;
using Tawtheef.Domain.Common.Interfaces;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Tawtheef.Application.Tests;

public class LookupProfileTests
{
    [Test]
    public void Map_LookupBase_Includes_NameAr_NameEn_In_AdditionalData()
    {
        var config = new TypeAdapterConfig();
        new LookupProfile().Register(config);

        var services = new ServiceCollection()
            .AddSingleton<ILocalizationService>(new TestLocalizationService())
            .BuildServiceProvider();

        using var scope = new MapContextScope();
        scope.Context.Parameters["ServiceProvider"] = services;
        var mapper = new Mapper(config, services);

        var country = new Country
        {
            Id = Guid.NewGuid(),
            BackendName = "test",
            NameAr = "الاسم",
            NameEn = "Name",
            ISOCode = "TST",
            CodeAlpha = "TT",
            Code = 1,
        };

        var result = mapper.Map<DropdownOptions>(country);

        var json = JsonSerializer.Serialize(result.AdditionalData);
        using var doc = JsonDocument.Parse(json);

        Assert.That(doc.RootElement.GetProperty("nameAr").GetString(), Is.EqualTo("الاسم"));
        Assert.That(doc.RootElement.GetProperty("nameEn").GetString(), Is.EqualTo("Name"));
    }

    private sealed class TestLocalizationService : ILocalizationService
    {
        public string GetLocalizedName(ILocalizedName? source) => source?.NameEn ?? string.Empty;

        public string GetLocalizedFullName(ILocalizedFullName? source) => source?.FullNameEn ?? string.Empty;

        public string? GetLocalizedDescription(ILocalizedDescription? source) => source?.DescriptionEn;

        public string GetLocalizedValue(string valueAr, string valueEn) => valueEn;

        public string GetLocalizedValue(string value) => value;
    }
}
