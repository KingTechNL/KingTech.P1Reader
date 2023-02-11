namespace KingTech.P1Reader.Broker;

/// <summary>
/// This message broker is used for handling incoming messages in a thread-safe way.
/// </summary>
/// <typeparam name="TMessage">The type of the messages handled by this broker.</typeparam>
public interface IMessageBroker<TMessage>
{
    /// <summary>
    /// Subscribe to messages that are published.
    /// </summary>
    /// <param name="action">The action to trigger when a new message is published.</param>
    /// <param name="filter">Function defining what messages to trigger the action on.</param>
    /// <returns>True on success, false otherwise.</returns>
    public bool Subscribe(Action<TMessage> action, Func<TMessage, bool>? filter = null);

    /// <summary>
    /// Subscribe to messages that are published.
    /// </summary>
    /// <param name="action">The asynchronous action to trigger when a new message is published.</param>
    /// <param name="filter">Function defining what messages to trigger the action on.</param>
    /// <returns>True on success, false otherwise.</returns>
    public bool SubscribeASync(Func<TMessage, Task> action, Func<TMessage, bool>? filter = null);

    /// <summary>
    /// Unsubscribe from a vehicle queue.
    /// If there are no more subscribers for the given vehicle, the queue is removed.
    /// </summary>
    /// <param name="action">The action to unsubscribe.</param>
    /// <returns>True on success, false otherwise.</returns>
    public bool Unsubscribe(Action<TMessage> action);

    /// <summary>
    /// Unsubscribe from a vehicle queue.
    /// If there are no more subscribers for the given vehicle, the queue is removed.
    /// </summary>
    /// <param name="action">The action to unsubscribe.</param>
    /// <returns>True on success, false otherwise.</returns>
    public bool UnsubscribeASync(Func<TMessage, Task> action);

    /// <summary>
    /// Add a message to the vehicle specific message queue.
    /// </summary>
    /// <param name="message">The message to enqueue.</param>
    /// <returns>True on success, false otherwise.</returns>
    public bool Enqueue(TMessage message);
}