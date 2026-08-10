namespace Application.Operation.Features.Employee.Dashboard.DTOs.Candidates;

public sealed class CandidateTypeKpisDto
{
    public int Total { get; init; }
    public int Qatari { get; init; }
    public int NonQatari { get; init; }
    public int SonOfQatariMother { get; init; }
    public int WifeOfQatari { get; init; }
    public int Gcc { get; init; }
    public int ResidentQatar { get; init; }
    public int Unknown { get; init; }
}
