using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.QuestionBankAssignments.Queries;

public sealed record CheckQuestionBankImageAccessQuery(string BlobKey) : IRequest<IResult<Unit>>
{
    public string NormalizedBlobKey { get; } = NormalizeKey(BlobKey);

    public bool IsQuestionBankImage => NormalizedBlobKey.StartsWith(
        "private/operation/question-bank/", StringComparison.OrdinalIgnoreCase);

    private static string NormalizeKey(string blobKey)
    {
        // Classify the path storage actually resolves, including alternate separators.
        var parts = new List<string>();
        foreach (var part in blobKey.Replace('\\', '/').Split('/', StringSplitOptions.RemoveEmptyEntries))
        {
            if (part == ".") continue;
            if (part == "..")
            {
                if (parts.Count > 0) parts.RemoveAt(parts.Count - 1);
            }
            else parts.Add(part);
        }
        return string.Join('/', parts);
    }
}
