using System.Collections.Concurrent;

namespace Recruitment.API.Realtime;

public sealed class CandidateExamConnectionRegistry
{
    private readonly ConcurrentDictionary<string, Guid> _connections = new();

    public void Set(string connectionId, Guid attemptId) =>
        _connections[connectionId] = attemptId;

    public bool TryRemove(string connectionId, out Guid attemptId) =>
        _connections.TryRemove(connectionId, out attemptId);

    public IReadOnlyCollection<string> SnapshotConnectionIds() =>
        _connections.Keys.ToArray();
}
