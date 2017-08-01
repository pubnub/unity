using System;
using System.Collections.Generic;

namespace PubNubAPI
{
    internal interface IPubNubSubcribeBuilder<U>
    {
        void Execute();

        void SetChannels (List<string> channels);
        void SetChannelGroups (List<string> channelGroups);
    }
}

