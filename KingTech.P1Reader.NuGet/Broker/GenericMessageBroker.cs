using Microsoft.Extensions.Logging;

namespace KingTech.P1Reader.Broker;

/// <summary>
/// This generic message broker is used for handling incoming messages in a thread-safe way.
/// </summary>
/// <typeparam name="TMessage">The type of the messages handled by this broker.</typeparam>
public class GenericMessageBroker<TMessage> : IMessageBroker<TMessage>, IDisposable
{
    private readonly ILogger<GenericMessageBroker<TMessage>>? _logger;

    /// <summary>
    /// Each vehicle has its own queue.
    /// This field allows us to map a queue to a vehicleId.
    /// </summary>
    private readonly AwaitableConcurrentQueueHub<TMessage> _messageQueue;

    public GenericMessageBroker(ILogger<GenericMessageBroker<TMessage>>? logger)
    {
        _logger = logger;
        _messageQueue = new AwaitableConcurrentQueueHub<TMessage>(logger);
        _messageQueue.Start();
    }

    /// <summary>
    /// Subscribe to messages that are published.
    /// </summary>
    /// <param name="action">The action to trigger when a new message is published.</param>
    /// <param name="filter">Function defining what messages to trigger the action on.</param>
    /// <returns>True on success, false otherwise.</returns>
    public bool Subscribe(Action<TMessage> action, Func<TMessage, bool>? filter = null)
    {
        _logger?.LogDebug("Action {action} with filter {filter} subscribed to message broker", action, filter);
        return _messageQueue.Subscribe(action, filter);
    }

    /// <summary>
    /// Subscribe to messages that are published.
    /// </summary>
    /// <param name="action">The asynchronous action to trigger when a new message is published.</param>
    /// <param name="filter">Function defining what messages to trigger the action on.</param>
    /// <returns>True on success, false otherwise.</returns>
    public bool SubscribeASync(Func<TMessage, Task> action, Func<TMessage, bool>? filter = null)
    {
        _logger?.LogDebug("Async action {action} with filter {filter} subscribed to message broker", action, filter);
        return _messageQueue.Subscribe(action, filter);
    }

    /// <summary>
    /// Unsubscribe from a vehicle queue.
    /// If there are no more subscribers for the given vehicle, the queue is removed.
    /// </summary>
    /// <param name="action">The action to unsubscribe.</param>
    /// <returns>True on success, false otherwise.</returns>
    public bool Unsubscribe(Action<TMessage> action)
    {
        _logger?.LogDebug("Action {action} unsubscribed from message broker", action);
        return _messageQueue.Unsubscribe(action);
    }

    /// <summary>
    /// Unsubscribe from a vehicle queue.
    /// If there are no more subscribers for the given vehicle, the queue is removed.
    /// </summary>
    /// <param name="action">The action to unsubscribe.</param>
    /// <returns>True on success, false otherwise.</returns>
    public bool UnsubscribeASync(Func<TMessage, Task> action)
    {
        _logger?.LogDebug("Async action {action} unsubscribed from message broker", action);
        return _messageQueue.Unsubscribe(action);
    }

    /// <summary>
    /// Add a message to the vehicle specific message queue.
    /// </summary>
    /// <param name="message">The message to enqueue.</param>
    /// <returns>True on success, false otherwise.</returns>
    public bool Enqueue(TMessage message)
    {
        _logger?.LogTrace("Enqueuing message {@Message}", message);
        _messageQueue.Enqueue(message);
        return true;
    }

    /// <summary>
    /// Dispose this object.
    /// </summary>
    public void Dispose()
    {
        _logger?.LogTrace("Disposing message broker");
        _messageQueue.Dispose();
        GC.SuppressFinalize(this);
    }
}