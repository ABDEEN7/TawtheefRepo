using Seeds.Importing;

namespace Seeds.Pipeline;

internal sealed class ImportStep
{
    public required string Code { get; init; }
    public required string Title { get; init; }
    public IReadOnlyList<string> DependsOn { get; init; } = [];
    public bool UseTransaction { get; init; } = true;
    public required Func<ImportRunner, CancellationToken, Task> RunAsync { get; init; }
}
