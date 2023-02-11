using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace KingTech.P1Reader.Broker;

/// <summary>
/// Extension of ConcurrentQueue featuring reset event.
/// </summary>
/// <typeparam name="TItem">The type of item in the queue.</typeparam>
internal class AwaitableConcurrentQueue<TItem> : IDisposable
{
    private readonly ILogger? _logger;
    private readonly AutoResetEvent _signal;
    private readonly ConcurrentQueue<TItem> _queue;

    public AwaitableConcurrentQueue(ILogger? logger)
    {
        _logger = logger;
        _signal = new AutoResetEvent(false);
        _queue = new ConcurrentQueue<TItem>();
    }

    public void Enqueue(TItem item)
    {
        _logger?.LogTrace("Enqueueing {@Item}", item);
        _queue.Enqueue(item);
        _signal.Set();
    }

    public bool TryDequeue(out TItem? result)
    {
        _logger?.LogTrace("Waiting for next item.");
        result = default;
        if (_queue.IsEmpty)
        {
            return _signal.WaitOne() && _queue.TryDequeue(out result);
        }
        return _queue.TryDequeue(out result);
    }

    public void Clear()
    {
        _queue.Clear();
        _signal.Set();
    }

    public void Dispose()
    {
        _signal.Set();
        _signal.Dispose();
        GC.SuppressFinalize(this);
    }
}