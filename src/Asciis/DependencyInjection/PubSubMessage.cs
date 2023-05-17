//===============================================================================
// Based on concepts from TinyMessenger
//===============================================================================


namespace No8.Ascii.DependencyInjection;

/// <summary>
/// A message to be published/delivered by PubSubMessenger
/// </summary>
public interface IPubSubMessage
{
    /// <summary>
    /// The sender of the message, or null if not supported by the message implementation.
    /// </summary>
    object? Sender { get; }
}

/// <summary>
/// Base class for messages that provides weak reference storage of the sender
/// </summary>
public abstract class PubSubMessageBase : IPubSubMessage
{
    /// <summary>
    /// Store a WeakReference to the sender just in case anyone is daft enough to
    /// keep the message around and prevent the sender from being collected.
    /// </summary>
    private readonly WeakReference _sender;

    public object? Sender => _sender.IsAlive ? _sender.Target : null;

    /// <summary>
    /// Initializes a new instance of the MessageBase class.
    /// </summary>
    /// <param name="sender">Message sender (usually "this")</param>
    protected PubSubMessageBase(object sender)
    {
        if (sender == null)
            throw new ArgumentNullException(nameof(sender));

        _sender = new WeakReference(sender);
    }
}

/// <summary>
/// Generic message with user specified content
/// </summary>
/// <typeparam name="TContent">Content type to store</typeparam>
public class GenericPubSubMessage<TContent> : PubSubMessageBase
{
    /// <summary>
    /// Contents of the message
    /// </summary>
    public TContent Content { get; protected set; }

    /// <summary>
    /// Create a new instance of the GenericPubSubMessage class.
    /// </summary>
    /// <param name="sender">Message sender (usually "this")</param>
    /// <param name="content">Contents of the message</param>
    public GenericPubSubMessage(object sender, TContent content)
        : base(sender)
    {
        Content = content;
    }
}

/// <summary>
/// Basic "cancellable" generic message
/// </summary>
/// <typeparam name="TContent">Content type to store</typeparam>
public class CancellableGenericPubSubMessage<TContent> : PubSubMessageBase
{
    /// <summary>
    /// Cancel action
    /// </summary>
    public Action Cancel { get; protected set; }

    /// <summary>
    /// Contents of the message
    /// </summary>
    public TContent Content { get; protected set; }

    /// <summary>
    /// Create a new instance of the CancellableGenericPubSubMessage class.
    /// </summary>
    /// <param name="sender">Message sender (usually "this")</param>
    /// <param name="content">Contents of the message</param>
    /// <param name="cancelAction">Action to call for cancellation</param>
    public CancellableGenericPubSubMessage(object sender, TContent content, Action cancelAction)
        : base(sender)
    {
        Content = content;
        Cancel  = cancelAction ?? throw new ArgumentNullException(nameof(cancelAction));
    }
}

/// <summary>
/// Represents an active subscription to a message
/// </summary>
public sealed class TinyMessageSubscriptionToken : IDisposable
{
    private readonly WeakReference _hub;
    private readonly Type          _messageType;

    /// <summary>
    /// Initializes a new instance of the TinyMessageSubscriptionToken class.
    /// </summary>
    public TinyMessageSubscriptionToken(ITinyMessengerHub hub, Type messageType)
    {
        if (hub == null)
            throw new ArgumentNullException(nameof(hub));

        if (!typeof(IPubSubMessage).IsAssignableFrom(messageType))
            throw new ArgumentOutOfRangeException(nameof(messageType));

        _hub         = new WeakReference(hub);
        _messageType = messageType;
    }

    public void Dispose()
    {
        if (_hub.IsAlive)
        {
            if (_hub.Target is ITinyMessengerHub hub)
            {
                var unsubscribeMethod = typeof(ITinyMessengerHub).GetMethod("Unsubscribe", new[] { typeof(TinyMessageSubscriptionToken) });
                unsubscribeMethod = unsubscribeMethod?.MakeGenericMethod(_messageType);

                unsubscribeMethod?.Invoke(hub, new object[] { this });
            }
        }
    }
}

/// <summary>
/// Represents a message subscription
/// </summary>
public interface ITinyMessageSubscription
{
    /// <summary>
    /// Token returned to the subscribed to reference this subscription
    /// </summary>
    TinyMessageSubscriptionToken SubscriptionToken { get; }

    /// <summary>
    /// Whether delivery should be attempted.
    /// </summary>
    /// <param name="message">Message that may potentially be delivered.</param>
    /// <returns>True - ok to send, False - should not attempt to send</returns>
    bool ShouldAttemptDelivery(IPubSubMessage message);

    /// <summary>
    /// Deliver the message
    /// </summary>
    /// <param name="message">Message to deliver</param>
    void Deliver(IPubSubMessage message);
}

/// <summary>
/// Message proxy definition.
/// 
/// A message proxy can be used to intercept/alter messages and/or
/// marshall delivery actions onto a particular thread.
/// </summary>
public interface ITinyMessageProxy
{
    void Deliver(IPubSubMessage message, ITinyMessageSubscription subscription);
}

/// <summary>
/// Default "pass through" proxy.
/// 
/// Does nothing other than deliver the message.
/// </summary>
public sealed class DefaultTinyMessageProxy : ITinyMessageProxy
{
    static DefaultTinyMessageProxy() { }

    /// <summary>
    /// Singleton instance of the proxy.
    /// </summary>
    public static DefaultTinyMessageProxy Instance { get; } = new();

    private DefaultTinyMessageProxy()
    {
    }

    public void Deliver(IPubSubMessage message, ITinyMessageSubscription subscription)
    {
        subscription.Deliver(message);
    }
}
    
/// <summary>
/// Thrown when an exceptions occurs while subscribing to a message type
/// </summary>
public class TinyMessengerSubscriptionException : Exception
{
    private const string ErrorText = "Unable to add subscription for {0} : {1}";

    public TinyMessengerSubscriptionException(Type messageType, string reason)
        : base(String.Format(ErrorText, messageType, reason))
    {

    }

    public TinyMessengerSubscriptionException(Type messageType, string reason, Exception innerException)
        : base(String.Format(ErrorText, messageType, reason), innerException)
    {

    }
}
    
/// <summary>
/// Messenger hub responsible for taking subscriptions/publications and delivering of messages.
/// </summary>
public interface ITinyMessengerHub
{
    /// <summary>
    /// Subscribe to a message type with the given destination and delivery action.
    /// All references are held with WeakReferences
    /// 
    /// All messages of this type will be delivered.
    /// </summary>
    /// <typeparam name="TMessage">Type of message</typeparam>
    /// <param name="deliveryAction">Action to invoke when message is delivered</param>
    /// <returns>TinyMessageSubscription used to unsubscribing</returns>
    TinyMessageSubscriptionToken Subscribe<TMessage>(Action<TMessage> deliveryAction) where TMessage : class, IPubSubMessage;

    /// <summary>
    /// Subscribe to a message type with the given destination and delivery action.
    /// Messages will be delivered via the specified proxy.
    /// All references (apart from the proxy) are held with WeakReferences
    /// 
    /// All messages of this type will be delivered.
    /// </summary>
    /// <typeparam name="TMessage">Type of message</typeparam>
    /// <param name="deliveryAction">Action to invoke when message is delivered</param>
    /// <param name="proxy">Proxy to use when delivering the messages</param>
    /// <returns>TinyMessageSubscription used to unsubscribing</returns>
    TinyMessageSubscriptionToken Subscribe<TMessage>(Action<TMessage> deliveryAction, ITinyMessageProxy proxy) where TMessage : class, IPubSubMessage;

    /// <summary>
    /// Subscribe to a message type with the given destination and delivery action.
    /// 
    /// All messages of this type will be delivered.
    /// </summary>
    /// <typeparam name="TMessage">Type of message</typeparam>
    /// <param name="deliveryAction">Action to invoke when message is delivered</param>
    /// <param name="useStrongReferences">Use strong references to destination and deliveryAction </param>
    /// <returns>TinyMessageSubscription used to unsubscribing</returns>
    TinyMessageSubscriptionToken Subscribe<TMessage>(Action<TMessage> deliveryAction, bool useStrongReferences) where TMessage : class, IPubSubMessage;

    /// <summary>
    /// Subscribe to a message type with the given destination and delivery action.
    /// Messages will be delivered via the specified proxy.
    /// 
    /// All messages of this type will be delivered.
    /// </summary>
    /// <typeparam name="TMessage">Type of message</typeparam>
    /// <param name="deliveryAction">Action to invoke when message is delivered</param>
    /// <param name="useStrongReferences">Use strong references to destination and deliveryAction </param>
    /// <param name="proxy">Proxy to use when delivering the messages</param>
    /// <returns>TinyMessageSubscription used to unsubscribing</returns>
    TinyMessageSubscriptionToken Subscribe<TMessage>(Action<TMessage> deliveryAction, bool useStrongReferences, ITinyMessageProxy proxy) where TMessage : class, IPubSubMessage;

    /// <summary>
    /// Subscribe to a message type with the given destination and delivery action with the given filter.
    /// All references are held with WeakReferences
    /// 
    /// Only messages that "pass" the filter will be delivered.
    /// </summary>
    /// <typeparam name="TMessage">Type of message</typeparam>
    /// <param name="deliveryAction">Action to invoke when message is delivered</param>
    /// <param name="messageFilter"></param>
    /// <returns>TinyMessageSubscription used to unsubscribing</returns>
    TinyMessageSubscriptionToken Subscribe<TMessage>(Action<TMessage> deliveryAction, Func<TMessage, bool> messageFilter) where TMessage : class, IPubSubMessage;

    /// <summary>
    /// Subscribe to a message type with the given destination and delivery action with the given filter.
    /// Messages will be delivered via the specified proxy.
    /// All references (apart from the proxy) are held with WeakReferences
    /// 
    /// Only messages that "pass" the filter will be delivered.
    /// </summary>
    /// <typeparam name="TMessage">Type of message</typeparam>
    /// <param name="deliveryAction">Action to invoke when message is delivered</param>
    /// <param name="messageFilter"></param>
    /// <param name="proxy">Proxy to use when delivering the messages</param>
    /// <returns>TinyMessageSubscription used to unsubscribing</returns>
    TinyMessageSubscriptionToken Subscribe<TMessage>(Action<TMessage> deliveryAction, Func<TMessage, bool> messageFilter, ITinyMessageProxy proxy) where TMessage : class, IPubSubMessage;

    /// <summary>
    /// Subscribe to a message type with the given destination and delivery action with the given filter.
    /// All references are held with WeakReferences
    /// 
    /// Only messages that "pass" the filter will be delivered.
    /// </summary>
    /// <typeparam name="TMessage">Type of message</typeparam>
    /// <param name="deliveryAction">Action to invoke when message is delivered</param>
    /// <param name="messageFilter"></param>
    /// <param name="useStrongReferences">Use strong references to destination and deliveryAction </param>
    /// <returns>TinyMessageSubscription used to unsubscribing</returns>
    TinyMessageSubscriptionToken Subscribe<TMessage>(Action<TMessage> deliveryAction, Func<TMessage, bool> messageFilter, bool useStrongReferences) where TMessage : class, IPubSubMessage;

    /// <summary>
    /// Subscribe to a message type with the given destination and delivery action with the given filter.
    /// Messages will be delivered via the specified proxy.
    /// All references are held with WeakReferences
    /// 
    /// Only messages that "pass" the filter will be delivered.
    /// </summary>
    /// <typeparam name="TMessage">Type of message</typeparam>
    /// <param name="deliveryAction">Action to invoke when message is delivered</param>
    /// <param name="messageFilter"></param>
    /// <param name="useStrongReferences">Use strong references to destination and deliveryAction </param>
    /// <param name="proxy">Proxy to use when delivering the messages</param>
    /// <returns>TinyMessageSubscription used to unsubscribing</returns>
    TinyMessageSubscriptionToken Subscribe<TMessage>(Action<TMessage> deliveryAction, Func<TMessage, bool> messageFilter, bool useStrongReferences, ITinyMessageProxy proxy) where TMessage : class, IPubSubMessage;

    /// <summary>
    /// Unsubscribe from a particular message type.
    /// 
    /// Does not throw an exception if the subscription is not found.
    /// </summary>
    /// <typeparam name="TMessage">Type of message</typeparam>
    /// <param name="subscriptionToken">Subscription token received from Subscribe</param>
    void Unsubscribe<TMessage>(TinyMessageSubscriptionToken subscriptionToken) where TMessage : class, IPubSubMessage;

    /// <summary>
    /// Publish a message to any subscribers
    /// </summary>
    /// <typeparam name="TMessage">Type of message</typeparam>
    /// <param name="message">Message to deliver</param>
    void Publish<TMessage>(TMessage message) where TMessage : class, IPubSubMessage;

    /// <summary>
    /// Publish a message to any subscribers asynchronously
    /// </summary>
    /// <typeparam name="TMessage">Type of message</typeparam>
    /// <param name="message">Message to deliver</param>
    void PublishAsync<TMessage>(TMessage message) where TMessage : class, IPubSubMessage;

    /// <summary>
    /// Publish a message to any subscribers asynchronously
    /// </summary>
    /// <typeparam name="TMessage">Type of message</typeparam>
    /// <param name="message">Message to deliver</param>
    /// <param name="callback">AsyncCallback called on completion</param>
    void PublishAsync<TMessage>(TMessage message, AsyncCallback callback) where TMessage : class, IPubSubMessage;
}
    
/// <summary>
/// Messenger hub responsible for taking subscriptions/publications and delivering of messages.
/// </summary>
public sealed class TinyMessengerHub : ITinyMessengerHub
{
        
    private class WeakTinyMessageSubscription<TMessage> : ITinyMessageSubscription
        where TMessage : class, IPubSubMessage
    {
        private readonly WeakReference _deliveryAction;
        private readonly WeakReference _messageFilter;

        public TinyMessageSubscriptionToken SubscriptionToken { get; }

        public bool ShouldAttemptDelivery(IPubSubMessage message)
        {
            if (message is not TMessage tinyMessage)
                return false;

            if (!_deliveryAction.IsAlive)
                return false;

            if (!_messageFilter.IsAlive)
                return false;

            return ((Func<TMessage, bool>?)_messageFilter.Target)?.Invoke(tinyMessage) ?? false;
        }

        public void Deliver(IPubSubMessage message)
        {
            if (message is not TMessage tinyMessage)
                throw new ArgumentException("Message is not the correct type");

            if (!_deliveryAction.IsAlive)
                return;

            ((Action<TMessage>?)_deliveryAction.Target)?.Invoke(tinyMessage);
        }

        /// <summary>
        /// Initializes a new instance of the WeakTinyMessageSubscription class.
        /// </summary>
        /// <param name="subscriptionToken"></param>
        /// <param name="deliveryAction">Delivery action</param>
        /// <param name="messageFilter">Filter function</param>
        public WeakTinyMessageSubscription(TinyMessageSubscriptionToken subscriptionToken, Action<TMessage> deliveryAction, Func<TMessage, bool> messageFilter)
        {
            if (deliveryAction == null)
                throw new ArgumentNullException(nameof(deliveryAction));

            if (messageFilter == null)
                throw new ArgumentNullException(nameof(messageFilter));

            SubscriptionToken = subscriptionToken ?? throw new ArgumentNullException(nameof(subscriptionToken));
            _deliveryAction   = new WeakReference(deliveryAction);
            _messageFilter    = new WeakReference(messageFilter);
        }
    }

    private class StrongTinyMessageSubscription<TMessage> : ITinyMessageSubscription
        where TMessage : class, IPubSubMessage
    {
        private readonly Action<TMessage>     _deliveryAction;
        private readonly Func<TMessage, bool> _messageFilter;

        public TinyMessageSubscriptionToken SubscriptionToken { get; }

        public bool ShouldAttemptDelivery(IPubSubMessage message)
        {
            if (message is not TMessage tinyMessage)
                return false;

            return _messageFilter.Invoke(tinyMessage);
        }

        public void Deliver(IPubSubMessage message)
        {
            if (message is not TMessage tinyMessage)
                throw new ArgumentException("Message is not the correct type");

            _deliveryAction.Invoke(tinyMessage);
        }

        /// <summary>
        /// Initializes a new instance of the TinyMessageSubscription class.
        /// </summary>
        /// <param name="subscriptionToken"></param>
        /// <param name="deliveryAction">Delivery action</param>
        /// <param name="messageFilter">Filter function</param>
        public StrongTinyMessageSubscription(TinyMessageSubscriptionToken subscriptionToken, Action<TMessage> deliveryAction, Func<TMessage, bool> messageFilter)
        {
            SubscriptionToken = subscriptionToken ?? throw new ArgumentNullException(nameof(subscriptionToken));
            _deliveryAction   = deliveryAction ?? throw new ArgumentNullException(nameof(deliveryAction));
            _messageFilter    = messageFilter ?? throw new ArgumentNullException(nameof(messageFilter));
        }
    }
        
    private class SubscriptionItem
    {
        public ITinyMessageProxy        Proxy        { get; }
        public ITinyMessageSubscription Subscription { get; }

        public SubscriptionItem(ITinyMessageProxy proxy, ITinyMessageSubscription subscription)
        {
            Proxy        = proxy;
            Subscription = subscription;
        }
    }

    private readonly object                                   _subscriptionsPadlock = new();
    private readonly Dictionary<Type, List<SubscriptionItem>> _subscriptions        = new();
        
    /// <summary>
    /// Subscribe to a message type with the given destination and delivery action.
    /// All references are held with strong references
    /// 
    /// All messages of this type will be delivered.
    /// </summary>
    /// <typeparam name="TMessage">Type of message</typeparam>
    /// <param name="deliveryAction">Action to invoke when message is delivered</param>
    /// <returns>TinyMessageSubscription used to unsubscribing</returns>
    public TinyMessageSubscriptionToken Subscribe<TMessage>(Action<TMessage> deliveryAction) where TMessage : class, IPubSubMessage
    {
        return AddSubscriptionInternal(deliveryAction, (_) => true, true, DefaultTinyMessageProxy.Instance);
    }

    /// <summary>
    /// Subscribe to a message type with the given destination and delivery action.
    /// Messages will be delivered via the specified proxy.
    /// All references (apart from the proxy) are held with strong references
    /// 
    /// All messages of this type will be delivered.
    /// </summary>
    /// <typeparam name="TMessage">Type of message</typeparam>
    /// <param name="deliveryAction">Action to invoke when message is delivered</param>
    /// <param name="proxy">Proxy to use when delivering the messages</param>
    /// <returns>TinyMessageSubscription used to unsubscribing</returns>
    public TinyMessageSubscriptionToken Subscribe<TMessage>(Action<TMessage> deliveryAction, ITinyMessageProxy proxy) where TMessage : class, IPubSubMessage
    {
        return AddSubscriptionInternal(deliveryAction, (_) => true, true, proxy);
    }

    /// <summary>
    /// Subscribe to a message type with the given destination and delivery action.
    /// 
    /// All messages of this type will be delivered.
    /// </summary>
    /// <typeparam name="TMessage">Type of message</typeparam>
    /// <param name="deliveryAction">Action to invoke when message is delivered</param>
    /// <param name="useStrongReferences">Use strong references to destination and deliveryAction </param>
    /// <returns>TinyMessageSubscription used to unsubscribing</returns>
    public TinyMessageSubscriptionToken Subscribe<TMessage>(Action<TMessage> deliveryAction, bool useStrongReferences) where TMessage : class, IPubSubMessage
    {
        return AddSubscriptionInternal(deliveryAction, (_) => true, useStrongReferences, DefaultTinyMessageProxy.Instance);
    }

    /// <summary>
    /// Subscribe to a message type with the given destination and delivery action.
    /// Messages will be delivered via the specified proxy.
    /// 
    /// All messages of this type will be delivered.
    /// </summary>
    /// <typeparam name="TMessage">Type of message</typeparam>
    /// <param name="deliveryAction">Action to invoke when message is delivered</param>
    /// <param name="useStrongReferences">Use strong references to destination and deliveryAction </param>
    /// <param name="proxy">Proxy to use when delivering the messages</param>
    /// <returns>TinyMessageSubscription used to unsubscribing</returns>
    public TinyMessageSubscriptionToken Subscribe<TMessage>(Action<TMessage> deliveryAction, bool useStrongReferences, ITinyMessageProxy proxy) where TMessage : class, IPubSubMessage
    {
        return AddSubscriptionInternal(deliveryAction, (_) => true, useStrongReferences, proxy);
    }

    /// <summary>
    /// Subscribe to a message type with the given destination and delivery action with the given filter.
    /// All references are held with WeakReferences
    /// 
    /// Only messages that "pass" the filter will be delivered.
    /// </summary>
    /// <typeparam name="TMessage">Type of message</typeparam>
    /// <param name="deliveryAction">Action to invoke when message is delivered</param>
    /// <param name="messageFilter"></param>
    /// <returns>TinyMessageSubscription used to unsubscribing</returns>
    public TinyMessageSubscriptionToken Subscribe<TMessage>(Action<TMessage> deliveryAction, Func<TMessage, bool> messageFilter) where TMessage : class, IPubSubMessage
    {
        return AddSubscriptionInternal(deliveryAction, messageFilter, true, DefaultTinyMessageProxy.Instance);
    }

    /// <summary>
    /// Subscribe to a message type with the given destination and delivery action with the given filter.
    /// Messages will be delivered via the specified proxy.
    /// All references (apart from the proxy) are held with WeakReferences
    /// 
    /// Only messages that "pass" the filter will be delivered.
    /// </summary>
    /// <typeparam name="TMessage">Type of message</typeparam>
    /// <param name="deliveryAction">Action to invoke when message is delivered</param>
    /// <param name="messageFilter"></param>
    /// <param name="proxy">Proxy to use when delivering the messages</param>
    /// <returns>TinyMessageSubscription used to unsubscribing</returns>
    public TinyMessageSubscriptionToken Subscribe<TMessage>(Action<TMessage> deliveryAction, Func<TMessage, bool> messageFilter, ITinyMessageProxy proxy) where TMessage : class, IPubSubMessage
    {
        return AddSubscriptionInternal(deliveryAction, messageFilter, true, proxy);
    }

    /// <summary>
    /// Subscribe to a message type with the given destination and delivery action with the given filter.
    /// All references are held with WeakReferences
    /// 
    /// Only messages that "pass" the filter will be delivered.
    /// </summary>
    /// <typeparam name="TMessage">Type of message</typeparam>
    /// <param name="deliveryAction">Action to invoke when message is delivered</param>
    /// <param name="messageFilter"></param>
    /// <param name="useStrongReferences">Use strong references to destination and deliveryAction </param>
    /// <returns>TinyMessageSubscription used to unsubscribing</returns>
    public TinyMessageSubscriptionToken Subscribe<TMessage>(Action<TMessage> deliveryAction, Func<TMessage, bool> messageFilter, bool useStrongReferences) where TMessage : class, IPubSubMessage
    {
        return AddSubscriptionInternal(deliveryAction, messageFilter, useStrongReferences, DefaultTinyMessageProxy.Instance);
    }

    /// <summary>
    /// Subscribe to a message type with the given destination and delivery action with the given filter.
    /// Messages will be delivered via the specified proxy.
    /// All references are held with WeakReferences
    /// 
    /// Only messages that "pass" the filter will be delivered.
    /// </summary>
    /// <typeparam name="TMessage">Type of message</typeparam>
    /// <param name="deliveryAction">Action to invoke when message is delivered</param>
    /// <param name="messageFilter"></param>
    /// <param name="useStrongReferences">Use strong references to destination and deliveryAction </param>
    /// <param name="proxy">Proxy to use when delivering the messages</param>
    /// <returns>TinyMessageSubscription used to unsubscribing</returns>
    public TinyMessageSubscriptionToken Subscribe<TMessage>(Action<TMessage> deliveryAction, Func<TMessage, bool> messageFilter, bool useStrongReferences, ITinyMessageProxy proxy) where TMessage : class, IPubSubMessage
    {
        return AddSubscriptionInternal(deliveryAction, messageFilter, useStrongReferences, proxy);
    }

    /// <summary>
    /// Unsubscribe from a particular message type.
    /// 
    /// Does not throw an exception if the subscription is not found.
    /// </summary>
    /// <typeparam name="TMessage">Type of message</typeparam>
    /// <param name="subscriptionToken">Subscription token received from Subscribe</param>
    public void Unsubscribe<TMessage>(TinyMessageSubscriptionToken subscriptionToken) where TMessage : class, IPubSubMessage
    {
        RemoveSubscriptionInternal<TMessage>(subscriptionToken);
    }

    /// <summary>
    /// Publish a message to any subscribers
    /// </summary>
    /// <typeparam name="TMessage">Type of message</typeparam>
    /// <param name="message">Message to deliver</param>
    public void Publish<TMessage>(TMessage message) where TMessage : class, IPubSubMessage
    {
        PublishInternal(message);
    }

    /// <summary>
    /// Publish a message to any subscribers asynchronously
    /// </summary>
    /// <typeparam name="TMessage">Type of message</typeparam>
    /// <param name="message">Message to deliver</param>
    public void PublishAsync<TMessage>(TMessage message) where TMessage : class, IPubSubMessage
    {
        PublishAsyncInternal(message, null);
    }

    /// <summary>
    /// Publish a message to any subscribers asynchronously
    /// </summary>
    /// <typeparam name="TMessage">Type of message</typeparam>
    /// <param name="message">Message to deliver</param>
    /// <param name="callback">AsyncCallback called on completion</param>
    public void PublishAsync<TMessage>(TMessage message, AsyncCallback? callback) where TMessage : class, IPubSubMessage
    {
        PublishAsyncInternal(message, callback);
    }
        
    private TinyMessageSubscriptionToken AddSubscriptionInternal<TMessage>(Action<TMessage> deliveryAction, Func<TMessage, bool> messageFilter, bool strongReference, ITinyMessageProxy proxy)
        where TMessage : class, IPubSubMessage
    {
        if (deliveryAction == null)
            throw new ArgumentNullException(nameof(deliveryAction));

        if (messageFilter == null)
            throw new ArgumentNullException(nameof(messageFilter));

        if (proxy == null)
            throw new ArgumentNullException(nameof(proxy));

        lock (_subscriptionsPadlock)
        {
            if (!_subscriptions.TryGetValue(typeof(TMessage), out var currentSubscriptions))
            {
                currentSubscriptions             = new List<SubscriptionItem>();
                _subscriptions[typeof(TMessage)] = currentSubscriptions;
            }

            var subscriptionToken = new TinyMessageSubscriptionToken(this, typeof(TMessage));

            ITinyMessageSubscription subscription;
            if (strongReference)
                subscription = new StrongTinyMessageSubscription<TMessage>(subscriptionToken, deliveryAction, messageFilter);
            else
                subscription = new WeakTinyMessageSubscription<TMessage>(subscriptionToken, deliveryAction, messageFilter);

            currentSubscriptions.Add(new SubscriptionItem(proxy, subscription));

            return subscriptionToken;
        }
    }

    private void RemoveSubscriptionInternal<TMessage>(TinyMessageSubscriptionToken subscriptionToken)
        where TMessage : class, IPubSubMessage
    {
        if (subscriptionToken == null)
            throw new ArgumentNullException(nameof(subscriptionToken));

        lock (_subscriptionsPadlock)
        {
            if (!_subscriptions.TryGetValue(typeof(TMessage), out var currentSubscriptions))
                return;

            var currentlySubscribed = (from sub in currentSubscriptions
                where ReferenceEquals(sub.Subscription.SubscriptionToken, subscriptionToken)
                select sub).ToList();

            currentlySubscribed.ForEach(sub => currentSubscriptions.Remove(sub));
        }
    }

    private void PublishInternal<TMessage>(TMessage message)
        where TMessage : class, IPubSubMessage
    {
        if (message == null)
            throw new ArgumentNullException(nameof(message));

        List<SubscriptionItem> currentlySubscribed;
        lock (_subscriptionsPadlock)
        {
            if (!_subscriptions.TryGetValue(typeof(TMessage), out var currentSubscriptions))
                return;

            currentlySubscribed = (from sub in currentSubscriptions
                where sub.Subscription.ShouldAttemptDelivery(message)
                select sub).ToList();
        }

        currentlySubscribed.ForEach(sub =>
        {
            try
            {
                sub.Proxy.Deliver(message, sub.Subscription);
            }
            catch (Exception)
            {
                // Ignore any errors and carry on
                // TODO - add to a list of erroring subs and remove them?
            }
        });
    }

    private void PublishAsyncInternal<TMessage>(TMessage message, AsyncCallback? callback) where TMessage : class, IPubSubMessage
    {
        var publishAction = () => PublishInternal(message);

        publishAction.Invoke();
        callback?.Invoke(null!);
    }
}