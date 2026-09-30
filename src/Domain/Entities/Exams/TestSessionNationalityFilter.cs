using System.Text.Json.Serialization;

namespace Tawtheef.Domain.Entities.Exams;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TestSessionNationalityFilter
{
    Qatari = 1,
    NonQatari = 2
}
