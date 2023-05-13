using Asciis.App.DependencyInjection;
using Asciis.App.Tests.DependencyInjectionTests.TestData;
using Xunit;

namespace Asciis.App.Tests.DependencyInjectionTests;

[TestClass]
public class PubSubMessengerTests
{
    [Fact]
    public void TinyMessenger_Ctor_DoesNotThrow()
    {
        var messenger = UtilityMethods.GetMessenger();
    }

    [Fact]
    public void Subscribe_ValidDeliverAction_DoesNotThrow()
    {
        var messenger = UtilityMethods.GetMessenger();

        messenger.Subscribe<TestMessage>(new Action<TestMessage>(UtilityMethods.FakeDeliveryAction<TestMessage>));
    }

    [Fact]
    public void SubScribe_ValidDeliveryAction_ReturnsRegistrationObject()
    {
        var messenger = UtilityMethods.GetMessenger();

        var output = messenger.Subscribe<TestMessage>(new Action<TestMessage>(UtilityMethods.FakeDeliveryAction<TestMessage>));

        Assert.IsType<TinyMessageSubscriptionToken>(output);
    }

    [Fact]
    public void Subscribe_ValidDeliverActionWIthStrongReferences_DoesNotThrow()
    {
        var messenger = UtilityMethods.GetMessenger();

        messenger.Subscribe<TestMessage>(new Action<TestMessage>(UtilityMethods.FakeDeliveryAction<TestMessage>), true);
    }

    [Fact]
    public void Subscribe_ValidDeliveryActionAndFilter_DoesNotThrow()
    {
        var messenger = UtilityMethods.GetMessenger();

        messenger.Subscribe<TestMessage>(new Action<TestMessage>(UtilityMethods.FakeDeliveryAction<TestMessage>), new Func<TestMessage, bool>(UtilityMethods.FakeMessageFilter<TestMessage>));
    }

    [Fact]
    //[ExpectedException(typeof(ArgumentNullException))]
    public void Subscribe_NullDeliveryAction_Throws()
    {
        var messenger = UtilityMethods.GetMessenger();

        Assert.Throws<ArgumentNullException>(() => messenger.Subscribe<TestMessage>(null!, new Func<TestMessage, bool>(UtilityMethods.FakeMessageFilter<TestMessage>)));
    }

    [Fact]
    //[ExpectedException(typeof(ArgumentNullException))]
    public void Subscribe_NullFilter_Throws()
    {
        var messenger = UtilityMethods.GetMessenger();

        Assert.Throws<ArgumentNullException>(() => messenger.Subscribe<TestMessage>(new Action<TestMessage>(UtilityMethods.FakeDeliveryAction<TestMessage>), null!, new TestProxy()));
    }

    [Fact]
    //[ExpectedException(typeof(ArgumentNullException))]
    public void Subscribe_NullProxy_Throws()
    {
        var messenger = UtilityMethods.GetMessenger();

        Assert.Throws<ArgumentNullException>(() => messenger.Subscribe<TestMessage>(new Action<TestMessage>(UtilityMethods.FakeDeliveryAction<TestMessage>), new Func<TestMessage, bool>(UtilityMethods.FakeMessageFilter<TestMessage>), null!));
    }

    [Fact]
    //[ExpectedException(typeof(ArgumentNullException))]
    public void Unsubscribe_NullSubscriptionObject_Throws()
    {
        var messenger = UtilityMethods.GetMessenger();

        Assert.Throws<ArgumentNullException>(() => messenger.Unsubscribe<TestMessage>(null!));
    }

    [Fact]
    public void Unsubscribe_PreviousSubscription_DoesNotThrow()
    {
        var messenger = UtilityMethods.GetMessenger();
        var subscription = messenger.Subscribe<TestMessage>(new Action<TestMessage>(UtilityMethods.FakeDeliveryAction<TestMessage>), new Func<TestMessage, bool>(UtilityMethods.FakeMessageFilter<TestMessage>));

        messenger.Unsubscribe<TestMessage>(subscription);
    }

    [Fact]
    public void Subscribe_PreviousSubscription_ReturnsDifferentSubscriptionObject()
    {
        var messenger = UtilityMethods.GetMessenger();
        var sub1 = messenger.Subscribe<TestMessage>(new Action<TestMessage>(UtilityMethods.FakeDeliveryAction<TestMessage>), new Func<TestMessage, bool>(UtilityMethods.FakeMessageFilter<TestMessage>));
        var sub2 = messenger.Subscribe<TestMessage>(new Action<TestMessage>(UtilityMethods.FakeDeliveryAction<TestMessage>), new Func<TestMessage, bool>(UtilityMethods.FakeMessageFilter<TestMessage>));

        Assert.False(object.ReferenceEquals(sub1, sub2));
    }

    [Fact]
    public void Subscribe_CustomProxyNoFilter_DoesNotThrow()
    {
        var messenger = UtilityMethods.GetMessenger();
        var proxy     = new TestProxy();

        messenger.Subscribe<TestMessage>(new Action<TestMessage>(UtilityMethods.FakeDeliveryAction<TestMessage>), proxy);
    }

    [Fact]
    public void Subscribe_CustomProxyWithFilter_DoesNotThrow()
    {
        var messenger = UtilityMethods.GetMessenger();
        var proxy     = new TestProxy();

        messenger.Subscribe<TestMessage>(new Action<TestMessage>(UtilityMethods.FakeDeliveryAction<TestMessage>), new Func<TestMessage, bool>(UtilityMethods.FakeMessageFilter<TestMessage>), proxy);
    }

    [Fact]
    public void Subscribe_CustomProxyNoFilterStrongReference_DoesNotThrow()
    {
        var messenger = UtilityMethods.GetMessenger();
        var proxy     = new TestProxy();

        messenger.Subscribe<TestMessage>(new Action<TestMessage>(UtilityMethods.FakeDeliveryAction<TestMessage>), true, proxy);
    }

    [Fact]
    public void Subscribe_CustomProxyFilterStrongReference_DoesNotThrow()
    {
        var messenger = UtilityMethods.GetMessenger();
        var proxy     = new TestProxy();

        messenger.Subscribe<TestMessage>(new Action<TestMessage>(UtilityMethods.FakeDeliveryAction<TestMessage>), new Func<TestMessage, bool>(UtilityMethods.FakeMessageFilter<TestMessage>), true, proxy);
    }

    [Fact]
    public void Publish_CustomProxyNoFilter_UsesCorrectProxy()
    {
        var messenger = UtilityMethods.GetMessenger();
        var proxy     = new TestProxy();
        messenger.Subscribe<TestMessage>(new Action<TestMessage>(UtilityMethods.FakeDeliveryAction<TestMessage>), proxy);
        var message = new TestMessage(this);

        messenger.Publish<TestMessage>(message);

        Assert.Same(message, proxy.Message);
    }

    [Fact]
    public void Publish_CustomProxyWithFilter_UsesCorrectProxy()
    {
        var messenger = UtilityMethods.GetMessenger();
        var proxy     = new TestProxy();
        messenger.Subscribe<TestMessage>(new Action<TestMessage>(UtilityMethods.FakeDeliveryAction<TestMessage>), new Func<TestMessage, bool>(UtilityMethods.FakeMessageFilter<TestMessage>), proxy);
        var message = new TestMessage(this);

        messenger.Publish<TestMessage>(message);

        Assert.Same(message, proxy.Message);
    }

    [Fact]
    public void Publish_CustomProxyNoFilterStrongReference_UsesCorrectProxy()
    {
        var messenger = UtilityMethods.GetMessenger();
        var proxy     = new TestProxy();
        messenger.Subscribe<TestMessage>(new Action<TestMessage>(UtilityMethods.FakeDeliveryAction<TestMessage>), true, proxy);
        var message = new TestMessage(this);

        messenger.Publish<TestMessage>(message);

        Assert.Same(message, proxy.Message);
    }

    [Fact]
    public void Publish_CustomProxyFilterStrongReference_UsesCorrectProxy()
    {
        var messenger = UtilityMethods.GetMessenger();
        var proxy     = new TestProxy();
        messenger.Subscribe<TestMessage>(new Action<TestMessage>(UtilityMethods.FakeDeliveryAction<TestMessage>), new Func<TestMessage, bool>(UtilityMethods.FakeMessageFilter<TestMessage>), true, proxy);
        var message = new TestMessage(this);

        messenger.Publish<TestMessage>(message);

        Assert.Same(message, proxy.Message);
    }

    [Fact]
    //[ExpectedException(typeof(ArgumentNullException))]
    public void Publish_NullMessage_Throws()
    {
        var messenger = UtilityMethods.GetMessenger();

        Assert.Throws<ArgumentNullException>(() => messenger.Publish<TestMessage>(null!));
    }

    [Fact]
    public void Publish_NoSubscribers_DoesNotThrow()
    {
        var messenger = UtilityMethods.GetMessenger();

        messenger.Publish<TestMessage>(new TestMessage(this));
    }

    [Fact]
    public void Publish_Subscriber_DoesNotThrow()
    {
        var messenger = UtilityMethods.GetMessenger();
        messenger.Subscribe<TestMessage>(new Action<TestMessage>(UtilityMethods.FakeDeliveryAction<TestMessage>), new Func<TestMessage, bool>(UtilityMethods.FakeMessageFilter<TestMessage>));

        messenger.Publish<TestMessage>(new TestMessage(this));
    }

    [Fact]
    public void Publish_SubscribedMessageNoFilter_GetsMessage()
    {
        var  messenger = UtilityMethods.GetMessenger();
        bool received  = false;
        messenger.Subscribe<TestMessage>((m) => { received = true; });

        messenger.Publish<TestMessage>(new TestMessage(this));

        Assert.True(received);
    }

    [Fact]
    public void Publish_SubscribedThenUnsubscribedMessageNoFilter_DoesNotGetMessage()
    {
        var  messenger = UtilityMethods.GetMessenger();
        bool received  = false;
        var  token     = messenger.Subscribe<TestMessage>((m) => { received = true; });
        messenger.Unsubscribe<TestMessage>(token);

        messenger.Publish<TestMessage>(new TestMessage(this));

        Assert.False(received);
    }

    [Fact]
    public void Publish_SubscribedMessageButFiltered_DoesNotGetMessage()
    {
        var  messenger = UtilityMethods.GetMessenger();
        bool received  = false;
        messenger.Subscribe<TestMessage>((m) => { received = true; }, (m) => false);

        messenger.Publish<TestMessage>(new TestMessage(this));

        Assert.False(received);
    }

    [Fact]
    public void Publish_SubscribedMessageNoFilter_GetsActualMessage()
    {
        var          messenger       = UtilityMethods.GetMessenger();
        IPubSubMessage? receivedMessage = null;
        var          payload         = new TestMessage(this);
        messenger.Subscribe<TestMessage>((m) => { receivedMessage = m; });

        messenger.Publish<TestMessage>(payload);

        Assert.Same(payload, receivedMessage);
    }

    [Fact]
    public void GenericTinyMessage_String_SubscribeDoesNotThrow()
    {
        var messenger = UtilityMethods.GetMessenger();
        var output    = string.Empty;
        messenger.Subscribe<GenericPubSubMessage<string>>((m) => { output = m.Content; });
    }

    [Fact]
    public void GenericTinyMessage_String_PubishDoesNotThrow()
    {
        var messenger = UtilityMethods.GetMessenger();
        messenger.Publish(new GenericPubSubMessage<string>(this, "Testing"));
    }

    [Fact]
    public void GenericTinyMessage_String_PubishAndSubscribeDeliversContent()
    {
        var messenger = UtilityMethods.GetMessenger();
        var output    = string.Empty;
        messenger.Subscribe<GenericPubSubMessage<string>>((m) => { output = m.Content; });
        messenger.Publish(new GenericPubSubMessage<string>(this, "Testing"));

        Assert.Equal("Testing", output);
    }

    [Fact]
    public void Publish_SubscriptionThrowingException_DoesNotThrow()
    {
        var messenger = UtilityMethods.GetMessenger();
        messenger.Subscribe<GenericPubSubMessage<string>>((m) => throw new NotImplementedException());

        messenger.Publish(new GenericPubSubMessage<string>(this, "Testing"));
    }

    [Fact]
    public void PublishAsync_NoCallback_DoesNotThrow()
    {
        var messenger = UtilityMethods.GetMessenger();

        messenger.PublishAsync(new TestMessage(this));
    }

    [Fact]
    public void PublishAsync_Callback_EventOnce()
    {
        var syncRoot      = new object();
        var messageCount  = 0;
        var callbackEvent = new ManualResetEvent(false);

        var messenger = UtilityMethods.GetMessenger();
        messenger.Subscribe<TestMessage>(m => { lock (syncRoot) messageCount++; });
        var message = new TestMessage(this);

        messenger.PublishAsync<TestMessage>(message, ar => callbackEvent.Set());

        Assert.True(callbackEvent.WaitOne(1000));
        Assert.Equal(1, messageCount);
    }

    [Fact]
    public void PublishAsync_NoCallback_PublishesMessage()
    {
        var messenger = UtilityMethods.GetMessenger();
        var received  = new ManualResetEvent(false);
        messenger.Subscribe<TestMessage>(m => received.Set());

        messenger.PublishAsync(new TestMessage(this));

        Assert.True(received.WaitOne(1000));
    }

    [Fact]
    public void PublishAsync_Callback_DoesNotThrow()
    {
        var messenger = UtilityMethods.GetMessenger();
        #pragma warning disable 219
        messenger.PublishAsync(new TestMessage(this), (r) => {string test = "Testing";});
        #pragma warning restore 219
    }

    [Fact]
    public void PublishAsync_Callback_PublishesMessage()
    {
        var messenger = UtilityMethods.GetMessenger();
        var received  = new ManualResetEvent(false);
        messenger.Subscribe<TestMessage>(m => received.Set());

        #pragma warning disable 219
        messenger.PublishAsync(new TestMessage(this), (r) => { string test = "Testing"; });
        #pragma warning restore 219

        Assert.True(received.WaitOne(1000));
    }

    [Fact]
    public void PublishAsync_Callback_CallsCallback()
    {
        var messenger        = UtilityMethods.GetMessenger();
        var received         = new ManualResetEvent(false);
        var callbackReceived = new ManualResetEvent(false);
        messenger.Subscribe<TestMessage>(m => received.Set());

        messenger.PublishAsync(new TestMessage(this), r => callbackReceived.Set());

        Assert.True(callbackReceived.WaitOne(1000));
        Assert.True(received.WaitOne(0));
    }

    [Fact]
    public void CancellableGenericTinyMessage_Publish_DoesNotThrow()
    {
        var messenger = UtilityMethods.GetMessenger();
        #pragma warning disable 219
        messenger.Publish<CancellableGenericPubSubMessage<string>>(new CancellableGenericPubSubMessage<string>(this, "Testing", () => { bool test = true; }));
        #pragma warning restore 219
    }

    [Fact]
    //[ExpectedException(typeof(ArgumentNullException))]
    public void CancellableGenericTinyMessage_PublishWithNullAction_Throws()
    {
        var messenger = UtilityMethods.GetMessenger();
        Assert.Throws<ArgumentNullException>(() => messenger.Publish<CancellableGenericPubSubMessage<string>>(new CancellableGenericPubSubMessage<string>(this, "Testing", null!)));
    }

    [Fact]
    public void CancellableGenericTinyMessage_SubscriberCancels_CancelActioned()
    {
        var  messenger = UtilityMethods.GetMessenger();
        bool cancelled = false;
        messenger.Subscribe<CancellableGenericPubSubMessage<string>>((m) => { m.Cancel(); });

        messenger.Publish<CancellableGenericPubSubMessage<string>>(new CancellableGenericPubSubMessage<string>(this, "Testing", () => { cancelled = true; }));

        Assert.True(cancelled);
    }

    [Fact]
    public void CancellableGenericTinyMessage_SeveralSubscribersOneCancels_CancelActioned()
    {
        var  messenger = UtilityMethods.GetMessenger();
        bool cancelled = false;
        #pragma warning disable 219
        messenger.Subscribe<CancellableGenericPubSubMessage<string>>((m) => { var test = 1; });
        messenger.Subscribe<CancellableGenericPubSubMessage<string>>((m) => { m.Cancel(); });
        messenger.Subscribe<CancellableGenericPubSubMessage<string>>((m) => { var test = 1; });
        #pragma warning restore 219
        messenger.Publish<CancellableGenericPubSubMessage<string>>(new CancellableGenericPubSubMessage<string>(this, "Testing", () => { cancelled = true; }));

        Assert.True(cancelled);
    }
}