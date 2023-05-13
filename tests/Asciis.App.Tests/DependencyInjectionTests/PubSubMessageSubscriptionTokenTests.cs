using System;
using Asciis.App.DependencyInjection;
using Asciis.App.Tests.DependencyInjectionTests.Helpers;
using Asciis.App.Tests.DependencyInjectionTests.TestData;
using Xunit;

namespace Asciis.App.Tests.DependencyInjectionTests;

[TestClass]
public class PubSubMessageSubscriptionTokenTests
{
#if MOQ
        [Fact]
        public void Dispose_WithValidHubReference_UnregistersWithHub()
        {
            var messengerMock = new Moq.Mock<ITinyMessengerHub>();
            messengerMock.Setup((messenger) => messenger.Unsubscribe<TestMessage>(Moq.It.IsAny<TinyMessageSubscriptionToken>())).Verifiable();
            var token = new TinyMessageSubscriptionToken(messengerMock.Object, typeof(TestMessage));

            token.Dispose();

            messengerMock.VerifyAll();
        }
#endif

    // can't do GC.WaitForFullGCComplete in WinRT...
    [Fact]
    public void Dispose_WithInvalidHubReference_DoesNotThrow()
    {
        var token = UtilityMethods.GetTokenWithOutOfScopeMessenger();
        GC.Collect();
        GC.WaitForFullGCComplete(2000);

        token.Dispose();
    }

    [Fact]
    //[ExpectedException(typeof(ArgumentNullException))]
    public void Ctor_NullHub_ThrowsArgumentNullException()
    {
        var messenger = UtilityMethods.GetMessenger();

        Assert.Throws<ArgumentNullException>(() => new TinyMessageSubscriptionToken(null!, typeof(IPubSubMessage)));
    }

    [Fact]
    //[ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Ctor_InvalidMessageType_ThrowsArgumentOutOfRangeException()
    {
        var messenger = UtilityMethods.GetMessenger();

        Assert.Throws<ArgumentOutOfRangeException>(() => new TinyMessageSubscriptionToken(messenger, typeof(object)));
    }

    [Fact]
    public void Ctor_ValidHubAndMessageType_DoesNotThrow()
    {
        var messenger = UtilityMethods.GetMessenger();

        var token = new TinyMessageSubscriptionToken(messenger, typeof(TestMessage));
    }
}