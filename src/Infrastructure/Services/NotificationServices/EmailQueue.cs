using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Tawtheef.Application.Common.Interfaces.NotificationServices;
using Tawtheef.Application.Common.Models.Notification;

namespace Tawtheef.Infrastructure.Services.NotificationServices;

public sealed class EmailQueue(int capacity = 100) : IEmailQueue, IDisposable
{
    private readonly Channel<EmailEnvelope> _channel = Channel.CreateBounded<EmailEnvelope>(new BoundedChannelOptions(capacity)
    {
        FullMode = BoundedChannelFullMode.Wait,   // producers wait when full
        SingleReader = false,                     // multiple dispatcher workers can read
        SingleWriter = false                      // multiple producers can enqueue
    });

    /// <summary>
    /// Enqueue an email envelope for dispatch.
    /// Will block (async) if the queue is full (bounded).
    /// </summary>
    public ValueTask EnqueueAsync(EmailEnvelope env, CancellationToken ct = default) =>
        _channel.Writer.WriteAsync(env, ct);

    /// <summary>
    /// Reader for dispatcher workers.
    /// </summary>
    public ChannelReader<EmailEnvelope> Reader => _channel.Reader;

    /// <summary>
    /// Try enqueue without waiting (drop or handle backpressure yourself).
    /// </summary>
    public bool TryEnqueue(EmailEnvelope env) => _channel.Writer.TryWrite(env);

    /// <summary>
    /// Marks the queue as complete for writers (no new items).
    /// </summary>
    public void Complete() => _channel.Writer.TryComplete();

    public void Dispose()
    {
        // Make sure to close writer to avoid hanging consumers on shutdown
        Complete();
    }
}
