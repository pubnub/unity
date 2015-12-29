using System;
using PubNubMessaging.Core;
using NUnit.Framework;

namespace PubNubMessaging.Tests
{
    [TestFixture]
    public class PubnubChannelsUnitTests
    {
        #if DEBUG
        [Test]
        public void TestGetPubnubChannelCallbackKey (){ 
            string channel = "testchannel";
            ResponseType respType = ResponseType.Subscribe;
            PubnubChannelCallbackKey callbackKey = new PubnubChannelCallbackKey ();
            callbackKey.Channel = channel;
            callbackKey.Type = respType;

            PubnubChannelCallbackKey callbackKey2 = PubnubCallbacks.GetPubnubChannelCallbackKey(channel, respType);
            Assert.IsTrue (callbackKey.Channel.Equals(callbackKey2.Channel) && callbackKey.Type.Equals(callbackKey2.Type));
        }
        #endif
    }
}

