using System.Runtime.Serialization;
using Tawtheef.Application.Features.Authenticator.DTOs;

namespace Application.Operation.Features.Authenticator.DTOs;

[DataContract(
    Name = "ApiResponseOfPersonalInfoViewModelY_SsWVfMV",
    Namespace = "http://schemas.datacontract.org/2004/07/MOI.NEW.Services.Models")]
public class PersonalInfoApiResponse
{
    [DataMember]
    public bool IsSuccess { get; set; }

    [DataMember]
    public string? Message { get; set; }

    [DataMember]
    public MOEPersonalInfo? ResponseData { get; set; }
}
