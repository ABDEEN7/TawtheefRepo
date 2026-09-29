using System.Text.Json.Serialization;

namespace Tawtheef.Domain.Entities.Exams;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TestSessionGenderFilter
{
    Male = 1,
    Female = 2
}
