using System;
using System.Collections.Generic;

namespace PubNubAPI
{
    internal interface IPubNubNonSubscribeBuilder<U>
    {
        U Async();
        U SetChannels (List<string> channels);
        U SetChannelGroups (List<string> channelGroups);
    }
}

