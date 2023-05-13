using No8.Ascii.DependencyInjection;

namespace No8.Ascii.Tests.DependencyInjectionTests.TestData;

public class TestMessage : PubSubMessageBase
{
    public TestMessage(object sender) : base(sender)
    {
            
    }
}

public class TestProxy : ITinyMessageProxy
{
    public IPubSubMessage? Message {get; private set;}

    public void Deliver(IPubSubMessage message, ITinyMessageSubscription subscription)
    {
        Message = message;
        subscription.Deliver(message);
    }
}