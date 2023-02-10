using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace KingTech.P1Reader.Broker;

internal class AwaitableConcurrentQueueHub<TItem> : IDisposable
{
    private readonly ILogger? _logger;
    public bool HasSubscribers => _subscribers.Any();

    private readonly ConcurrentDictionary<int, Func<TItem, Task>> _subscribers;
    private AwaitableConcurrentQueue<TItem>? _queue;
    private CancellationTokenSource? _cancellationTokenSource;

    public AwaitableConcurrentQueueHub(ILogger? logger)
    {
        _logger = logger;
        _queue = new AwaitableConcurrentQueue<TItem>(logger);
        _subscribers = new ConcurrentDictionary<int, Func<TItem, Task>>();
    }

    public void Start(CancellationToken? cancellationToken = null)
    {
        _logger?.LogDebug("Starting awaitable queue hub");
        _cancellationTokenSource = cancellationToken != null ? CancellationTokenSource.CreateLinkedTokenSource(cancellationToken.Value) : new CancellationTokenSource();
        Task.Factory.StartNew(ProcessQueue, _cancellationTokenSource.Token,
            TaskCreationOptions.LongRunning, TaskScheduler.Default);
    }

    public void Stop()
    {
        _logger?.LogDebug("Stopping awaitable queue hub");
        _cancellationTokenSource?.Cancel();
        _queue?.Clear();
        _queue?.Dispose();
        _queue = null;
    }

    public bool Subscribe(Action<TItem> action, Func<TItem, bool>? filter = null) =>
        Subscribe(async item => await Task.Run(() => action(item)), action.GetHashCode(), filter);

    public bool Subscribe(Func<TItem, Task> action, Func<TItem, bool>? filter = null) =>
        Subscribe(action, action.GetHashCode(), filter);


    private bool Subscribe(Func<TItem, Task> action, int hash, Func<TItem, bool>? filter = null)
    {
        if (_subscribers.ContainsKey(hash))
            return false;
        return _subscribers.TryAdd(hash, async message =>
        {
            if (filter == null || filter.Invoke(message))
            {
                await action(message);
            }
        });
    }

    public bool Unsubscribe(Action<TItem> action) => Unsubscribe(action.GetHashCode());

    public bool Unsubscribe(Func<TItem, Task> action) => Unsubscribe(action.GetHashCode());


    private bool Unsubscribe(int hash) => _subscribers.ContainsKey(hash) && _subscribers.Remove(hash, out _);

    public void Enqueue(TItem item) => _queue?.Enqueue(item);

    public void Dispose()
    {
        Stop();
        GC.SuppressFinalize(this);
    }

    private async Task ProcessQueue()
    {
        while (!_cancellationTokenSource?.IsCancellationRequested ?? false)
        {
            if (!(_queue?.TryDequeue(out var item) ?? false))
                continue;


            if (item == null)
            {
                _logger?.LogWarning("Null item dequeued");
                continue;
            }

            _logger?.LogTrace("Invoking all subscribers with item {@Item}", item);
            foreach (var subscriber in _subscribers.Values)
            {
                _logger?.LogTrace("Invoking {subscriber} with item {@Item}", subscriber, item);
                try
                {
                    await subscriber.Invoke(item);
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Exception occurred while invoking item {@Item} on subscriber {subscriber}", item, subscriber);
                }
            }
        }
    }
}